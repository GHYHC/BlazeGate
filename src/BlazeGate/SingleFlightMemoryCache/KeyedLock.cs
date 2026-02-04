using System.Collections.Concurrent;

namespace BlazeGate.SingleFlightMemoryCache
{
    /// <summary>
    /// 基于键的异步锁（带引用计数的安全回收）
    /// </summary>
    public sealed class KeyedLock : IKeyedLock
    {
        private readonly ConcurrentDictionary<string, Entry> locks = new(StringComparer.Ordinal);

        public async Task<IDisposable> AcquireAsync(string key, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key不能为空。", nameof(key));

            var entry = locks.GetOrAdd(key, static _ => new Entry());

            // 引用计数：包含等待者/持有者
            entry.AddRef();

            try
            {
                await entry.Semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                return new Releaser(this, key, entry);
            }
            catch
            {
                // WaitAsync 失败/取消也要归还引用计数
                ReleaseRef(key, entry);
                throw;
            }
        }

        private void ReleaseAndRef(string key, Entry entry)
        {
            try
            {
                entry.Semaphore.Release();
            }
            finally
            {
                ReleaseRef(key, entry);
            }
        }

        private void ReleaseRef(string key, Entry entry)
        {
            if (entry.ReleaseRef() == 0)
            {
                // 只在字典中仍是该 entry 时才移除，避免并发下误删新 entry
                locks.TryRemove(new KeyValuePair<string, Entry>(key, entry));
            }
        }

        private sealed class Releaser : IDisposable
        {
            private readonly KeyedLock owner;
            private readonly string key;
            private readonly Entry entry;
            private int disposed;

            public Releaser(KeyedLock owner, string key, Entry entry)
            {
                this.owner = owner;
                this.key = key;
                this.entry = entry;
            }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref disposed, 1) != 0) return;
                owner.ReleaseAndRef(key, entry);
            }
        }

        private sealed class Entry
        {
            public SemaphoreSlim Semaphore { get; } = new(1, 1);

            // 引用计数：等待/持有 Acquire 的总数
            private int refCount;

            public void AddRef() => Interlocked.Increment(ref refCount);

            /// <summary>
            /// 返回释放后的引用计数
            /// </summary>
            public int ReleaseRef() => Interlocked.Decrement(ref refCount);
        }
    }

    public interface IKeyedLock
    {
        public Task<IDisposable> AcquireAsync(string key, CancellationToken cancellationToken);
    }
}
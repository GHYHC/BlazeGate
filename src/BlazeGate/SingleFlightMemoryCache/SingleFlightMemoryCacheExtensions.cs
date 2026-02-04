namespace BlazeGate.SingleFlightMemoryCache
{
    public static class SingleFlightMemoryCacheExtensions
    {
        /// <summary>
        /// 添加单飞内存缓存服务
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AddSingleFlightMemoryCache(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<IKeyedLock, KeyedLock>();
            services.AddSingleton<ISingleFlightMemoryCache, SingleFlightMemoryCache>();

            return services;
        }
    }
}
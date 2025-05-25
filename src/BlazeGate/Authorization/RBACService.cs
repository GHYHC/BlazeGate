using BlazeGate.Model.EFCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlazeGate.Authorization
{
    public class RBACService : IRBACService
    {
        private readonly BlazeGateContext BlazeGateContext;
        private readonly IMemoryCache memoryCache;

        public RBACService(BlazeGateContext BlazeGateContext, IMemoryCache memoryCache)
        {
            this.BlazeGateContext = BlazeGateContext;
            this.memoryCache = memoryCache;
        }

        public async Task<bool> IsAuthorized(string account, string serviceName, string path)
        {
            if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(serviceName))
            {
                return false;
            }

            //把serviceName和path转为小写
            serviceName = serviceName.ToLower();
            path = path.ToLower();

            string key = $"pathCache-{account}-{serviceName}";

            List<string> paths = await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);
                return await GetPath(account, serviceName);
            }) ?? new List<string>();

            return paths.Contains(path);
        }

        private async Task<List<string>> GetPath(string account, string serviceName)
        {
            long userId = BlazeGateContext.Users.Where(u => u.Account == account && u.Enabled == true).Select(u => u.Id).FirstOrDefault();
            long serviceId = BlazeGateContext.Services.Where(s => s.ServiceName.ToLower() == serviceName.ToLower() && s.Enabled == true).Select(u => u.Id).FirstOrDefault();

            if (userId == 0 || serviceId == 0)
            {
                return await Task.FromResult(new List<string>());
            }

            List<long> roleIds = BlazeGateContext.UserRoles.Where(ur => ur.UserId == userId && ur.ServiceId == serviceId).Select(ur => ur.RoleId).ToList();

            List<long> pageIds = BlazeGateContext.RolePages.Where(rp => roleIds.Contains(rp.RoleId) && rp.ServiceId == serviceId).Select(rp => rp.PageId).ToList();

            List<Page> pages = BlazeGateContext.Pages.Where(p => pageIds.Contains(p.Id)).Select(p => new Page() { Path = p.Path, SubPath = p.SubPath }).ToList();

            List<string> paths = new List<string>();
            foreach (var page in pages)
            {
                if (!string.IsNullOrWhiteSpace(page.Path))
                {
                    paths.Add(page.Path.ToLower());
                }
                page.SubPath?.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(subPath => paths.Add(subPath.ToLower()));
            }
            return await Task.FromResult(paths.Distinct().ToList());
        }
    }

    public interface IRBACService
    {
        Task<bool> IsAuthorized(string account, string serviceName, string path);
    }
}
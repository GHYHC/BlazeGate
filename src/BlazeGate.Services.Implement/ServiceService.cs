using BlazeGate.Common.Autofac;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.Policies;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Interface;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Implement
{
    public class ServiceService : IServiceService, IScopeDenpendency
    {
        private readonly BlazeGateContext context;
        private readonly ISnowFlakeService snowFlake;
        private readonly IAuthRsaKeyService authRsaKeyService;

        public ServiceService(BlazeGateContext context, ISnowFlakeService snowFlake, IAuthRsaKeyService authRsaKeyService)
        {
            this.context = context;
            this.snowFlake = snowFlake;
            this.authRsaKeyService = authRsaKeyService;
        }

        public async Task<ApiResult<long>> AddService()
        {
            Service service = new Service();
            service.Id = await snowFlake.NextId();
            service.ServiceName = $"DefaultService{service.Id}";
            service.Enabled = false;
            service.CreateTime = DateTime.Now;
            service.UpdateTime = DateTime.Now;
            context.Services.Add(service);

            //添加服务配置
            ServiceConfig serviceConfig = new ServiceConfig();
            serviceConfig.Id = await snowFlake.NextId();
            serviceConfig.ServiceId = service.Id;
            serviceConfig.ServiceName = service.ServiceName;
            serviceConfig.SessionAffinityKeyName = "SessionAffinity" + service.Id;
            serviceConfig.CreateTime = DateTime.Now;
            serviceConfig.UpdateTime = DateTime.Now;
            context.ServiceConfigs.Add(serviceConfig);

            //添加授权RSA秘钥
            var createKeyResult = await authRsaKeyService.CreateKey();
            if (createKeyResult.Success)
            {
                var rsaKey = createKeyResult.Data;
                AuthRsaKey authRsaKey = new AuthRsaKey();
                authRsaKey.Id = await snowFlake.NextId();
                authRsaKey.ServiceId = service.Id;
                authRsaKey.ServiceName = service.ServiceName;
                authRsaKey.PublicKey = rsaKey.PublicKey;
                authRsaKey.PrivateKey = rsaKey.PrivateKey;
                authRsaKey.CreateTime = DateTime.Now;
                authRsaKey.UpdateTime = DateTime.Now;
                context.AuthRsaKeys.Add(authRsaKey);
            }

            //添加默认角色
            Role role = new Role();
            role.Id = await snowFlake.NextId();
            role.ServiceId = service.Id;
            role.ServiceName = service.ServiceName;
            role.RoleName = "admin";
            role.CreateTime = DateTime.Now;
            role.UpdateTime = DateTime.Now;
            context.Roles.Add(role);

            //添加默认页面
            List<Page> pages = await GetDefaultPage(service);
            context.Pages.AddRange(pages);

            //添加默认角色页面
            List<RolePage> rolePages = await GetDefaultRolePage(role, pages);
            context.RolePages.AddRange(rolePages);

            int result = await context.SaveChangesAsync();

            return ApiResult<long>.Result(result > 0, service.Id);
        }

        /// <summary>
        /// 获取默认页面
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task<List<Page>> GetDefaultPage(Service service)
        {
            List<Page> pages = new List<Page>();

            Page systemSetting = new Page
            {
                Id = await snowFlake.NextId(),
                ServiceId = service.Id,
                ServiceName = service.ServiceName,
                ParentPageId = 0,
                IndexNumber = 0,
                Type = 0,
                Title = "系统设置",
                Icon = "smile",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            pages.Add(systemSetting);

            //页面管理
            Page pageManager = new Page
            {
                Id = await snowFlake.NextId(),
                ServiceId = service.Id,
                ServiceName = service.ServiceName,
                ParentPageId = systemSetting.Id,
                IndexNumber = 0,
                Type = 0,
                Title = "页面管理",
                Icon = "smile",
                Path = "/Page/Index",
                SubPath = string.Join("\r\n",
                [
                    "/api/Page/GetPageByServiceName",
                    "/api/Page/SavePage",
                    "/api/Page/RemovePage",
                    "/api/Page/SaveDrop"
                ]),
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            pages.Add(pageManager);

            //角色管理
            Page roleManager = new Page
            {
                Id = await snowFlake.NextId(),
                ServiceId = service.Id,
                ServiceName = service.ServiceName,
                ParentPageId = systemSetting.Id,
                IndexNumber = 1,
                Type = 0,
                Title = "角色管理",
                Icon = "smile",
                Path = "/Role/Index",
                SubPath = string.Join("\r\n",
                [
                    "/api/Role/QueryByPage",
                    "/api/Role/RemoveById",
                    "/api/Role/SaveRole"
                ]),
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            pages.Add(roleManager);

            //用户管理
            Page userManager = new Page
            {
                Id = await snowFlake.NextId(),
                ServiceId = service.Id,
                ServiceName = service.ServiceName,
                ParentPageId = systemSetting.Id,
                IndexNumber = 2,
                Type = 0,
                Title = "用户管理",
                Icon = "smile",
                Path = "/User/Index",
                SubPath = string.Join("\r\n",
                [
                    "/api/User/QueryByPage",
                    "/api/User/ChangeUserEnabled",
                    "/api/User/RemoveById",
                    "/api/User/SaveUser"
                ]),
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            pages.Add(userManager);

            //用户角色管理
            Page userRoleManager = new Page
            {
                Id = await snowFlake.NextId(),
                ServiceId = service.Id,
                ServiceName = service.ServiceName,
                ParentPageId = systemSetting.Id,
                IndexNumber = 3,
                Type = 0,
                Title = "用户角色管理",
                Icon = "smile",
                Path = "/UserRole/Index",
                SubPath = string.Join("\r\n",
                [
                    "/api/UserRole/QueryByPage",
                    "/api/UserRole/RemoveById",
                    "/api/UserRole/GetRoleByServiceName",
                    "/api/UserRole/SaveUserRole"
                ]),
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            pages.Add(userRoleManager);

            return pages;
        }

        /// <summary>
        /// 获取默认角色页面
        /// </summary>
        /// <param name="role"></param>
        /// <param name="pages"></param>
        /// <returns></returns>
        public async Task<List<RolePage>> GetDefaultRolePage(Role role, List<Page> pages)
        {
            List<RolePage> rolePages = new List<RolePage>();
            foreach (Page page in pages)
            {
                RolePage rolePage = new RolePage
                {
                    RoleId = role.Id,
                    PageId = page.Id,
                    ServiceId = role.ServiceId,
                    ServiceName = role.ServiceName,
                };
                rolePages.Add(rolePage);
            }
            return rolePages;
        }

        public async Task<ApiResult<long>> DeleteService(long serviceId)
        {
            var serviceConfigs = context.ServiceConfigs.AsNoTracking().Where(b => b.ServiceId == serviceId).ToList();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                context.Services.Where(b => b.Id == serviceId).ExecuteDelete();
                context.ServiceConfigs.Where(b => b.ServiceId == serviceId).ExecuteDelete();
                context.Destinations.Where(b => b.ServiceId == serviceId).ExecuteDelete();
                context.Pages.Where(b => b.ServiceId == serviceId).ExecuteDelete();
                context.Roles.Where(b => b.ServiceId == serviceId).ExecuteDelete();
                context.RolePages.Where(b => b.ServiceId == serviceId).ExecuteDelete();
                context.UserRoles.Where(b => b.ServiceId == serviceId).ExecuteDelete();
                context.AuthWhiteLists.Where(b => b.ServiceId == serviceId).ExecuteDelete();
                context.AuthRsaKeys.Where(b => b.ServiceId == serviceId).ExecuteDelete();
                await transaction.CommitAsync();

                return ApiResult<long>.SuccessResult(serviceId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResult<long>.FailResult(ex.Message);
            }
        }

        public async Task<ApiResult<int>> EnabledChanged(long serviceId, bool enabled)
        {
            var result = await context.Services
                .Where(b => b.Id == serviceId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.Enabled, t => enabled)
                    .SetProperty(t => t.UpdateTime, t => DateTime.Now));

            return ApiResult<int>.Result(result > 0, result);
        }

        public async Task<ApiResult<List<Service>>> LoadData(string serviceName)
        {
            var where = PredicateBuilder.New<Service>(true);

            if (!string.IsNullOrEmpty(serviceName))
            {
                where.And(x => x.ServiceName.Contains(serviceName));
            }

            var list = await context.Services.AsNoTracking().Where(where).OrderByDescending(b => b.Id).ToListAsync();

            return ApiResult<List<Service>>.SuccessResult(list);
        }

        public async Task<ApiResult<ServiceDetails>> GetServiceDetailsByName(string serviceName)
        {
            ServiceDetails details = new ServiceDetails();

            details.Service = await context.Services.AsNoTracking().Where(b => b.ServiceName == serviceName).FirstOrDefaultAsync();
            details.ServiceConfig = await context.ServiceConfigs.AsNoTracking().Where(b => b.ServiceName == serviceName).FirstOrDefaultAsync();

            return ApiResult<ServiceDetails>.Result(details.Service != null, details);
        }

        public async Task<ApiResult<long>> UpdateServiceDetails(ServiceDetails serviceDetails)
        {
            //判断ServiceName是否为系统保留名称,不区分大小写
            if (serviceDetails.Service.ServiceName.Equals("api", StringComparison.OrdinalIgnoreCase))
            {
                return await Task.FromResult(ApiResult<long>.FailResult("服务名称为系统保留名称"));
            }

            //判断是否有重复的ServiceName
            if (context.Services.AsNoTracking().Where(b => b.ServiceName == serviceDetails.ServiceConfig.ServiceName && b.Id != serviceDetails.ServiceConfig.ServiceId).Any())
            {
                return await Task.FromResult(ApiResult<long>.FailResult("服务名称已存在"));
            }

            //查询原ServiceName
            string oldServiceName = context.Services.AsNoTracking().Where(b => b.Id == serviceDetails.ServiceConfig.ServiceId).Select(b => b.ServiceName).FirstOrDefault();

            using var transaction = context.Database.BeginTransaction();
            try
            {
                //修改服务
                serviceDetails.Service.UpdateTime = DateTime.Now;
                context.Services.Update(serviceDetails.Service);

                //修改服务配置
                serviceDetails.ServiceConfig.ServiceName = serviceDetails.Service.ServiceName;
                serviceDetails.ServiceConfig.UpdateTime = DateTime.Now;
                context.ServiceConfigs.Update(serviceDetails.ServiceConfig);

                //判断ServiceName是否有变化
                if (serviceDetails.Service.ServiceName != oldServiceName)
                {
                    //修改角色
                    context.Roles.Where(b => b.ServiceId == serviceDetails.ServiceConfig.ServiceId).ExecuteUpdate(s => s
                        .SetProperty(t => t.ServiceName, t => serviceDetails.Service.ServiceName)
                        .SetProperty(t => t.UpdateTime, t => DateTime.Now));
                    //修改页面
                    context.Pages.Where(b => b.ServiceId == serviceDetails.ServiceConfig.ServiceId).ExecuteUpdate(s => s
                        .SetProperty(t => t.ServiceName, t => serviceDetails.Service.ServiceName)
                        .SetProperty(t => t.UpdateTime, t => DateTime.Now));
                    //修改角色页面
                    context.RolePages.Where(b => b.ServiceId == serviceDetails.ServiceConfig.ServiceId).ExecuteUpdate(s => s
                        .SetProperty(t => t.ServiceName, t => serviceDetails.Service.ServiceName));
                    //修改用户角色
                    context.UserRoles.Where(b => b.ServiceId == serviceDetails.ServiceConfig.ServiceId).ExecuteUpdate(s => s
                        .SetProperty(t => t.ServiceName, t => serviceDetails.Service.ServiceName));
                    //修改目标
                    context.Destinations.Where(b => b.ServiceId == serviceDetails.ServiceConfig.ServiceId).ExecuteUpdate(s => s
                        .SetProperty(t => t.ServiceName, t => serviceDetails.Service.ServiceName));
                    //修改白名单
                    context.AuthWhiteLists.Where(b => b.ServiceId == serviceDetails.ServiceConfig.ServiceId).ExecuteUpdate(s => s
                        .SetProperty(t => t.ServiceName, t => serviceDetails.Service.ServiceName));
                    //修改RSA秘钥
                    context.AuthRsaKeys.Where(b => b.ServiceId == serviceDetails.ServiceConfig.ServiceId).ExecuteUpdate(s => s
                        .SetProperty(t => t.ServiceName, t => serviceDetails.Service.ServiceName));
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return ApiResult<long>.SuccessResult(serviceDetails.ServiceConfig.ServiceId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResult<long>.FailResult(ex.Message);
            }
        }

        public async Task<ApiResult<Service>> GetServiceById(long serviceId)
        {
            var service = await context.Services.AsNoTracking().Where(b => b.Id == serviceId).FirstOrDefaultAsync();
            return ApiResult<Service>.Result(service != null, service);
        }

        public async Task<ApiResult<string>> GetDestinationConfig(long serviceId)
        {
            string blazeGateTemplate =
@"  ""BlazeGate"": {{
    ""BlazeGateAddress"": ""BlazeGate地址 如:http://localhost:5182"",
    ""ServiceName"": ""{0}"",
    ""Token"": ""{1}"",
    ""Address"": ""当前目标的注册地址 如:http://localhost:5037""
  }}";

            string jwtConfig =
@"
  ""Jwt"": {{
    ""Audience"": ""{0}"",
    ""Issuer"": ""BlazeGate"",
    ""PublicKey"": ""{1}""
  }}";

            var service = await context.Services.AsNoTracking()
                          .Where(b => b.Id == serviceId)
                          .FirstOrDefaultAsync();

            var serviceConfig = await context.ServiceConfigs.AsNoTracking()
                          .Where(b => b.ServiceId == serviceId)
                          .FirstOrDefaultAsync();

            StringBuilder config = new StringBuilder();
            config.Append(string.Format(blazeGateTemplate, serviceConfig?.ServiceName, service?.Token));

            if (AuthorizationPolicies.defaultPolicies.Equals(serviceConfig.AuthorizationPolicy) || AuthorizationPolicies.RBAC.Equals(serviceConfig.AuthorizationPolicy))
            {
                var rsaKey = await context.AuthRsaKeys.AsNoTracking()
                          .Where(b => b.ServiceId == serviceId)
                          .FirstOrDefaultAsync();

                config.Append(",");

                //将换行符替换为\r\n
                config.Append(string.Format(jwtConfig, serviceConfig?.ServiceName, rsaKey?.PublicKey?.Replace("\r", "\\r").Replace("\n", "\\n")));
            }

            return ApiResult<string>.SuccessResult(config.ToString());
        }
    }
}
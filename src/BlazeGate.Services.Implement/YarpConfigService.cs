using BlazeGate.Common;
using BlazeGate.Common.Autofac;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.Helper;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Services.Interface;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarp.ReverseProxy;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Health;
using Yarp.ReverseProxy.Model;

namespace BlazeGate.Services.Implement
{
    internal class YarpConfigService : IYarpConfigService, IScopeDenpendency
    {
        private readonly BlazeGateContext blazeGateContext;
        private readonly ILogger<YarpConfigService> logger;
        private readonly IConfigValidator proxyConfigValidator;
        private readonly InMemoryConfigProvider proxyConfigProvider;
        private readonly IProxyStateLookup proxyState;
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;

        public YarpConfigService(BlazeGateContext blazeGateContext, ILogger<YarpConfigService> logger, IConfigValidator proxyConfigValidator, InMemoryConfigProvider proxyConfigProvider, IProxyStateLookup proxyState, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.blazeGateContext = blazeGateContext;
            this.logger = logger;
            this.proxyConfigValidator = proxyConfigValidator;
            this.proxyConfigProvider = proxyConfigProvider;
            this.proxyState = proxyState;
            this.configuration = configuration;
            this.httpClient = httpClientFactory.CreateClient();
        }

        public async Task<ApiResult<bool>> UpdateAll(AuthBaseInfo auth)
        {
            var clusterAddress = configuration.GetSection("BlazeGateClusterAddress").Get<string[]>();

            //如果集群中没有节点，则更新当前节点
            if (clusterAddress == null || clusterAddress.Length == 0)
            {
                return await Update(auth);
            }

            //更新集群中的所有节点
            string errorMsg = null;
            foreach (var item in clusterAddress)
            {
                try
                {
                    string url = StringHelper.CombineUrl(item, "api/YarpConfig/Update");
                    var result = await httpClient.HttpPostAsJsonAsync<AuthBaseInfo, ApiResult<bool>>(url, auth);
                    if (!result.Success)
                    {
                        errorMsg += $"{item}：{result.Msg}\n";
                    }
                }
                catch (Exception ex)
                {
                    errorMsg = $"{item}：{ex.Message}\n";
                }
            }

            return ApiResult<bool>.Result(string.IsNullOrWhiteSpace(errorMsg), string.IsNullOrWhiteSpace(errorMsg), errorMsg);
        }

        public async Task<ApiResult<bool>> Update(AuthBaseInfo auth)
        {
            try
            {
                logger.LogInformation($"路由配置开始更新");

                //获取服务配置和目标
                GetServiceConfigAndDestination(out List<ServiceConfig> configs, out List<Destination> destinations, null);

                //提取集群和路由
                var clusters = await ExtractClusters(configs, destinations);
                var routes = await ExtractRoutes(configs);

                //更新路由配置
                proxyConfigProvider.Update(routes, clusters);

                //更新目标健康状态
                await UpdateDestinationHealthState(destinations);

                //移除不健康超时的目标
                await RemoveUnhealthyTimeoutDestination(configs);

                logger.LogInformation($"路由配置更新完成");

                return ApiResult<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"更新路由配置时出现错误：{ex.Message}");

                return ApiResult<bool>.FailResult(ex.Message);
            }
        }

        /// <summary>
        /// 更新目标健康状态
        /// </summary>
        /// <param name="destinations"></param>
        /// <returns></returns>
        private async Task UpdateDestinationHealthState(List<Destination> destinations)
        {
            foreach (Destination dest in destinations)
            {
                if (proxyState.TryGetCluster(dest.ServiceName, out Yarp.ReverseProxy.Model.ClusterState cluster))
                {
                    string key = $"{dest.ServiceName} {dest.Address}";

                    //判断cluster.Destinations是否存在key
                    if (cluster.Destinations.TryGetValue(key, out Yarp.ReverseProxy.Model.DestinationState destination))
                    {
                        if (dest.PassiveHealthState != destination.Health.Passive.ToString())
                        {
                            //更新被动健康状态
                            int result = await blazeGateContext.Destinations
                                   .Where(b => b.Id == dest.Id && b.PassiveHealthStateUpdateTime == dest.PassiveHealthStateUpdateTime)
                                       .ExecuteUpdateAsync(s => s
                                           .SetProperty(t => t.PassiveHealthState, t => destination.Health.Passive.ToString())
                                           .SetProperty(t => t.PassiveHealthStateUpdateTime, t => DateTime.Now)
                                           .SetProperty(t => t.UpdateTime, t => DateTime.Now));

                            if (result > 0)
                            {
                                logger.LogInformation($"[{key}]更新被动健康状态：{dest.ActiveHealthState} -> {destination.Health.Active}");
                            }
                            else
                            {
                                logger.LogWarning($"[{key}]更新被动健康状态失败：{dest.ActiveHealthState} -> {destination.Health.Active}");
                            }
                        }
                        if (dest.ActiveHealthState != destination.Health.Active.ToString())
                        {
                            //更新主动健康状态
                            int result = await blazeGateContext.Destinations
                                .Where(b => b.Id == dest.Id && b.ActiveHealthStateUpdateTime == dest.ActiveHealthStateUpdateTime)
                                    .ExecuteUpdateAsync(s => s
                                        .SetProperty(t => t.ActiveHealthState, t => destination.Health.Active.ToString())
                                        .SetProperty(t => t.ActiveHealthStateUpdateTime, t => DateTime.Now)
                                        .SetProperty(t => t.UpdateTime, t => DateTime.Now));

                            if (result > 0)
                            {
                                logger.LogInformation($"[{key}]更新主动健康状态：{dest.ActiveHealthState} -> {destination.Health.Active}");
                            }
                            else
                            {
                                logger.LogWarning($"[{key}]更新主动健康状态失败：{dest.ActiveHealthState} -> {destination.Health.Active}");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 移除不健康超时的目标
        /// </summary>
        /// <returns></returns>
        public async Task RemoveUnhealthyTimeoutDestination(List<ServiceConfig> configs)
        {
            foreach (ServiceConfig config in configs)
            {
                if (config.ActiveHealthCheckEnabled && config.ActiveRemoveUnhealthyAfter > 0)
                {
                    int result = await blazeGateContext.Destinations.Where(b => b.ServiceId == config.ServiceId && b.ActiveHealthState == DestinationHealth.Unhealthy.ToString() && b.ActiveHealthStateUpdateTime <= DateTime.Now.AddMinutes(-config.ActiveRemoveUnhealthyAfter)).ExecuteDeleteAsync();

                    if (result > 0)
                    {
                        logger.LogInformation($"[{config.ServiceName}]移除不健康超时的目标：{result}个");
                    }
                }
            }
        }

        /// <summary>
        /// 获取服务配置和目标
        /// </summary>
        /// <param name="configs"></param>
        /// <param name="destinations"></param>
        /// <param name="serviceName"></param>
        private void GetServiceConfigAndDestination(out List<ServiceConfig> configs, out List<Destination> destinations, string serviceName)
        {
            var serviceWhere = PredicateBuilder.New<Service>(true);
            serviceWhere.And(x => x.Enabled == true);
            if (!string.IsNullOrWhiteSpace(serviceName))
            {
                serviceWhere.And(x => x.ServiceName == serviceName);
            }

            configs = (from s in blazeGateContext.Services.Where(serviceWhere)
                       join c in blazeGateContext.ServiceConfigs on s.Id equals c.ServiceId
                       select c).ToList();

            destinations = (from s in blazeGateContext.Services.Where(serviceWhere)
                            join d in blazeGateContext.Destinations on s.Id equals d.ServiceId
                            select d).ToList();
        }

        /// <summary>
        /// 提取路由
        /// </summary>
        /// <param name="configs"></param>
        /// <returns></returns>
        private async Task<List<RouteConfig>> ExtractRoutes(List<ServiceConfig> configs)
        {
            var routes = new List<RouteConfig>();

            foreach (var config in configs)
            {
                RouteConfig route = new RouteConfig
                {
                    ClusterId = config.ServiceName,
                    RouteId = $"{config.ServiceName}-route",
                    Match = new RouteMatch
                    {
                        Path = $"/{config.ServiceName}/{{**restOfPath}}"
                    },
                    Transforms = new IReadOnlyDictionary<string, string>[]
                    {
                        new Dictionary<string, string>
                        {
                            { "PathPattern", "/{**restOfPath}" }
                        }
                    },
                    RateLimiterPolicy = string.IsNullOrWhiteSpace(config.RateLimiterPolicy) ? null : config.RateLimiterPolicy,
                    AuthorizationPolicy = string.IsNullOrWhiteSpace(config.AuthorizationPolicy) ? null : config.AuthorizationPolicy,
                };

                //验证路由
                var routeErrs = await proxyConfigValidator.ValidateRouteAsync(route);
                if (routeErrs.Any())
                {
                    logger.LogError($"[{config.ServiceName}]生成路由时出现错误");
                    foreach (var err in routeErrs)
                    {
                        logger.LogError(err, $"[{config.ServiceName}] 路由验证错误");
                    }
                    continue;
                }

                routes.Add(route);
            }

            return routes;
        }

        /// <summary>
        /// 提取集群
        /// </summary>
        /// <param name="configs"></param>
        /// <param name="destinations"></param>
        /// <returns></returns>
        private async Task<List<ClusterConfig>> ExtractClusters(List<ServiceConfig> configs, List<Destination> destinations)
        {
            var clusters = new List<ClusterConfig>();

            foreach (var config in configs)
            {
                ClusterConfig cluster = new ClusterConfig
                {
                    ClusterId = config.ServiceName,
                    //设置负载均衡策略
                    LoadBalancingPolicy = string.IsNullOrWhiteSpace(config.LoadBalancingPolicy) ? null : config.LoadBalancingPolicy,
                    //设置健康检查
                    HealthCheck = new HealthCheckConfig
                    {
                        //被动健康检查
                        Passive = new PassiveHealthCheckConfig
                        {
                            Enabled = config.PassiveHealthCheckEnabled,
                            Policy = string.IsNullOrWhiteSpace(config.PassiveHealthCheckPolicy) ? null : config.PassiveHealthCheckPolicy,
                            ReactivationPeriod = TimeSpan.FromSeconds(config.PassiveHealthCheckReactivationPeriod)
                        },
                        //主动健康检查
                        Active = new ActiveHealthCheckConfig
                        {
                            Enabled = config.ActiveHealthCheckEnabled,
                            Interval = TimeSpan.FromSeconds(config.ActiveHealthCheckInterval),
                            Timeout = TimeSpan.FromSeconds(config.ActiveHealthCheckTimeout),
                            Policy = string.IsNullOrWhiteSpace(config.ActiveHealthCheckPolicy) ? null : config.ActiveHealthCheckPolicy,
                            Path = config.ActiveHealthCheckPath,
                        }
                    },
                    //设置请求超时时间
                    HttpRequest = new ForwarderRequestConfig
                    {
                        ActivityTimeout = TimeSpan.FromSeconds(config.RequestActivityTimeout),
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        //主动健康检查阈值
                        { ConsecutiveFailuresHealthPolicyOptions.ThresholdMetadataName, config.ActiveHealthCheckThreshold.ToString() },

                        //被动健康检查失败率
                        { TransportFailureRateHealthPolicyOptions.FailureRateLimitMetadataName, config.PassiveHealthCheckFailureRate.ToString() }
                    },
                    //设置关联性会话
                    SessionAffinity = new SessionAffinityConfig
                    {
                        Enabled = config.SessionAffinityEnabled,
                        Policy = string.IsNullOrWhiteSpace(config.SessionAffinityPolicy) ? null : config.SessionAffinityPolicy,
                        FailurePolicy = string.IsNullOrWhiteSpace(config.SessionAffinityFailurePolicy) ? null : config.SessionAffinityFailurePolicy,
                        AffinityKeyName = config.SessionAffinityKeyName,
                        Cookie = new SessionAffinityCookieConfig
                        {
                            HttpOnly = config.SessionAffinityCookieHttpOnly,
                            MaxAge = config.SessionAffinityCookieMaxAge == 0 ? null : TimeSpan.FromSeconds(config.SessionAffinityCookieMaxAge),
                        }
                    },
                    Destinations = destinations.Where(b => b.ServiceId == config.ServiceId)
                    ?.Select(
                        b => new KeyValuePair<string, DestinationConfig>(
                            $"{config.ServiceName} {b.Address}",
                            new DestinationConfig()
                            {
                                Address = b.Address,
                                Health = b.Address,
                                Metadata = new Dictionary<string, string> {
                                    { "DestinationId", b.Id.ToString() },
                                    { "ServiceId", b.ServiceId.ToString() },
                                    { "ServiceName", b.ServiceName },
                                }
                            })
                        ).ToDictionary()
                };

                //验证集群
                var clusterErrs = await proxyConfigValidator.ValidateClusterAsync(cluster);
                if (clusterErrs.Any())
                {
                    logger.LogError($"[{config.ServiceName}]生成集群时出现错误");
                    foreach (var err in clusterErrs)
                    {
                        logger.LogError(err, $"[{config.ServiceName}] 集群验证错误");
                    }
                    continue;
                }

                clusters.Add(cluster);
            }

            return clusters;
        }
    }
}
using BlazeGate.Common.Autofac;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Interface;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarp.ReverseProxy;
using Yarp.ReverseProxy.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BlazeGate.Services.Implement
{
    public class DestinationService : IDestinationService, IScopeDenpendency
    {
        private readonly BlazeGateContext context;
        private readonly ISnowFlakeService snowFlake;
        private readonly IYarpConfigService yarpConfig;
        private readonly IProxyStateLookup proxyState;

        public DestinationService(BlazeGateContext context, ISnowFlakeService snowFlake, IYarpConfigService yarpConfig, IProxyStateLookup proxyState)
        {
            this.context = context;
            this.snowFlake = snowFlake;
            this.yarpConfig = yarpConfig;
            this.proxyState = proxyState;
        }

        public async Task<ApiResult<bool>> Add(DestinationInfo info)
        {
            var service = await context.Services.Where(b => b.ServiceName == info.ServiceName && b.Enabled == true).FirstOrDefaultAsync();
            if (service == null)
            {
                return ApiResult<bool>.FailResult("服务不存在或未启用");
            }

            if (!context.Destinations.Where(b => b.ServiceName == info.ServiceName && b.Address == info.Address).Any())
            {
                Destination destination = new Destination()
                {
                    Id = await snowFlake.NextId(),
                    ServiceId = service.Id,
                    ServiceName = service.ServiceName,
                    Address = info.Address,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };

                context.Destinations.Add(destination);
                int result = await context.SaveChangesAsync();

                //更新yarp配置
                if (result > 0)
                {
                    await yarpConfig.UpdateAll(info);
                }

                return ApiResult<bool>.Result(result > 0, true);
            }
            else
            {
                return ApiResult<bool>.SuccessResult(true, "目标已存在");
            }
        }

        public async Task<ApiResult<bool>> Remove(DestinationInfo info)
        {
            var service = await context.Services.Where(b => b.ServiceName == info.ServiceName && b.Enabled == true).FirstOrDefaultAsync();
            if (service == null)
            {
                return ApiResult<bool>.FailResult("服务不存在或未启用");
            }

            var destination = await context.Destinations.Where(b => b.ServiceName == info.ServiceName && b.Address == info.Address).FirstOrDefaultAsync();
            if (destination == null)
            {
                return ApiResult<bool>.FailResult("目标不存在");
            }
            context.Destinations.Remove(destination);
            int result = await context.SaveChangesAsync();

            //更新yarp配置
            if (result > 0)
            {
                await yarpConfig.UpdateAll(info);
            }

            return ApiResult<bool>.Result(result > 0, true);
        }

        public async Task<ApiResult<List<Destination>>> GetDestinations(DestinationQurey qurey)
        {
            List<Destination> destinations = new List<Destination>();
            if (proxyState.TryGetCluster(qurey.ServiceName, out var cluster))
            {
                foreach (DestinationState item in cluster.DestinationsState.AllDestinations)
                {
                    Destination destination = new Destination();
                    destination.Id = long.Parse(item?.Model?.Config?.Metadata?["DestinationId"] ?? "0");
                    destination.ServiceId = long.Parse(item?.Model?.Config?.Metadata?["ServiceId"] ?? "0");
                    destination.ServiceName = item?.Model?.Config?.Metadata?["ServiceName"];
                    destination.Address = item?.Model?.Config?.Address;
                    destination.PassiveHealthState = item?.Health.Passive.ToString();
                    destination.ActiveHealthState = item?.Health.Active.ToString();

                    destinations.Add(destination);
                }
            }

            if (destinations.Count > 0)
            {
                var where = PredicateBuilder.New<Destination>(true);
                if (!string.IsNullOrWhiteSpace(qurey.Address))
                {
                    where.And(x => x.Address.Contains(qurey.Address));
                }
                if (!string.IsNullOrWhiteSpace(qurey.ActiveHealthState))
                {
                    where.And(x => x.ActiveHealthState == qurey.ActiveHealthState);
                }
                if (!string.IsNullOrWhiteSpace(qurey.PassiveHealthState))
                {
                    where.And(x => x.PassiveHealthState == qurey.PassiveHealthState);
                }
                destinations = destinations.AsQueryable().Where(where).OrderByDescending(b => b.Id).ToList();
            }

            return ApiResult<List<Destination>>.SuccessResult(destinations);
        }

        public async Task<ApiResult<List<HealthStateCount>>> GetHealthStateCounts(AuthBaseInfo auth)
        {
            List<HealthStateCount> healthStateCounts = new List<HealthStateCount>();

            var clusters = proxyState.GetClusters();
            foreach (var cluster in clusters)
            {
                HealthStateCount healthStateCount = new HealthStateCount();
                healthStateCount.ServiceName = cluster.ClusterId;
                healthStateCount.Total = cluster.DestinationsState.AllDestinations.Count;
                healthStateCount.ActiveUnknownCount = cluster.DestinationsState.AllDestinations.Count(b => b.Health.Active == DestinationHealth.Unknown);
                healthStateCount.ActiveHealthyCount = cluster.DestinationsState.AllDestinations.Count(b => b.Health.Active == DestinationHealth.Healthy);
                healthStateCount.ActiveUnhealthyCount = cluster.DestinationsState.AllDestinations.Count(b => b.Health.Active == DestinationHealth.Unhealthy);
                healthStateCount.PassiveUnknownCount = cluster.DestinationsState.AllDestinations.Count(b => b.Health.Passive == DestinationHealth.Unknown);
                healthStateCount.PassiveHealthyCount = cluster.DestinationsState.AllDestinations.Count(b => b.Health.Passive == DestinationHealth.Healthy);
                healthStateCount.PassiveUnhealthyCount = cluster.DestinationsState.AllDestinations.Count(b => b.Health.Passive == DestinationHealth.Unhealthy);

                healthStateCounts.Add(healthStateCount);
            }

            return ApiResult<List<HealthStateCount>>.SuccessResult(healthStateCounts);
        }
    }
}
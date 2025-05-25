using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Health;
using Yarp.ReverseProxy.Model;

namespace BlazeGate.Policy
{
    /// <summary>
    /// 在代理请求的第一次不成功的响应时将目标标记为不健康
    /// </summary>
    public class FirstUnsuccessfulResponseHealthPolicy : IPassiveHealthCheckPolicy
    {
        private static readonly TimeSpan _defaultReactivationPeriod = TimeSpan.FromSeconds(60);
        private readonly IDestinationHealthUpdater _healthUpdater;

        public FirstUnsuccessfulResponseHealthPolicy(IDestinationHealthUpdater healthUpdater)
        {
            _healthUpdater = healthUpdater;
        }

        public string Name => "FirstUnsuccessfulResponse";

        public void RequestProxied(HttpContext context, ClusterState cluster, DestinationState destination)
        {
            var error = context.Features.Get<IForwarderErrorFeature>();
            if (error is not null)
            {
                var reactivationPeriod = cluster.Model.Config.HealthCheck?.Passive?.ReactivationPeriod ?? _defaultReactivationPeriod;
                _healthUpdater.SetPassive(cluster, destination, DestinationHealth.Unhealthy, reactivationPeriod);
            }
            else
            {
                var reactivationPeriod = cluster.Model.Config.HealthCheck?.Passive?.ReactivationPeriod ?? _defaultReactivationPeriod;
                _healthUpdater.SetPassive(cluster, destination, DestinationHealth.Healthy, reactivationPeriod);
            }
        }
    }
}
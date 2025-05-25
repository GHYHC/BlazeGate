using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.Policies
{
    /// <summary>
    /// 被动健康检查策略
    /// </summary>
    public class PassiveHealthCheckPolicies
    {
        /// <summary>
        /// 失败率
        /// </summary>
        public static readonly string TransportFailureRate = "TransportFailureRate";
        /// <summary>
        /// 第一次不成功的响应时将目标标记为不健康
        /// </summary>
        public static readonly string FirstUnsuccessfulResponse = "FirstUnsuccessfulResponse";
    }
}

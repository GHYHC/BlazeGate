using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi.Response
{
    public class HealthStateCount
    {
        public string ServiceName { get; set; }

        /// <summary>
        /// 主动未知状态数量
        /// </summary>
        public int ActiveUnknownCount { get; set; }

        /// <summary>
        /// 主动健康状态数量
        /// </summary>
        public int ActiveHealthyCount { get; set; }

        /// <summary>
        /// 主动不健康状态数量
        /// </summary>
        public int ActiveUnhealthyCount { get; set; }

        /// <summary>
        /// 被动未知状态数量
        /// </summary>
        public int PassiveUnknownCount { get; set; }

        /// <summary>
        /// 被动健康状态数量
        /// </summary>
        public int PassiveHealthyCount { get; set; }

        /// <summary>
        /// 被动不健康状态数量
        /// </summary>
        public int PassiveUnhealthyCount { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }
    }
}
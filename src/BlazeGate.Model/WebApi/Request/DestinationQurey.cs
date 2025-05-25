using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi.Request
{
    public class DestinationQurey : AuthBaseInfo
    {
        /// <summary>
        /// 目标地址
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 被动健康状态
        /// </summary>
        public string? PassiveHealthState { get; set; }

        /// <summary>
        /// 主动健康状态
        /// </summary>
        public string? ActiveHealthState { get; set; }
    }
}
using BlazeGate.Model.JsonConverter;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazeGate.Model.EFCore
{
    /// <summary>
    /// 服务配置
    /// </summary>
    [Index(nameof(ServiceId))]
    [Index(nameof(ServiceName))]
    [Table("ServiceConfig")]
    public class ServiceConfig : BaseEntity
    {
        /// <summary>
        /// 服务ID
        /// </summary>
        [JsonConverter(typeof(LongToStringConverter))]
        public long ServiceId { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        [StringLength(64)]
        public string ServiceName { get; set; }

        /// <summary>
        /// 负载均衡策略
        /// </summary>
        [StringLength(64)]
        public string LoadBalancingPolicy { get; set; } = "PowerOfTwoChoices";

        /// <summary>
        /// 授权策略
        /// </summary>
        [StringLength(64)]
        public string? AuthorizationPolicy { get; set; } = "";

        /// <summary>
        /// 限流策略
        /// </summary>
        [StringLength(64)]
        public string? RateLimiterPolicy { get; set; } = "";

        /// <summary>
        /// 请求超时（秒）
        /// </summary>
        public int RequestActivityTimeout { get; set; } = 100;

        /// <summary>
        /// 主动健康检查启用
        /// </summary>
        public bool ActiveHealthCheckEnabled { get; set; } = false;

        /// <summary>
        /// 主动健康检查间隔（秒）
        /// </summary>
        public int ActiveHealthCheckInterval { get; set; } = 10;

        /// <summary>
        /// 主动健康检查超时（秒）
        /// </summary>
        public int ActiveHealthCheckTimeout { get; set; } = 5;

        /// <summary>
        /// 主动健康检查策略
        /// </summary>
        [StringLength(64)]
        public string? ActiveHealthCheckPolicy { get; set; } = "ConsecutiveFailures";

        /// <summary>
        /// 主动健康检查路径
        /// </summary>
        [StringLength(128)]
        public string? ActiveHealthCheckPath { get; set; } = "/api/health";

        /// <summary>
        /// 主动健康检查阈值
        /// </summary>
        public int ActiveHealthCheckThreshold { get; set; } = 2;

        /// <summary>
        /// 主动健康检查,删除不健康目标时间。如果为0就不会被删除（分钟）
        /// </summary>
        public int ActiveRemoveUnhealthyAfter { get; set; } = 30;

        /// <summary>
        /// 被动健康检查启用
        /// </summary>
        public bool PassiveHealthCheckEnabled { get; set; } = false;

        /// <summary>
        /// 被动健康检查策略
        /// </summary>
        [StringLength(64)]
        public string? PassiveHealthCheckPolicy { get; set; } = "FirstUnsuccessfulResponse";

        /// <summary>
        /// 被动健康检查重新激活时间（秒）
        /// </summary>
        public int PassiveHealthCheckReactivationPeriod { get; set; } = 60;

        /// <summary>
        /// 被动健康检查失败率
        /// </summary>
        public float PassiveHealthCheckFailureRate { get; set; } = 0.1f;

        /// <summary>
        /// 被动健康检查,删除不健康目标时间。如果为0就不会被删除（分钟）
        /// </summary>
        public int PassiveRemoveUnhealthyAfter { get; set; } = 30;

        /// <summary>
        /// 关联性会话
        /// </summary>
        public bool SessionAffinityEnabled { get; set; } = false;

        /// <summary>
        /// 关联性会话故障策略
        /// </summary>
        [StringLength(64)]
        public string? SessionAffinityFailurePolicy { get; set; } = "Redistribute";

        /// <summary>
        /// 关联性会话策略
        /// </summary>
        [StringLength(64)]
        public string? SessionAffinityPolicy { get; set; } = "HashCookie";

        /// <summary>
        /// 关联性会话Key
        /// </summary>
        [StringLength(64)]
        public string SessionAffinityKeyName { get; set; } = "SessionAffinity";

        /// <summary>
        /// 关联性会话Cookie的HttpOnly
        /// </summary>
        public bool SessionAffinityCookieHttpOnly { get; set; } = true;

        /// <summary>
        /// 关联性会话Cookie过期时间（秒）
        /// </summary>
        public int SessionAffinityCookieMaxAge { get; set; } = 0;
    }
}
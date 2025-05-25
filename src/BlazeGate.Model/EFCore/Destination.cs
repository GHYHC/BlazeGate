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
    /// 目标
    /// </summary>
    [Index(nameof(ServiceId))]
    [Index(nameof(ServiceName))]
    [Index(nameof(Address))]
    [Index(nameof(Address), nameof(ServiceName), IsUnique = true)]
    [Table("Destination")]
    public class Destination : BaseEntity
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
        /// 目标地址
        /// </summary>

        [StringLength(512)]
        public string Address { get; set; }

        /// <summary>
        /// 被动健康状态
        /// </summary>
        [StringLength(64)]
        public string PassiveHealthState { get; set; } = "Unknown";

        /// <summary>
        /// 被动健康状态更新时间
        /// </summary>
        public DateTime PassiveHealthStateUpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 主动健康状态
        /// </summary>
        [StringLength(64)]
        public string ActiveHealthState { get; set; } = "Unknown";

        /// <summary>
        /// 主动健康状态
        /// </summary>
        public DateTime ActiveHealthStateUpdateTime { get; set; } = DateTime.Now;
    }
}
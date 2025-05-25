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
    /// 认证白名单
    /// </summary>
    [Index(nameof(ServiceId))]
    [Index(nameof(ServiceName))]
    [Table("AuthWhiteList")]
    public class AuthWhiteList : BaseEntity
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
        public string ServiceName { get; set; } = "";

        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(512)]
        public string Address { get; set; } = "";
    }
}
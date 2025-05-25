using BlazeGate.Model.JsonConverter;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazeGate.Model.EFCore
{
    /// <summary>
    /// 服务表
    /// </summary>
    [Index(nameof(ServiceName), IsUnique = true)]
    [Table("Service")]
    [Comment("服务表")]
    public class Service : BaseEntity
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        [StringLength(64)]
        [Comment("服务名称")]
        public string ServiceName { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        [StringLength(256)]
        [Comment("Token")]
        public string Token { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 是否为系统服务
        /// </summary>
        [Comment("是否为系统服务")]
        public bool IsSystem { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        [Comment("启用")]
        public bool Enabled { get; set; }
    }
}
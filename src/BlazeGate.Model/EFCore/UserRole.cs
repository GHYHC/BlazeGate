using BlazeGate.Model.JsonConverter;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazeGate.Model.EFCore
{
    /// <summary>
    /// 用户角色
    /// </summary>
    [PrimaryKey(nameof(RoleId), nameof(UserId), nameof(ServiceId))]
    [Index(nameof(ServiceName))]
    [Table("UserRole")]
    public class UserRole
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonConverter(typeof(LongToStringConverter))]
        public long UserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonConverter(typeof(LongToStringConverter))]
        public long RoleId { get; set; }

        /// <summary>
        /// 服务ID
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonConverter(typeof(LongToStringConverter))]
        public long ServiceId { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        [StringLength(64)]
        public string ServiceName { get; set; }
    }
}
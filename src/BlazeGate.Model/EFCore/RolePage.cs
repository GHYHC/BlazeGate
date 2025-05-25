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
    /// 角色页面
    /// </summary>
    [PrimaryKey(nameof(RoleId), nameof(PageId), nameof(ServiceId))]
    [Index(nameof(ServiceName))]
    [Table("RolePage")]
    public class RolePage
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonConverter(typeof(LongToStringConverter))]
        public long RoleId { get; set; }

        /// <summary>
        /// 页面ID
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonConverter(typeof(LongToStringConverter))]
        public long PageId { get; set; }

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
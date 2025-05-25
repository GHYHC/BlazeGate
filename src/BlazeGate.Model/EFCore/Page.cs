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
    /// 页面
    /// </summary>
    [Index(nameof(ServiceId))]
    [Index(nameof(ServiceName))]
    [Index(nameof(ParentPageId))]
    [Table("Page")]
    public class Page : BaseEntity
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
        /// 父页面ID
        /// </summary>
        public long ParentPageId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int IndexNumber { get; set; }

        /// <summary>
        /// 类型 0:菜单 1:按钮
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(64)]
        public string Title { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [StringLength(64)]
        public string Icon { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        [StringLength(128)]
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// 子路径
        /// </summary>
        [StringLength(2048)]
        public string SubPath { get; set; } = string.Empty;
    }
}
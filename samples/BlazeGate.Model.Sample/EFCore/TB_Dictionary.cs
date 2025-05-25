using BlazeGate.Model.EFCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazeGate.Model.Sample.EFCore
{
    /// <summary>
    /// 字典表
    /// </summary>
    [Index(nameof(Type))]
    [Index(nameof(Key))]
    [Index(nameof(Type), nameof(Key), IsUnique = true)]
    [Comment("字典表")]
    [Table("TB_Dictionary")]
    public class TB_Dictionary : BaseEntity
    {
        /// <summary>
        /// 类型
        /// </summary>
        [StringLength(128)]
        [Comment("类型")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 键
        /// </summary>
        [StringLength(128)]
        [Comment("键")]
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// 值
        /// </summary>
        [StringLength(512)]
        [Comment("值")]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 扩展
        /// </summary>
        [StringLength(512)]
        [Comment("扩展")]
        public string Extended { get; set; } = string.Empty;

        /// <summary>
        /// 扩展2
        /// </summary>
        [StringLength(512)]
        [Comment("扩展2")]
        public string Extended2 { get; set; } = string.Empty;

        /// <summary>
        /// 扩展3
        /// </summary>
        [StringLength(512)]
        [Comment("扩展3")]
        public string Extended3 { get; set; } = string.Empty;

        /// <summary>
        /// 是否启用
        /// </summary>
        [Comment("是否启用")]
        public bool Enabled { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Comment("排序")]
        public int NumberIndex { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.EFCore
{
    /// <summary>
    /// 表示分布式缓存中的一个缓存项
    /// </summary>
    [Table("DistributedCache", Schema = "dbo")]
    public class DistributedCache
    {
        /// <summary>
        /// 缓存项的唯一标识符
        /// </summary>
        [Key]
        [Required]
        [StringLength(449)]
        public string Id { get; set; }

        /// <summary>
        /// 缓存的二进制数据
        /// </summary>
        [Required]
        public byte[] Value { get; set; }

        /// <summary>
        /// 缓存项过期的绝对时间
        /// </summary>
        [Required]
        public DateTimeOffset ExpiresAtTime { get; set; }

        /// <summary>
        /// 滑动过期时间（以秒为单位）
        /// </summary>
        public long? SlidingExpirationInSeconds { get; set; }

        /// <summary>
        /// 绝对过期时间点
        /// </summary>
        public DateTimeOffset? AbsoluteExpiration { get; set; }
    }
}
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
    /// 授权RSA秘密
    /// </summary>
    [Index(nameof(ServiceId))]
    [Index(nameof(ServiceName))]
    [Table("AuthRsaKey")]
    [Comment("授权RSA秘密")]
    public class AuthRsaKey : BaseEntity
    {
        /// <summary>
        /// 服务ID
        /// </summary>
        [JsonConverter(typeof(LongToStringConverter))]
        [Comment("服务ID")]
        public long ServiceId { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        [StringLength(64)]
        [Comment("服务名称")]
        public string ServiceName { get; set; } = "";

        /// <summary>
        /// RSA 公钥
        /// </summary>
        [StringLength(1024)]
        [Comment("RSA 公钥")]
        public string PublicKey { get; set; } = "";

        /// <summary>
        /// RSA 私钥
        /// </summary>
        [StringLength(2048)]
        [Comment("RSA 私钥")]
        public string PrivateKey { get; set; } = "";
    }
}
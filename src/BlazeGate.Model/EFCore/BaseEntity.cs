using BlazeGate.Model.JsonConverter;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazeGate.Model.EFCore
{
    [PrimaryKey(nameof(Id))]
    public class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonConverter(typeof(LongToStringConverter))]
        public long Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Comment("创建时间")]
        [Column(TypeName = "dateTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Comment("更新时间")]
        [Column(TypeName = "dateTime")]
        public DateTime UpdateTime { get; set; }
    }
}

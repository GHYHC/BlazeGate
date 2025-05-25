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
    /// 用户
    /// </summary>
    [Index(nameof(Account), IsUnique = true)]
    [Index(nameof(Phone), IsUnique = true)]
    [Table("User")]
    public class User : BaseEntity
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        [StringLength(28)]
        public string UserName { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [StringLength(28, MinimumLength = 4)]
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [StringLength(64)]
        public string Password { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [StringLength(28)]
        public string Phone { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(512)]
        public string Remark { get; set; }

        /// <summary>
        /// 用户数据
        /// </summary>
        [StringLength(1024)]
        public string UserData { get; set; }
    }
}
using BlazeGate.Model.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi.Request
{
    public class UserSave
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [Display(Name = "用户名称")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [StringLength(28, ErrorMessage = ErrorMessage.StringLength)]
        public string UserName { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [Display(Name = "账号")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [StringLength(28, MinimumLength = 4, ErrorMessage = ErrorMessage.StringLength)]
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "密码")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [StringLength(64, MinimumLength = 6, ErrorMessage = ErrorMessage.StringLength)]
        public string Password { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [Display(Name = "电话")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [RegularExpression(@"^1[0-9]{10}$", ErrorMessage = ErrorMessage.Format)]
        public string Phone { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; } = string.Empty;

        /// <summary>
        /// 用户数据
        /// </summary>
        public string? UserData { get; set; } = string.Empty;

        /// <summary>
        /// 默认密码
        /// </summary>
        public const string DefaultPassword = "●●●●●●●●";
    }
}
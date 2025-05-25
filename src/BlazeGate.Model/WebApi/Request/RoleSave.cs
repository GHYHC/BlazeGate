using BlazeGate.Model.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi.Request
{
    public class RoleSave
    {
        /// <summary>
        /// 服务ID
        /// </summary>
        public long ServiceId { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Display(Name = "角色名称")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [StringLength(64, ErrorMessage = ErrorMessage.StringLength)]
        public string RoleName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; } = string.Empty;

        public string[] PageIds { get; set; } = Array.Empty<string>();
    }
}
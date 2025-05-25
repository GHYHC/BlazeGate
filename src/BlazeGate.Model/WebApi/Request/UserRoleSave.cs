using BlazeGate.Model.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi.Request
{
    public class UserRoleSave
    {
        public bool IsAdd { get; set; }

        [Display(Name = "服务名称")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        public string? ServiceName { get; set; }

        [Display(Name = "用户")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        public long? UserId { get; set; }

        [Display(Name = "角色")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        public IEnumerable<long> RoleIds { get; set; }
    }
}
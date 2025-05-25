using AntDesign;
using System.ComponentModel.DataAnnotations;

namespace BlazeGate.Dashboard.Models
{
    public class LoginParams
    {
        [Required(ErrorMessage = "账号不能为空")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }
    }
}

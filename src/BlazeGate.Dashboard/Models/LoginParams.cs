using AntDesign;
using System.ComponentModel.DataAnnotations;

namespace BlazeGate.Dashboard.Models
{
    public class LoginParams
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi.Request
{
    public class UserRoleQuery
    {
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Account { get; set; }
    }
}
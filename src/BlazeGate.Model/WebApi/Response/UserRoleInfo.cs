using BlazeGate.Model.EFCore;
using BlazeGate.Model.Helper;
using BlazeGate.Model.JwtBearer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi.Response
{
    public class UserRoleInfo
    {
        public UserDto User { get; set; }

        public IEnumerable<Role> Roles { get; set; } = new List<Role>();
    }
}
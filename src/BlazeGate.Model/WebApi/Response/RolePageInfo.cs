using BlazeGate.Model.EFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi.Response
{
    public class RolePageInfo
    {
        public Role Role { get; set; }
        public List<Page> Pages { get; set; }
    }
}

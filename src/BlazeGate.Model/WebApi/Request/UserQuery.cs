using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi.Request
{
    public class UserQuery
    {
        public long? UserId { get; set; }
        public string? NameOrAccount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi.Request
{
    public class LoginParam
    {
        public string Account { get; set; }

        public string Password { get; set; }

        public string ServiceName { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.Policies
{
    public class AuthorizationPolicies
    {
        public static readonly string RBAC = "RBAC";

        public static readonly string defaultPolicies = "default";

        public static readonly string anonymous = "anonymous";
    }
}

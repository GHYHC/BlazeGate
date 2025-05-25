using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.RBAC.Components.Extensions.AuthTokenStorage
{
    public enum AuthTokenStorageEnum
    {
        /// <summary>
        /// Memory
        /// </summary>
        Memory = 0,

        /// <summary>
        /// Cookie
        /// </summary>
        Cookie = 1,

        /// <summary>
        /// LocalStorage
        /// </summary>
        LocalStorage = 2,

        /// <summary>
        /// SessionStorage
        /// </summary>
        SessionStorage = 3,

        /// <summary>
        /// File
        /// </summary>
        File = 4
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi.Response
{
    public class RsaKey
    {
        /// <summary>
        /// RSA公钥
        /// </summary>
        public string PublicKey { get; set; } = string.Empty;

        /// <summary>
        /// RSA私钥
        /// </summary>
        public string PrivateKey { get; set; } = string.Empty;
    }
}
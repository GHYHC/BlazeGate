using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi.Request
{
    /// <summary>
    /// 授权基础信息
    /// </summary>
    public class AuthBaseInfo
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string? ServiceName { get; set; }

        /// <summary>
        /// 服务密钥
        /// </summary>
        public string? Token { get; set; }
    }
}
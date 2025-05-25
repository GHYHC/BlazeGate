using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.AspNetCore
{
    public class BlazeGateOptions
    {
        /// <summary>
        /// BlazeGate地址
        /// </summary>
        public string BlazeGateAddress { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 服务Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 目标地址
        /// </summary>
        public string Address { get; set; }
    }
}
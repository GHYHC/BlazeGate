using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi.Request
{
    public class DestinationInfo : AuthBaseInfo
    {
        /// <summary>
        /// 目标地址
        /// </summary>
        public string Address { get; set; }
    }
}
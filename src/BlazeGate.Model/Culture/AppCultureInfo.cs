using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.Culture
{
    /// <summary>
    /// 应用程序支持的文化信息
    /// </summary>
    public class AppCultureInfo
    {
        /// <summary>
        /// 应用程序支持的文化信息
        /// </summary>
        public string Culture { get; set; }

        /// <summary>
        /// 应用程序支持的UI文化信息
        /// </summary>
        public string UICulture { get; set; }
    }
}
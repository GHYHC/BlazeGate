using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.Helper
{
    public class ErrorMessage
    {
        public const string Required = "'{0}' 不能为空";
        public const string StringLength = "'{0}' 必须是最小长度为{2}、最大长度为{1}";
        public const string Format = "'{0}'格式不正确";
    }
}

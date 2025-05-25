using AntDesign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.RBAC.Components.Models
{
    public class ValidateMessages
    {
        public static FormValidateErrorMessages Messages { get; set; } =
        new FormValidateErrorMessages
        {
            Default = "'{0}' 验证失败",
            //OneOf = "'{0}' 必须是其中之一: {1}",
            Required = "'{0}' 是必选字段",
            Whitespace = "'{0}' 不能是空格",
            String = new FormValidateErrorMessages.CompareMessage
            {
                Len = "'{0}' 必须是{1}个字符",
                Max = "'{0}' 不能超过{1}个字符",
                Min = "'{0}' 不能少于{1}个字符",
                Range = "'{0}' 必须在{1}到{2}个字符之间"
            },
            Number = new FormValidateErrorMessages.CompareMessage
            {
                Len = "'{0}' 必须是{1}",
                Max = "'{0}' 不能大于{1}",
                Min = "'{0}' 不能小于{1}",
                Range = "'{0}' 必须在{1}到{2}之间"
            },
            Array = new FormValidateErrorMessages.CompareMessage
            {
                Len = "'{0}' 必须是{1}个",
                Max = "'{0}' 不能超过{1}个",
                Min = "'{0}' 不能少于{1}个",
                Range = "'{0}' 必须在{1}到{2}个之间"
            },
            Pattern = new FormValidateErrorMessages.PatternMessage
            {
                Mismatch = "'{0}' 格式不正确"
            }
        };
    }
}
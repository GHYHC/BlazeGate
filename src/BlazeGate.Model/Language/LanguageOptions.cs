using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.Culture
{
    public class LanguageOptions
    {
        /// <summary>
        /// 语言列表
        /// </summary>
        public static readonly string[] Languages = new string[]
        {
            "en-US",
            "zh-CN",
            "vi-VN",
        };

        /// <summary>
        /// 语言标签
        /// </summary>
        public static readonly IDictionary<string, string> LanguageLabels = new Dictionary<string, string>
        {
            ["en-US"] = "English",
            ["zh-CN"] = "简体中文",
            ["vi-VN"] = "Tiếng Việt"
        };

        /// <summary>
        /// 语言图标
        /// </summary>
        public static readonly IDictionary<string, string> LanguageIcons = new Dictionary<string, string>
        {
            ["en-US"] = "🇺🇸",
            ["zh-CN"] = "🇨🇳",
            ["vi-VN"] = "🇻🇳"
        };
    }
}
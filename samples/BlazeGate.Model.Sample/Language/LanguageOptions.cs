﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.Sample.Language
{
    public class LanguageOptions
    {
        /// <summary>
        /// 语言列表
        /// </summary>
        public static readonly string[] Languages = new string[]
        {
            "en-US",
            "zh-CN"
        };

        /// <summary>
        /// 语言标签
        /// </summary>
        public static readonly IDictionary<string, string> LanguageLabels = new Dictionary<string, string>
        {
            ["en-US"] = "English",
            ["zh-CN"] = "简体中文"
        };

        /// <summary>
        /// 语言图标
        /// </summary>
        public static readonly IDictionary<string, string> LanguageIcons = new Dictionary<string, string>
        {
            ["en-US"] = "🇺🇸",
            ["zh-CN"] = "🇨🇳"
        };
    }
}
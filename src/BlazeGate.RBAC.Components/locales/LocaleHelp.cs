using AntDesign;
using AntDesign.Locales;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace BlazeGate.RBAC.locales
{
    public partial class LocaleHelp
    {
        [GeneratedRegex(@"^.*locales\.(.+)\.json")]
        private static partial Regex LocaleJsonRegex();

        private static readonly JsonSerializerOptions _localeJsonOpt = InitLocaleJsonOpt();

        private static JsonSerializerOptions InitLocaleJsonOpt()
        {
            var opt = new JsonSerializerOptions()
            {
                TypeInfoResolver = LocaleSourceGenerationContext.Default,
                PropertyNameCaseInsensitive = true,
            };
            opt.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            return opt;
        }

        public static Dictionary<string, string> GetAvailableResources(Assembly resourcesAssembly)
        {
            var regex = LocaleJsonRegex();
            var availableResources = resourcesAssembly
                .GetManifestResourceNames()
                .Select(x => regex.Match(x))
                .Where(x => x.Success)
                .ToDictionary(x => x.Groups[1].Value, x => x.Value);

            foreach (var resource in availableResources.ToArray())
            {
                var parentCultureName = GetParentCultureName(resource.Key);
                while (parentCultureName != string.Empty)
                {
                    availableResources.TryAdd(parentCultureName, resource.Value);
                    parentCultureName = GetParentCultureName(parentCultureName);
                }
            }

            return availableResources;
        }

        /// <summary>
        /// 覆盖AntDesign的Locale根据资源程序集
        /// </summary>
        /// <param name="resourcesAssembly"></param>
        public static void OverrideAntDesignLocaleByAssembly(Assembly resourcesAssembly)
        {
            var locales = GetLocaleByAssembly(resourcesAssembly);
            OverrideAntDesignLocale(locales);
        }

        /// <summary>
        /// 覆盖AntDesign的Locale
        /// </summary>
        /// <param name="locales"></param>
        public static void OverrideAntDesignLocale(List<Locale> locales)
        {
            if (locales == null)
            {
                return;
            }

            //通过反射调用LocaleProvider._localeCache的AddOrUpdate把locales添加到缓存中
            var localeProviderType = typeof(LocaleProvider);
            var localeCacheField = localeProviderType.GetField("_localeCache", BindingFlags.Static | BindingFlags.NonPublic);
            if (localeCacheField != null)
            {
                var localeCache = (ConcurrentDictionary<string, Locale>)localeCacheField.GetValue(null);
                if (localeCache != null)
                {
                    foreach (var locale in locales)
                    {
                        localeCache.AddOrUpdate(locale.LocaleName, locale, (name, original) => locale);
                    }
                }
            }
        }

        /// <summary>
        /// 获取资源程序集中的Locale列表
        /// </summary>
        /// <param name="resourcesAssembly"></param>
        /// <returns></returns>
        public static List<Locale> GetLocaleByAssembly(Assembly resourcesAssembly)
        {
            List<Locale> locales = new List<Locale>();
            var availableResources = GetAvailableResources(resourcesAssembly);
            foreach (var availableResource in availableResources)
            {
                using var fileStream = resourcesAssembly.GetManifestResourceStream(availableResource.Value);
                if (fileStream == null)
                {
                    continue;
                }
                using var streamReader = new StreamReader(fileStream);
                var content = streamReader.ReadToEnd();

                var result = JsonSerializer.Deserialize<Locale>(content, _localeJsonOpt);
                if (result == null)
                {
                    continue;
                }

                //通过反射调用Locale的SetCultureInfo方法 result.SetCultureInfo(key);
                var setCultureInfoMethod = typeof(Locale).GetMethod("SetCultureInfo", BindingFlags.Instance | BindingFlags.NonPublic);
                if (setCultureInfoMethod != null)
                {
                    setCultureInfoMethod.Invoke(result, new object[] { availableResource.Key });
                }

                locales.Add(result);
            }
            return locales;
        }

        private static string GetParentCultureName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;
            try
            {
                return CultureInfo.GetCultureInfo(name).Parent.Name;
            }
            catch (Exception)
            {
                return name.Contains('-') ? name[0..name.LastIndexOf('-')] : string.Empty;
            }
        }
    }

    [JsonSerializable(typeof(AntDesign.Locales.Locale))]
    internal partial class LocaleSourceGenerationContext : JsonSerializerContext
    {
    }
}
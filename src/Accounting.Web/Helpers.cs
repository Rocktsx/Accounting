using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Accounting.Web
{
    public static class Helpers
    {
        public static Dictionary<string, string> Select2Languages { get; set; } =
        new Dictionary<string, string>
        {
            { "en", "en" },
            { "zh-Hans", "zh-CN" },
            { "zh-Hant", "zh-TW" }
        };
        public static string GetSelect2LanguageName()
        {
            var lang = CultureInfo.CurrentCulture.Name;
            return Select2Languages.ContainsKey(lang) ? Select2Languages[lang] : "en";
        }
        public static string GetText(string code, string name, string otherName)
        {
            return $"{code} - {name}" + (string.IsNullOrWhiteSpace(otherName) ? "" : $" ({otherName})");
        }
        public static List<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, Func<T, string> getValue, Func<T, string> getText, bool insertEmpty = true)
        {
            var list = items.Select(item => new SelectListItem
            {
                Value = getValue(item),
                Text = getText(item),
            }).ToList(); 
             
            if (insertEmpty)
            {
                list.Insert(0, new SelectListItem { Value = null, Text = "--" });
            }
            return list;
        }

        public static List<SelectListItem> GetEnumSelectList(Type type, IStringLocalizer localizer)
        {

            var items = Enum.GetValues(type);
            var list = new List<SelectListItem>(items.Length);
            foreach (int item in items)
            {
                list.Add(new SelectListItem
                {
                    Value = item.ToString(),
                    Text = localizer[Enum.GetName(type, item)].Value
                });
            }
            return list;
        }

    }
}

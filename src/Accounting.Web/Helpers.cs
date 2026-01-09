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
        public const string EnLanguage = "en";
        public static Dictionary<string, string> Select2Languages { get; set; } =
        new Dictionary<string, string>
        {
            { EnLanguage, EnLanguage },
            { "zh-Hans", "zh-CN" },
            { "zh-Hant", "zh-TW" }
        };
        public const string EmptyText = "--";
        public static string GetSelect2LanguageName()
        {
            var lang = CultureInfo.CurrentCulture.Name;
            return Select2Languages.ContainsKey(lang) ? Select2Languages[lang] : EnLanguage;
        }
        public static string GetText(string code, string name, string otherName)
        {
            return $"{code} - {name}" + (string.IsNullOrWhiteSpace(otherName) ? string.Empty : $" ({otherName})");
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
                list.Insert(0, new SelectListItem { Value = string.Empty, Text = EmptyText });
            }
            return list;
        }

        public static List<SelectListItem> GetEnumSelectList(Type type, IStringLocalizer localizer, bool insertEmpty = false)
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

            if (insertEmpty)
            {
                list.Insert(0, new SelectListItem { Value = string.Empty, Text = EmptyText });
            }

            return list;
        }

    }
}

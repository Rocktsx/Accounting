using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Accounting.Web
{
    public static class Helpers
    {
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
    }
}

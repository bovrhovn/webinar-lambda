using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lambada.Generators.Helpers
{
    public static class SelectListHelper
    {
        public static List<SelectListItem> GetSelectedItems<T>(this List<T> list, Func<T, string> text,
            Func<T, string> value, Func<T, bool> selected)
        {
            var currentList = new List<SelectListItem>();
            foreach (var currentItem in list)
            {
                var selectListItem = new SelectListItem
                {
                    Text = text.Invoke(currentItem),
                    Value = value.Invoke(currentItem),
                    Selected = selected.Invoke(currentItem)
                };
                currentList.Add(selectListItem);
            }

            return currentList;
        }

        public static List<SelectListItem> GetListItems<T>(this List<T> list, Func<T, string> text,
            Func<T, string> value)
        {
            var currentList = new List<SelectListItem>();
            foreach (var currentItem in list)
            {
                var selectListItem = new SelectListItem
                {
                    Text = text.Invoke(currentItem),
                    Value = value.Invoke(currentItem)
                };
                currentList.Add(selectListItem);
            }

            return currentList;
        }
    }
}
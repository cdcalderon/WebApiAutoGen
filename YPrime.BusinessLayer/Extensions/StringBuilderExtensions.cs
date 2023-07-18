using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.BusinessLayer.Extensions
{
    public static class StringBuilderExtensions
    {
        public static void CompleteTemplate(this StringBuilder template, IDictionary<string, string> data)
        {
            string YYYY = DateTime.Now.Year.ToString();
            template.Replace("<=YYYY=>", YYYY);

            foreach (var key in data.Keys)
            {
                if (key == "ExpirationDate")
                {
                    template.Replace($"<={key}=>", Convert.ToDateTime(data[key]).ToString("MM/dd/yyyy") ?? "n/a");
                }
                else
                {
                    template.Replace($"<={key}=>", data[key] ?? "n/a");
                }
            }
        }
    }
}
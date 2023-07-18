using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace YPrime.Web.E2E.Utilities
{
    public class TableUtilities
    {
        public static Dictionary<string,string> ToDictionary(Table table)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach(var row in table.Rows)
            {
                dictionary.Add(row[0], row[1]);
            }
            return dictionary;
        }
    }
}

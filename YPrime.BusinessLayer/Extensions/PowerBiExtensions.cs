using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.BusinessLayer.Models;

namespace YPrime.BusinessLayer.Extensions
{
    public static class PowerBiExtensions
    {
        public static IEnumerable<ExternalReport> FilterByExcludedPrefix(this IEnumerable<ExternalReport> analytics, string excludedPrefix)
        {
            if (!string.IsNullOrEmpty(excludedPrefix))
            {
                analytics = analytics
                    .Where(analytic => !analytic.Report.Name.StartsWith(excludedPrefix, StringComparison.OrdinalIgnoreCase));
            }
            
            return analytics;
        }

        public static IEnumerable<ExternalReport> FilterByExcludedTerms(this IEnumerable<ExternalReport> analytics, IEnumerable<string> excludedTerms)
        {
            if (excludedTerms != null && excludedTerms.Any())
            {
                analytics = analytics
                    .Where(analytic => !excludedTerms
                        .Any(term => analytic.Report.Name.Contains(term, StringComparison.OrdinalIgnoreCase)));
            }

            return analytics;
        }
    }
}

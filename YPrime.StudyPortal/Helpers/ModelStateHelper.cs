using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace YPrime.StudyPortal.Helpers
{
    public static class ModelStateHelper
    {
        public static Dictionary<string, string> GetModelErrors(this ModelStateDictionary errorDictionary)
        {
            var errors = new Dictionary<string, string>();

            errorDictionary.Where(k => k.Value.Errors.Count > 0)
                .ToList()
                .ForEach(i =>
                {
                    var error = string.Join(", ", i.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                    errors.Add(i.Key, error);
                });
            return errors;
        }
    }
}
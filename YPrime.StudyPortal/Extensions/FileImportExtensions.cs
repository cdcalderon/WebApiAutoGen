using System.Web;
using YPrime.Shared.Helpers.Data;

namespace YPrime.StudyPortal.Extensions
{
    public static class FileImportExtensions
    {
        private const string StringPropertyName = "string";

        public static void Sanitize<T>(this FileImport<T> import)
        {
            foreach (var importedObject in import.ImportedObjects)
            {
                var properties = importedObject.Entity.GetType().GetProperties();

                foreach (var property in properties)
                {
                    if (property.PropertyType.Name.ToLower() == StringPropertyName && property.CanWrite)
                    {
                        var oldValue = property.GetValue(importedObject.Entity)?.ToString();
                        var newValue = HttpUtility.HtmlEncode(oldValue);
                        property.SetValue(importedObject.Entity, newValue);
                    }
                }
            }
        }
    }
}
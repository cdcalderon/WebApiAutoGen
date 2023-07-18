using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IKeyVaultBasedContext
    {
        IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
    }
}

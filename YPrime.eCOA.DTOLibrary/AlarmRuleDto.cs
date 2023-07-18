using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YPrime.eCOA.DTOLibrary
{
    public class AlarmRuleDto
    {
        [Required]
        public Guid AlarmId { get; set; }

        [Required]
        public Guid DeviceId { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            if (AlarmId == Guid.Empty)
            {
                errors.Add(new ValidationResult($"{nameof(AlarmId)} value can not be null or empty"));
            }

            if (PatientId == Guid.Empty)
            {
                errors.Add(new ValidationResult($"{nameof(AlarmId)} value can not be null or empty"));
            }

            if (DeviceId == Guid.Empty)
            {
                errors.Add(new ValidationResult($"{nameof(DeviceId)} value can not be null or empty"));
            }

            return errors;
        }
    }
}

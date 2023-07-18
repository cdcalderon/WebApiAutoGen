using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Core.BusinessLayer.Models
{
    [Serializable]
    public class PatientStatusModel : IConfigModel
    {
         public int Id { get; set; }
         public string Name { get; set; }
         public bool IsActive { get; set; }
         public bool IsRemoved { get; set; }
    }
}

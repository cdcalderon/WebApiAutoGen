using System;
using System.Collections.Generic;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class ReferenceMaterialTypeDto : DtoBase
    {
        public ReferenceMaterialTypeDto()
        {
            ReferenceMaterials = new HashSet<ReferenceMaterialDto>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<ReferenceMaterialDto> ReferenceMaterials { get; set; }
    }
}
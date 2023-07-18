using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IReferenceMaterialRepository
    {
        ReferenceMaterialDto GetReferenceMaterial(Guid Id);

        IOrderedQueryable<ReferenceMaterialDto> GetReferenceMaterials();

        IList<ReferenceMaterialTypeDto> GetReferenceMaterialTypes();

        IList<ReferenceMaterialTypeDto> GetReferenceMaterialTypeWithMaterials();

        bool ReferenceMaterialNameExists(string name);

        bool ReferenceMaterialFileNameExists(string fileName);

        string AllowedFileExtensions(string Delim = ",");

        bool CheckUploadedFileNameExtension(string fileName);

        bool CheckUploadedFileMimeContents(HttpPostedFileBase file);

        void DeleteReferenceMaterial(Guid Id);

        void CreateReferenceMaterial(ReferenceMaterialDto referenceMaterialDto);
    }
}
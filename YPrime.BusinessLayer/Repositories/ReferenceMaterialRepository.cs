using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Config.Enums;
using System.Data.Entity.Infrastructure;
using YPrime.BusinessLayer.Extensions;
using YPrime.BusinessLayer.Session;

namespace YPrime.BusinessLayer.Repositories
{
    public class ReferenceMaterialRepository : BaseRepository, IReferenceMaterialRepository
    {
        private readonly Dictionary<string, string> AcceptedFileTypes = new Dictionary<string, string>
        {
            {".pdf", "application/pdf"},
            {".mp4", "video/mp4;application/octet-stream"}
        };

        /*
              {".docx","application/x-zip-compressed" },
              {".xlsx","application/x-zip-compressed" },
              {".pptx","application/x-zip-compressed" },
              {".txt", "application/octet-stream" },
              {".csv", "text/plain" },
              {".jpg", "image/pjpeg" },
              {".png", "image/x-png" },
              {".mov" , "video/quicktime" },
              {".wmv" , "video/x-ms-wmv"  }
         
         */

        public ReferenceMaterialRepository(IStudyDbContext db) : base(db)
        {
        }

        public void CreateReferenceMaterial(ReferenceMaterialDto referenceMaterialDto)
        {
            var referenceMaterialEntity = new ReferenceMaterial
            {
                //Id = Guid.NewGuid(), => This uses a default constraint
                StudyUserId = referenceMaterialDto.UserId,
                ReferenceMaterialTypeId = referenceMaterialDto.ReferenceMaterialTypeId,
                Name = referenceMaterialDto.Name,
                FileName = referenceMaterialDto.File.FileName,
                ContentType = referenceMaterialDto.File.ContentType,
                CreatedTime = DateTimeOffset.Now
            };
            _db.ReferenceMaterials.Add(referenceMaterialEntity);
            _db.SaveChanges(referenceMaterialDto.UserId.ToString());
            referenceMaterialDto.Id = referenceMaterialEntity.Id;
        }

        public ReferenceMaterialDto GetReferenceMaterial(Guid Id)
        {
            var referenceMaterial =  _db.ReferenceMaterials.Single(rm => rm.Id == Id);
            var referenceMaterialDto = new ReferenceMaterialDto
            {
                Id = referenceMaterial.Id,
                UserId = referenceMaterial.StudyUserId,
                Name = referenceMaterial.Name,
                ReferenceMaterialTypeId = referenceMaterial.ReferenceMaterialTypeId,
                ReferenceMaterialType = referenceMaterial.GetReferenceMaterialType().Name,
                FileName = referenceMaterial.FileName,
                ContentType = referenceMaterial.ContentType,
                CreatedTime = referenceMaterial.CreatedTime,
                UpdatedTime = referenceMaterial.UpdatedTime
            };
            return referenceMaterialDto;
        }

        public string AllowedFileExtensions(string delim = ",")
        {
            var Extensions = AcceptedFileTypes.Select(x => x.Key).ToList();
            return string.Join(delim, Extensions);
        }

        public bool CheckUploadedFileNameExtension(string fileName)
        {
            var CurrentFileExtension = Path.GetExtension(fileName).ToLower();
            return AcceptedFileTypes.ContainsKey(CurrentFileExtension);
        }

        public bool CheckUploadedFileMimeContents(HttpPostedFileBase fileName)
        {
            bool Rtn = false;
            var CurrentFileExtension = Path.GetExtension(fileName.FileName).ToLower();
            if (AcceptedFileTypes.ContainsKey(CurrentFileExtension))
            {
                var ActualMimeType = fileName.ContentType.ToLower();
                string ExpectedMimeTypes = AcceptedFileTypes[CurrentFileExtension];
                Rtn = ExpectedMimeTypes.Contains(ActualMimeType);
            }

            return Rtn;
        }

        public IOrderedQueryable<ReferenceMaterialDto> GetReferenceMaterials()
        {
            var referenceMaterialDtos = new List<ReferenceMaterialDto>();
            var referenceMaterials = _db.ReferenceMaterials.ToList();
            foreach (var r in referenceMaterials)
            {
                referenceMaterialDtos.Add(new ReferenceMaterialDto
                {
                    Id = r.Id,
                    UserId = r.StudyUserId,
                    Username = r.StudyUser.UserName,
                    Name = r.Name,
                    ReferenceMaterialTypeId = r.ReferenceMaterialTypeId,
                    ReferenceMaterialType = r.GetReferenceMaterialType().Name,
                    FileName = r.FileName,
                    ContentType = r.ContentType,
                    CreatedTime = r.CreatedTime,
                    UpdatedTime = r.UpdatedTime
                });
            }
            return referenceMaterialDtos.AsQueryable().OrderBy(r => r.Name);
        }

        public IList<ReferenceMaterialTypeDto> GetReferenceMaterialTypes()
        {
            return ReferenceMaterialType.GetAll<ReferenceMaterialType>().Select(r => new ReferenceMaterialTypeDto
            {
                Id = r.Id,
                Name = r.Name
            }).OrderBy(q => q.Name).ToList();
        }

        public IList<ReferenceMaterialTypeDto> GetReferenceMaterialTypeWithMaterials()
        {
            var referenceMaterialTypeDtos = new List<ReferenceMaterialTypeDto>();
            var referenceMaterialTypes = ReferenceMaterialType.GetAll<ReferenceMaterialType>();
            var referenceMaterials = _db.ReferenceMaterials.Include(r => r.StudyUser).ToList();

            foreach (var r in referenceMaterialTypes)
            {
                referenceMaterialTypeDtos.Add(new ReferenceMaterialTypeDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    ReferenceMaterials = referenceMaterials
                        .Where(rm => rm.ReferenceMaterialTypeId == r.Id)        
                        .Select(rm => new ReferenceMaterialDto
                        {
                            Id = rm.Id,
                            UserId = rm.StudyUserId,
                            ReferenceMaterialTypeId = rm.ReferenceMaterialTypeId,
                            Name = rm.Name,
                            FileName = rm.FileName,
                            ContentType = rm.ContentType,
                            CreatedTime = rm.CreatedTime,
                            UpdatedTime = rm.UpdatedTime,
                            Username = rm.StudyUser.UserName
                        }).OrderBy(rm => rm.Name).ToList()
                });
            }

            return referenceMaterialTypeDtos;
        }

        public bool ReferenceMaterialNameExists(string name)
        {
            return _db.ReferenceMaterials.Any(r => r.Name.ToLower() == name.ToLower());
        }

        public bool ReferenceMaterialFileNameExists(string fileName)
        {
            return _db.ReferenceMaterials.Any(r => r.FileName.ToLower() == fileName.ToLower());
        }

        public void DeleteReferenceMaterial(Guid Id)
        {
            var refMaterial = _db.ReferenceMaterials.SingleOrDefault(r => r.Id == Id);
            _db.ReferenceMaterials.Remove(refMaterial);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
        }
    }
}
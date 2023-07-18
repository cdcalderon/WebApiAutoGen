using System;
using System.Collections.Generic;

namespace YPrime.Core.BusinessLayer.Models
{
    public class ApproverGroupModel : IConfigModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<StudyRoleModel> Roles { get; set; } = new List<StudyRoleModel>();
    }
}

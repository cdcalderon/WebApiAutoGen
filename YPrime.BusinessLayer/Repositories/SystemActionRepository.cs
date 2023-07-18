using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.BusinessLayer.Interfaces;

namespace YPrime.BusinessLayer.Repositories
{
    public class SystemActionRepository : BaseRepository, ISystemActionRepository
    {
        public SystemActionRepository(IStudyDbContext db) : base(db)
        {
        }

        public List<SystemActionDto> GetAllSystemActions()
        {
            var result = _db.SystemActions
                .Select(d => new SystemActionDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    IsBlinded = d.IsBlinded,
                    ActionLocation = d.ActionLocation
                }).ToList();

            return result;
        }

        public void SaveActionList(Dictionary<string, dynamic> ActionList)
        {
            // Could probably not hit the DB as much but this isn't a function that's going to run a whole lot.
            foreach (var Action in ActionList)
            {
                var DBAction = _db.SystemActions.SingleOrDefault(a => a.Name == Action.Key);
                if (DBAction == null)
                {
                    DBAction = new SystemAction();
                    DBAction.Id = Guid.NewGuid();
                    _db.SystemActions.Add(DBAction);
                }

                DBAction.Name = Action.Key;
                DBAction.Description = Action.Value.Description;
                DBAction.IsBlinded = Action.Value.IsBlinded;
                DBAction.ActionLocation = Action.Value.ActionLocation;
            }

            _db.SaveChanges(null);
        }
    }
}
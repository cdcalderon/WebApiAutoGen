using System;
using System.Collections.Generic;
using System.Text;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface IVisitService : IConfigServiceBase<VisitModel, Guid>
    { }
}
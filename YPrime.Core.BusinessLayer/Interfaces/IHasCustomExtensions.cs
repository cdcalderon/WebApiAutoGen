using System.Collections.Generic;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface IHasCustomExtensions
    {
        List<CustomExtensionModel> CustomExtensions { get; set; }
    }
}

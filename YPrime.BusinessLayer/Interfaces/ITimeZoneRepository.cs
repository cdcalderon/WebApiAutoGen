using System.Threading.Tasks;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface ITimeZoneRepository
    {
        Task<string> GetTimeZoneId(string ipAddress);
        Task<string> GetTimeZoneIdWithDefault(string ipAddress, string defaultTimeZone);
    }
}
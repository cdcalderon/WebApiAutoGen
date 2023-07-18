using System.Threading.Tasks;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IKeyVaultBasedContextFactory
    {
        Task<T> GetCurrentContext<T>() where T : class, IKeyVaultBasedContext;
    }
}
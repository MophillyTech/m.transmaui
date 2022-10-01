using System.Threading.Tasks;
using m.transport.Dto;

namespace m.transport.Cache
{
    public interface ICache
    {
        Task<CacheUserCredentialsDto> GetLastLoggedInUser();
        Task SaveLoggedUser(CacheUserCredentialsDto userCredentials);
    }
}
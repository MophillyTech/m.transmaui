using System.Reactive.Linq;
using System.Threading.Tasks;
using m.transport.Dto;
using Akavache;

namespace m.transport.Cache
{
    public class GlobalCache : ICache
    {
        private const string UserCacheKey = "UserCacheKey";

        public async Task<CacheUserCredentialsDto> GetLastLoggedInUser()
        {
            return await BlobCache.LocalMachine.GetObject<CacheUserCredentialsDto>(UserCacheKey)
                .Catch(Observable.Return(default(CacheUserCredentialsDto)));
        }

        public async Task SaveLoggedUser(CacheUserCredentialsDto userCredentials)
        {
            var existingUserCredentials = await GetLastLoggedInUser();

            if (existingUserCredentials != null)
                await BlobCache.LocalMachine.InvalidateObject<CacheUserCredentialsDto>(UserCacheKey);

            await BlobCache.LocalMachine.InsertObject(UserCacheKey, userCredentials);
        }
    }
}
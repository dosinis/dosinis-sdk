using System.Threading.Tasks;

namespace DosinisSDK.Core
{
    public interface IAsyncModule : IModule
    {
        /// <summary>
        /// Happens after IModule Init() and is awaitable when app is initializing
        /// </summary>
        /// <returns></returns>
        Task InitAsync(IApp app);
    }
}

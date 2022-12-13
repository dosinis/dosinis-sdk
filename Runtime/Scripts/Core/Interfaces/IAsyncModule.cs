using System.Threading.Tasks;

namespace DosinisSDK.Core
{
    public interface IAsyncModule : IModule
    {
        /// <summary>
        /// Happens after IModule Init()
        /// </summary>
        /// <returns></returns>
        Task InitAsync(IApp app);
    }
}

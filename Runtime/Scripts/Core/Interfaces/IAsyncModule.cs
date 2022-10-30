using System.Threading.Tasks;

namespace DosinisSDK.Core
{
    public interface IAsyncModule
    {
        Task InitAsync(IApp app, ModuleConfig moduleConfig);
    }
}

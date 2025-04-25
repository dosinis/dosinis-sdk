using System.Threading.Tasks;

namespace DosinisSDK.LogConsole
{
    public interface ILogConsole
    {
        Task PostDataAsync(string log, string description);
    }
}
using DosinisSDK.Core;
using DosinisSDK.Utils;

namespace DosinisSDK.Connectivity
{
    public interface IConnectivityManager : IModule
    {
        IObservable<bool> Connected { get;}
        void RefreshConnectionStatus();
    }
}
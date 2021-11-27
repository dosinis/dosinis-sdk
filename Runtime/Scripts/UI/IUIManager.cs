using DosinisSDK.Core;

namespace DosinisSDK.UI
{
    public interface IUIManager : IManaged 
    {
        T GetWindow<T>() where T : Window;
    }
}



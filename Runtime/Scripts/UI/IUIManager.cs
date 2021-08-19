using DosinisSDK.Core;

namespace DosinisSDK.UI
{
    public interface IUIManager : IBehaviourModule 
    {
        T GetWindow<T>() where T : Window;
    }
}



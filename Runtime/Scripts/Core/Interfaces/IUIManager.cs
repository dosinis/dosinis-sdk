using DosinisSDK.UI;

namespace DosinisSDK.Core
{
    public interface IUIManager : IBehaviourModule 
    {
        T GetWindow<T>() where T : Window;
    }
}



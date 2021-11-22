using DosinisSDK.Core;

namespace DosinisSDK.UI
{
    public interface IUIManager : IManaged 
    {
        ISceneManager SceneManager { get; }
        T GetWindow<T>() where T : Window;
    }
}



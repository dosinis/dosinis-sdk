using DosinisSDK.Core;

namespace DosinisSDK.Game
{
    public interface IGame : IBehaviourModule
    {
        void CreateGameElement();
        void ReturnElementToPool(GameElement element);
    }
}



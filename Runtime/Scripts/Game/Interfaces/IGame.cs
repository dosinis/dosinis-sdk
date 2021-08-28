using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Game
{
    public interface IGame : IBehaviourModule
    {
        void CreateGameElement(GameObject source);
        void DestroyGameElement(GameElement element);
    }
}



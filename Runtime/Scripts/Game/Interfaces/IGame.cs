using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Game
{
    public interface IGame : IBehaviourModule
    {
        GameElement CreateGameElement(GameObject source, Vector3 position);
        GameElement CreateGameElement(GameElement gameElement, Vector3 position);
        void DestroyGameElement(GameElement element);
    }
}



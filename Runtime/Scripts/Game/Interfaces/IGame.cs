using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Game
{
    public interface IGame : IBehaviourModule
    {
        void CreateGameElement(GameObject source, Vector3 position);
        void DestroyGameElement(GameElement element);
    }
}



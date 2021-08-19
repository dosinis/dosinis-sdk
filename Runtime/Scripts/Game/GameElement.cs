using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Game
{
    public class GameElement : MonoBehaviour
    {
        private IGame game;

        public bool Alive { get; private set; }

        public void Init(IGame game)
        {
            this.game = game;
            Alive = true;
        }

        public virtual void Process(float delta)
        {
            
        }

        public virtual void Destruct()
        {
            Alive = false;
            game.ReturnElementToPool(this);
        }
    }
}

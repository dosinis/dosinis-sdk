using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.Game
{
    public class Stage : MonoBehaviour
    {
        [SerializeField] protected StageConfig mainConfig;

        public IStagedGame stagedGame { get; private set; }
        public int Index { get; private set; }

        public void Init()
        {

        }
    }
}

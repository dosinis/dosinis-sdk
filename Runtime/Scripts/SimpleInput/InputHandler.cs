using UnityEngine;

namespace DosinisSDK.SimpleInput
{
    public abstract class InputHandler : MonoBehaviour
    {
        public Vector2 Direction { get; protected set; }
    }
}
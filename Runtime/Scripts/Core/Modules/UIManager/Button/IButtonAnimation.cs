using UnityEngine;

namespace DosinisSDK.Core
{
    public interface IButtonAnimation
    {
        void Init();
        void PressAnimation();
        void ReleaseAnimation();
        void OnInteractableStateChanged(bool value);
        void Highlight(bool value);
        void SetStartScale(Vector3 scale);
    }
}
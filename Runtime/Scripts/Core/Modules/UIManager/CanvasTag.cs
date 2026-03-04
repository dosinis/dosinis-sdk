using UnityEngine;

namespace DosinisSDK.Core
{
    public class CanvasTag : MonoBehaviour
    {
        [SerializeField] private CanvasType type;
        [SerializeField] private Canvas canvas;

        public CanvasType Type => type;
        public Canvas Canvas => canvas;
        
        private void Reset()
        {
            canvas = GetComponent<Canvas>();
        }
    }
}

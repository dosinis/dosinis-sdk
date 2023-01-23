using TMPro;
using UnityEngine;

namespace DosinisSDK.Utils
{
    [RequireComponent(typeof(TMP_Text))]
    public class FpsWidget : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        private const float HUD_REFRESH_RATE = 1;
        private float timer;

#if UNITY_EDITOR
        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }
#endif

        private void Update()
        {
            if (Time.unscaledTime > timer)
            {
                int fps = (int)(1f / Time.unscaledDeltaTime);
                text.text = "FPS: " + fps;
                timer = Time.unscaledTime + HUD_REFRESH_RATE;
            }
        }
    }
}
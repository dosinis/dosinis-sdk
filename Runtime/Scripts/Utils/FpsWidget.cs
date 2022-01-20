using UnityEngine;
using UnityEngine.UI;

namespace DosinisSDK.Utils
{
    public class FpsWidget : MonoBehaviour
    {
        private Text text;

        private float hudRefreshRate = 1;
        private float timer;

        private void Awake()
        {
            if (gameObject.TryGetComponent(out Text text))
            {
                this.text = text;
            }
            else
            {
                this.text = gameObject.AddComponent<Text>();
            }
        }

        private void Update()
        {
            if (Time.unscaledTime > timer)
            {
                int fps = (int)(1f / Time.unscaledDeltaTime);
                text.text = "FPS: " + fps;
                timer = Time.unscaledTime + hudRefreshRate;
            }
        }
    }
}

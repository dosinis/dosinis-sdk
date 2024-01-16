using TMPro;
using UnityEngine;

namespace DosinisSDK.Utils
{
    [RequireComponent(typeof(TMP_Text))]
    public class FpsWidget : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        
        private const float FPS_MEASURE_PERIOD = 0.5f;

        private int framesPassed = 0;
        private float nextFrame = 0;
        private int fps;
        
        private static FpsWidget instance;

        public static void Enable(bool value)
        {
            if (instance == null || instance.gameObject == null)
            {
                Debug.LogWarning("FpsWidget is not initialized");
                return;
            }

            instance.gameObject.SetActive(value);
        }

        private void Awake()
        {
            instance = this;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }
#endif

        private void Update()
        {
            framesPassed++;
            
            if (Time.realtimeSinceStartup > nextFrame)
            {
                fps = (int)(framesPassed / FPS_MEASURE_PERIOD);
                framesPassed = 0;
                nextFrame += FPS_MEASURE_PERIOD;
            }

            text.text = "FPS: " + fps;
        }
    }
}
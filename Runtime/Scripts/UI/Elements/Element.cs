using UnityEngine;

namespace DosinisSDK.UI.Elements
{
    public class Element : MonoBehaviour
    {
        private RectTransform rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = GetComponent<RectTransform>();
                }

                return rectTransform;
            }
        }

        protected bool initialized = false;

        public void Init()
        {
            if (initialized)
                return;

            initialized = true;
            OnInit();
        }

        protected virtual void OnInit()
        {
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Process(float delta)
        {
        }
    }
}
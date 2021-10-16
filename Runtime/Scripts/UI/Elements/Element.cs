using UnityEngine;

namespace DosinisSDK.UI
{
    public class Element : MonoBehaviour
    {
        protected bool initialized = false;

        public virtual void Init()
        {
            if (initialized)
                return;

            initialized = true;
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
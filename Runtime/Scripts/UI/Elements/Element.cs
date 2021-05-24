using UnityEngine;

namespace DosinisSDK.UI
{
    public class Element : MonoBehaviour
    {
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
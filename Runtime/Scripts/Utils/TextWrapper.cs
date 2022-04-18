using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DosinisSDK.Utils
{
    public class TextWrapper : MonoBehaviour
    {
        private Text text;
        private TMP_Text tmpText;

        public Color Color
        {
            get
            {
                SetComponents();

                return text ? text.color : tmpText.color;
            }

            set
            {
                SetComponents();

                if (text)
                {
                    text.color = value;
                }
                else
                {
                    tmpText.color = value;
                }
            }
        }

        public string Text
        {
            get
            {
                SetComponents();

                return text ? text.text : tmpText.text;
            }

            set
            {
                SetComponents();

                if (text)
                {
                    text.text = value;
                }
                else
                {
                    tmpText.text = value;
                }
            }
        }

        public bool RaycastTarget
        {
            get
            {
                SetComponents();
                return text ? text.raycastTarget : tmpText.raycastTarget;
            }

            set
            {
                SetComponents();

                if (text)
                {
                    text.raycastTarget = value;
                }
                else
                {
                    tmpText.raycastTarget = value;
                }
            }
        }

        public void SetAlpha(float value)
        {
            SetComponents();

            if (text)
            {
                text.SetAlpha(value);
            }
            else
            {
                var color = tmpText.color;
                tmpText.color = color.SetAlpha(value);
            }
        }

        private void SetComponents()
        {
            if (text == null && tmpText == null)
            {
                text = GetComponent<Text>();
                tmpText = GetComponent<TMP_Text>();
            }
        }

        private void Awake()
        {
            SetComponents();
        }
    }
}

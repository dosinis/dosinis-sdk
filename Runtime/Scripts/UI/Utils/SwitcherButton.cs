using System;
using UnityEngine;
using UnityEngine.UI;

namespace DosinisSDK.UI
{
    public class SwitcherButton : MonoBehaviour
    {
        [SerializeField] private GameObject enabledState;
        [SerializeField] private GameObject disabledState;

        public event Action<bool> ClickAction = b => { };

        private bool value;

        private bool initialized = false;

        public void Init(bool value)
        {
            if (initialized)
            {
                Debug.LogError("Switcher is already initialized");
                return;
            }

            this.value = value;
            UpdateSwitcherState();
            GetComponent<Button>().onClick.AddListener(OnClick);
            initialized = true;
        }

        protected virtual void OnClick()
        {
            value = !value;
            UpdateSwitcherState();
            ClickAction(value);
        }

        private void UpdateSwitcherState()
        {
            enabledState.SetActive(value);
            disabledState.SetActive(!value);
        }
    }
}



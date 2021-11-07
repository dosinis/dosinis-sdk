using System;
using UnityEngine;
using UnityEngine.UI;

namespace DosinisSDK.UI
{
    [RequireComponent(typeof(Button))]
    public class SwitcherButton : MonoBehaviour
    {
        [SerializeField] private GameObject enabledState;
        [SerializeField] private GameObject disabledState;

        public event Action<bool> ClickAction = b => { };

        private bool isEnabled;

        private bool initialized = false;

        public void Init(bool value)
        {
            if (initialized)
            {
                Debug.LogError("Switcher is already initialized");
                return;
            }

            isEnabled = value;
            UpdateSwitcherState();
            GetComponent<Button>().onClick.AddListener(OnClick);
            initialized = true;
        }

        protected virtual void OnClick()
        {
            isEnabled = !isEnabled;
            UpdateSwitcherState();
            ClickAction(isEnabled);
        }

        private void UpdateSwitcherState()
        {
            enabledState.SetActive(isEnabled);
            disabledState.SetActive(!isEnabled);
        }
    }
}



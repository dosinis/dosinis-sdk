using System;
using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.UI
{
    public class ToggleButton : Button
    {
        [SerializeField] private bool toggled = false;
        [SerializeField] private GameObject onState;
        [SerializeField] private GameObject offState;
        
        public event Action<bool> OnToggle;

        protected override void OnInit(IApp app)
        {
            base.OnInit(app);
            SetWithoutNotify(toggled);
        }

        public void SetWithoutNotify(bool value)
        {
            toggled = value;
            
            onState.SetActive(toggled);
            offState.SetActive(!toggled);
        }
        
        public void Set(bool value)
        {
            SetWithoutNotify(value);
            OnToggle?.Invoke(toggled);
        }
        
        protected override void ClickPerformed()
        {
            base.ClickPerformed();
            Set(!toggled);
        }
    }
}

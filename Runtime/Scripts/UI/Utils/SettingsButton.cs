using DosinisSDK.Core;
using UnityEngine;

namespace DosinisSDK.UI
{
    [RequireComponent(typeof(BouncyButton))]
    public class SettingsButton : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<BouncyButton>().onClick.AddListener(() =>
            {
                App.Core.GetCachedModule<IUIManager>().GetWindow<SettingsWindow>().Show();
            });
        }
    }

}

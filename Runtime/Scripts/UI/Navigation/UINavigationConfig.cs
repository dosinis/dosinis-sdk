using DosinisSDK.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DosinisSDK.UI.Navigation
{
    [CreateAssetMenu(menuName = "DosinisSDK/UINavigationConfig", fileName = "UINavigationConfig")]
    public class UINavigationConfig: ModuleConfig
    {
        [field: SerializeField] public bool IsEnabled { get; private set; }
        [field: SerializeField] public InputActionReference OnSubmitAction { get; private set; }
        [field: SerializeField] public InputActionReference OnCancelAction { get; private set; }
        [field: SerializeField] public InputActionReference OnMoveAction { get; private set; }
        [field: SerializeField] public InputActionReference OnTabMovePrevAction { get; private set; }
        [field: SerializeField] public InputActionReference OnTabMoveNextAction { get; private set; }
    }
}
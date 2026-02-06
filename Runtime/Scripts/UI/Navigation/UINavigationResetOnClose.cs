using System;
using DosinisSDK.Core;
using DosinisSDK.Utils;

namespace DosinisSDK.UI.Navigation
{
    public class UINavigationResetOnClose : ManagedBehaviour
    {
        private IUINavigationController navigationController;
        protected override void OnInit(IApp app)
        {
            navigationController = app.GetModule<IUINavigationController>();
        }

        private void OnDisable()
        {
            navigationController.RebuildNavigation();
        }
    }
}
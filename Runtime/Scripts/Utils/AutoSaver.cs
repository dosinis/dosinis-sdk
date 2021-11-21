using DosinisSDK.Core;
using System.Collections;
using UnityEngine;

namespace DosinisSDK.Utils
{
    public class AutoSaver : BehaviourModule
    {
        [SerializeField] private int autoSaveInterval = 30;

        private IDataManager dataManager;

        public override void Init(IApp app, ModuleConfig config = null)
        {
            base.Init(app, config);

            dataManager = App.Core.GetCachedModule<IDataManager>();

            StartCoroutine(AutoSaveCoroutine());
        }

        private IEnumerator AutoSaveCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(autoSaveInterval);
                dataManager.SaveAll();
            }
        }
    }
}


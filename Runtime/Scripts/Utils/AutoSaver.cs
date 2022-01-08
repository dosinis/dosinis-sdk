using DosinisSDK.Core;
using System.Collections;
using UnityEngine;

namespace DosinisSDK.Utils
{
    public class AutoSaver : BehaviourModule
    {
        [SerializeField] private int autoSaveInterval = 30;

        private IDataManager dataManager;

        protected override void OnInit(IApp app)
        {
            dataManager = App.Core.GetModule<IDataManager>();

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


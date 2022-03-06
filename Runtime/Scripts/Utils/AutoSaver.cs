using DosinisSDK.Core;
using System.Collections;
using UnityEngine;

namespace DosinisSDK.Utils
{
    public class AutoSaver : ManagedBehaviour
    {
        [SerializeField] private int autoSaveInterval = 30;

        private IDataManager dataManager;

        private IEnumerator AutoSaveCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(autoSaveInterval);
                dataManager.SaveAll();
            }
        }

        protected override void Init()
        {
            dataManager = App.Core.GetModule<IDataManager>();

            StartCoroutine(AutoSaveCoroutine());
        }
    }
}


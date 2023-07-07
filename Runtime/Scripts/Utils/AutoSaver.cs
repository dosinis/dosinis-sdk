using DosinisSDK.Core;
using System.Collections;
using UnityEngine;

namespace DosinisSDK.Utils
{
    public class AutoSaver : ManagedBehaviour
    {
        [SerializeField] private int autoSaveInterval = 30;

        private WaitForSeconds waitInterval;
        private IDataManager dataManager;

        private IEnumerator AutoSaveCoroutine()
        {
            while (true)
            {
                yield return waitInterval;
                dataManager.SaveAll();
            }
        }

        protected override void OnInit(IApp app)
        {
            dataManager = app.GetModule<IDataManager>();

            waitInterval = new WaitForSeconds(autoSaveInterval);
            
            StartCoroutine(AutoSaveCoroutine());
        }
    }
}


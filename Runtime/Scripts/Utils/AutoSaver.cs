using DosinisSDK.Core;
using System.Collections;
using UnityEngine;

namespace DosinisSDK.Utils
{
    public class AutoSaver : Module
    {
        private const int AUTO_SAVE_INTERVAL = 60;

        private WaitForSeconds waitInterval;
        private IDataManager dataManager;

        private IEnumerator AutoSaveCoroutine()
        {
            while (true)
            {
                yield return waitInterval;
                dataManager.SaveAll();
            }
            // ReSharper disable once IteratorNeverReturns
        }

        protected override void OnInit(IApp app)
        {
            dataManager = app.GetModule<IDataManager>();
            waitInterval = new WaitForSeconds(AUTO_SAVE_INTERVAL);
            app.Coroutine.Begin(AutoSaveCoroutine());
        }
    }
}


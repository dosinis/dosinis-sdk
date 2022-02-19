using DosinisSDK.Core;
using System.Collections;
using UnityEngine;

namespace DosinisSDK.Utils
{
    public class AutoSaver : MonoBehaviour
    {
        [SerializeField] private int autoSaveInterval = 30;

        private IDataManager dataManager;

        private void Awake()
        {
            App.InitSignal(() => 
            {
                dataManager = App.Core.GetModule<IDataManager>();

                StartCoroutine(AutoSaveCoroutine());
            });      
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


using System.Collections.Generic;
using UnityEngine;

namespace DosinisSDK.Utils
{
    public static class TimeScaleUtils
    {
        private static float baseTimeScale = 1f;
        private static readonly HashSet<string> pauseLocks = new();
        private static bool timeOverriden = false;
        private static float timeOverrideValue = 1f;

        public static void SetBaseTimeScale(float timeScale)
        {
            baseTimeScale = timeScale;
            Time.timeScale = baseTimeScale;
        }
        
        public static void Pause(string id)
        {
            if (pauseLocks.Add(id))
            {
                Time.timeScale = float.Epsilon;
            }
            else
            {
                Debug.LogWarning($"Can't pause for {id}. Already paused!");
            }
        }

        public static void Resume(string id)
        {
            if (pauseLocks.Remove(id) == false)
            {
                Debug.LogWarning($"Can't resume for {id}. Currently not paused!");
                return;
            }
            
            if (timeOverriden)
            {
                Time.timeScale = timeOverrideValue;
                return;
            }
            
            if (pauseLocks.Count == 0)
            {
                Time.timeScale = baseTimeScale;
            }
        }
        
        public static void ForceResume()
        {
            pauseLocks.Clear();
            
            if (timeOverriden)
            {
                Time.timeScale = timeOverrideValue;
                return;
            }
            
            Time.timeScale = baseTimeScale;
        }

        public static void StartTimeOverride(float timeOverride)
        {
            timeOverriden = true;
            timeOverrideValue = timeOverride;
            
            Time.timeScale = timeOverride;
        }

        public static void StopTimeOverride()
        {
            timeOverriden = false;
            Time.timeScale = baseTimeScale;
        }
    }
}
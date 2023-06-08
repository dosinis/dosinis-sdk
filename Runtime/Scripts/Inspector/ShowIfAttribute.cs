using System;
using UnityEngine;

namespace DosinisSDK.Inspector
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string FieldName { get; private set; }
        public object ComparedValue { get; private set; }
        public AppearType AppearingType { get; private set; }
        
        public enum AppearType
        {
            ReadOnly = 2,
            DontDraw = 3
        }

        /// <summary>
        /// Only draws the field only if a condition is met. Supports enum, int, bool and null.
        /// </summary>
        /// <param name="fieldName">The name of the property that is being compared (case sensitive).</param>
        /// <param name="comparedValue">The value the property is being compared to.</param>
        /// <param name="appearingType">The type of disabling that should happen if the condition is NOT met. Defaulted to DisablingType.DontDraw.</param>
        public ShowIfAttribute(string fieldName, object comparedValue, AppearType appearingType = AppearType.DontDraw)
        {
            FieldName = fieldName;
            ComparedValue = comparedValue;
            AppearingType = appearingType;
        }
    }
}

using System;
using UnityEngine;

namespace DosinisSDK.Inspector
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfNotNullAttribute : PropertyAttribute
    {
        public string FieldName { get; private set; }
        public AppearType AppearingType { get; private set; }
        
        public enum AppearType
        {
            ReadOnly = 2,
            DontDraw = 3
        }

        /// <summary>
        /// Only draws the field only if the object with the fieldName is not null.
        /// </summary>
        /// <param name="fieldName">The name of the property that is being compared (case sensitive).</param>
        /// <param name="appearingType">The type of disabling that should happen if the condition is NOT met. Defaulted to DisablingType.DontDraw.</param>
        public ShowIfNotNullAttribute(string fieldName, AppearType appearingType = AppearType.DontDraw)
        {
            FieldName = fieldName;
            AppearingType = appearingType;
        }
    }
}

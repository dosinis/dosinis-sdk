using System.Collections.Generic;
using UnityEditor;

namespace DosinisSDK.Editor
{
    public static class EditorExtensions
    {
        public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty property)
        {
            var currentProperty = property.Copy();
            var nextSiblingProperty = property.Copy();
            {
                nextSiblingProperty.Next(false);
            }

            if (currentProperty.Next(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                        break;

                    yield return currentProperty;
                } while (currentProperty.Next(false));
            }
        }
    }
}
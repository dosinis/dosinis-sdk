using System.Numerics;
using DosinisSDK.Utils;
using UnityEditor;
using UnityEngine;

namespace DosinisSDK.Editor
{
    [CustomPropertyDrawer(typeof(BigNumber))]
    public class BigNumberDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Draw(position, property, label);
        }

        public static void Draw(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty prop = property.FindPropertyRelative("stringValue");
            
            if (prop != null)
            {
                EditorGUI.PropertyField(position, prop, label);
                if (BigInteger.TryParse(prop.stringValue, out BigInteger v))
                {
                    prop.stringValue = v.ToString();
                }
            }
            else
            {
                Debug.LogError(property.type);
            }
        }
    }
}
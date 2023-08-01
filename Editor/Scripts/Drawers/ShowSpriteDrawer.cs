using DosinisSDK.Inspector;
using UnityEditor;
using UnityEngine;

namespace DosinisSDK.Editor
{
    [CustomPropertyDrawer(typeof(ShowSprite), true)]
    public class ShowSpriteDrawer : PropertyDrawer
    {
        private const float SPRITE_SIZE = 55;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var spriteRect = new Rect(position.width - SPRITE_SIZE, position.y, SPRITE_SIZE, SPRITE_SIZE);
            
            if (Selection.objects.Length <= 1)
            {
                EditorGUI.DrawRect(position,new Color(0.25f, 0.25f, 0.25f));
            
                EditorGUI.LabelField(labelRect, new GUIContent(label.text));
                property.objectReferenceValue = EditorGUI.ObjectField(spriteRect, property.objectReferenceValue, typeof(Sprite), false);
            }
            else
            {
                EditorGUI.LabelField(position, "Multi object Sprite editing disabled.");
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + (SPRITE_SIZE - EditorGUIUtility.singleLineHeight);
        }
    }
}
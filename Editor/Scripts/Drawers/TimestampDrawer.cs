using System;
using DosinisSDK.Inspector;
using UnityEditor;
using UnityEngine;

namespace DosinisSDK.Editor
{
    [CustomPropertyDrawer(typeof(TimestampAttribute))]
    public class TimestampDrawer : PropertyDrawer
    {
        private const float WIDTH_OFFSET = 12f;
        private const float HEIGHT_OFFSET = 8f;
        private const float X_OFFSET = 6f;
        private const float Y_OFFSET = 4f;
        private const float GAP_HEIGHT = 2.5f;
        private const int FIELDS_AMOUNT = 6;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var box = new Rect(position.x, position.y, position.width, position.height);
            
            GUI.Box(box, GUIContent.none);
            
            var content = new Rect(
                position.x + X_OFFSET,
                position.y + Y_OFFSET,
                position.width - WIDTH_OFFSET,
                position.height - HEIGHT_OFFSET);
            
            long unixStamp = property.longValue;
            var date = DateTimeOffset.FromUnixTimeSeconds(unixStamp).UtcDateTime;
            
            float line = EditorGUIUtility.singleLineHeight;
            float y = content.y;  
            
            EditorGUI.LabelField(new Rect(content.x, y, content.width, EditorGUIUtility.singleLineHeight),
                label.text, EditorStyles.boldLabel);

            y += line + GAP_HEIGHT;
            int year = EditorGUI.IntField(new Rect(content.x, y, content.width, line), "Year",
                date.Year);
            
            y += line + GAP_HEIGHT;
            int month = EditorGUI.IntField(new Rect(content.x, y, content.width, line), "Month",
                date.Month);
            
            y += line + GAP_HEIGHT;
            int day = EditorGUI.IntField(new Rect(content.x, y, content.width, line), "Day",
                date.Day);
            
            y += line + GAP_HEIGHT;
            int hour = EditorGUI.IntField(new Rect(content.x, y, content.width, line), "Hour",
                date.Hour);

            y += line + GAP_HEIGHT;
            EditorGUI.LabelField(new Rect(content.x, y, content.width, EditorGUIUtility.singleLineHeight),
                $"Date: {date:yyyy-MM-dd HH:mm} UTC", EditorStyles.helpBox);

            year = Mathf.Clamp(year, 1970, 3000);
            month = Mathf.Clamp(month, 1, 12);
            int maxDay = DateTime.DaysInMonth(year, month);
            day = Mathf.Clamp(day, 1, maxDay);
            hour = Mathf.Clamp(hour, 0, 23);

            var newDate = new DateTime(year, month, day, hour, 0, 0, DateTimeKind.Utc);
            long newStamp = new DateTimeOffset(newDate).ToUnixTimeSeconds();

            if (newStamp != property.longValue)
                property.longValue = newStamp;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + GAP_HEIGHT) * FIELDS_AMOUNT + WIDTH_OFFSET;
        }
    }
}
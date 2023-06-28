using System;

using UnityEngine;

using UnityEditor;

namespace Timestamp.Editor
{
    using static UnityEditor.EditorGUI;
    using static UnityEditor.EditorGUIUtility;
    //using static Timestamp;

    [CustomPropertyDrawer(typeof(Timestamp))]
    public class TimestampPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            LabelField(rect, label, new GUIContent(GetTimestamp(property).ToString(Timestamp.iso8601HyphenatedFormat)));
            if (null != Event.current && EventType.ContextClick == Event.current.type)
            {
                if (rect.Contains(Event.current.mousePosition))
                {
                    UInt64 uint64 = UInt64(property);

                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Now"), false, OnNow, property);
                    menu.AddItem(new GUIContent("Utc Now"), false, OnUtcNow, property);
                    if (0 == uint64)
                    {
                        menu.AddDisabledItem(new GUIContent("Reset"));
                        menu.AddDisabledItem(new GUIContent("Copy"));
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Reset"), false, OnResetTimestamp, property);
                        menu.AddItem(new GUIContent("Copy"), false, OnCopy, uint64);
                    }

                    try
                    {
                        UInt64 buffer = Convert.ToUInt64(systemCopyBuffer);
                        menu.AddItem(new GUIContent("Paste"), false, OnPaste, property);
                    }
                    catch (OverflowException)
                    {
                        menu.AddDisabledItem(new GUIContent("Paste"));
                    }
                    catch (FormatException)
                    {
                        menu.AddDisabledItem(new GUIContent("Paste"));
                    }

                    menu.ShowAsContext();
                }
            }
        }

        static void OnResetTimestamp(object userData) { UInt64(userData as SerializedProperty, 0); }
        static void OnNow(object userData) { UInt64(userData as SerializedProperty, Timestamp.Now.uint64); }
        static void OnUtcNow(object userData) { UInt64(userData as SerializedProperty, Timestamp.UtcNow.uint64); }
        static void OnCopy(object userData) { systemCopyBuffer = ((UInt64)userData).ToString(); }
        static void OnPaste(object userData)
        {
            try
            {
                UInt64 buffer = Convert.ToUInt64(systemCopyBuffer);
                UInt64(userData as SerializedProperty, buffer);
            }
            catch { Debug.LogWarning("Unable to calculate Timestamp from Clipboard."); }
        }

        static Timestamp GetTimestamp(SerializedProperty property) { return new Timestamp { uint64 = UInt64(property) }; }
        static UInt64 UInt64(SerializedProperty property)
        {
            return (UInt64)property.FindPropertyRelative(nameof(Timestamp.uint64)).longValue;
        }

        static void UInt64(SerializedProperty property, UInt64 value)
        {
            if (null != property)
            {
                unchecked
                {
                    property.FindPropertyRelative(nameof(Timestamp.uint64)).longValue = (long)value;
                }
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;

namespace FIMSpace.FEditor
{
    [CustomPropertyDrawer(typeof(FPD_HideOnBoolAttribute))]
    public class FPropDrawers_HideOnBool : PropertyDrawer
    {
        FPD_HideOnBoolAttribute Attribute { get { return ((FPD_HideOnBoolAttribute)base.attribute); } }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent content)
        {
            bool enabled = IsEnabled(property);

            bool wasEnabled = GUI.enabled;
            GUI.enabled = enabled;

            if (!Attribute.HideInInspector || enabled)
            {
                EditorGUI.PropertyField(rect, property, content, true);
            }

            GUI.enabled = wasEnabled;
        }

        private bool IsEnabled(SerializedProperty property)
        {
            bool enabled;
            SerializedProperty boolProp = property.serializedObject.FindProperty(Attribute.BoolVarName);

            if (boolProp == null) enabled = true;
            else enabled = boolProp.boolValue;

            return enabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool enabled = IsEnabled(property);

            if (!Attribute.HideInInspector || enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }
}


#endif

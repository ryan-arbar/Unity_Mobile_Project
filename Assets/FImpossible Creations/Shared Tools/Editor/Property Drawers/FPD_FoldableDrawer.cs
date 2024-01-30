using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FIMSpace.FEditor
{
    [CustomPropertyDrawer(typeof(FPD_FoldableAttribute))]
    public class FPD_Foldable : PropertyDrawer
    {
        FPD_FoldableAttribute Attribute { get { return ((FPD_FoldableAttribute)base.attribute); } }
        private SerializedProperty foldProp = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (string.IsNullOrEmpty(Attribute.FoldVariable) == false) if ( foldProp == null ) foldProp = property.serializedObject.FindProperty(Attribute.FoldVariable);

            if (foldProp == null)
                EditorGUI.PropertyField(position, property, label);
            else
            {
                if (foldProp.boolValue)
                    EditorGUI.PropertyField(position, property, label);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (string.IsNullOrEmpty(Attribute.FoldVariable) == false) if (foldProp == null) foldProp = property.serializedObject.FindProperty(Attribute.FoldVariable);

            if (foldProp == null)
                return base.GetPropertyHeight(property, label);
            else
            {
                if (foldProp.boolValue)
                    return base.GetPropertyHeight(property, label);
                else
                    return 0;

            }
        }

    }


}


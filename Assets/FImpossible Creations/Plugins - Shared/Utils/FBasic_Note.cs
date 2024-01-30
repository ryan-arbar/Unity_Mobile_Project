#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FIMSpace.Basics
{
    public class FBasic_Note : MonoBehaviour
    {
        [HideInInspector] public string Title = "Custom Note";
        [HideInInspector] public string Note = "Write here custom description message for inspector view guide for users.";
        [HideInInspector] public bool EditMode = true;

        public Color HeaderColor = Color.white;
        public Color NoteColor = Color.white;
    }


#if UNITY_EDITOR
    /// <summary>
    /// FM: Editor class component to enchance controll over component from inspector window
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FBasic_Note))]
    public class FBasic_NoteEditor : UnityEditor.Editor
    {
        public FBasic_Note Get { get { if (_get == null) _get = (FBasic_Note)target; return _get; } }
        private FBasic_Note _get;

        private static GUIStyle header = null;
        private static GUIStyle padding = null;
        private static GUIStyle displayText = null;

        public override void OnInspectorGUI()
        {

            #region Refresh Styles

            Color preC = GUI.color;

            if (header == null)
            {
                header = new GUIStyle(EditorStyles.boldLabel);
                header.alignment = TextAnchor.MiddleCenter;
            }

            if (padding == null)
            {
                padding = new GUIStyle();
                padding.padding = new RectOffset(10, 10, 6, 6);
            }

            if (displayText == null)
            {
                displayText = new GUIStyle(EditorStyles.label);
                displayText.richText = true;
            }

            #endregion

            EditorGUI.BeginChangeCheck();

            GUILayout.Space(4);

            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            GUI.color = Get.HeaderColor;

            string title = Get.Title;
            if (Get.EditMode)
            {
                title = EditorGUILayout.TextField(Get.Title);
            }
            else
            {
                EditorGUILayout.LabelField(Get.Title, header);
            }

            GUI.color = preC;

            if (GUILayout.Button(EditorGUIUtility.IconContent("CollabEdit Icon"), EditorStyles.label, GUILayout.Width(22), GUILayout.Height(18)))
            {
                Get.EditMode = !Get.EditMode;
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(8);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginVertical(padding);

            GUI.color = Get.NoteColor;

            string note = Get.Note;
            if (Get.EditMode)
            {
                note = EditorGUILayout.TextArea(Get.Note);
            }
            else
            {
                GUIContent gc_note = new GUIContent(Get.Note);
                GUILayout.Label(gc_note, displayText, GUILayout.Height(EditorStyles.label.CalcSize(gc_note).y));
            }

            GUI.color = preC;

            GUILayout.EndVertical();
            GUILayout.EndVertical();
            GUILayout.Space(4);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(Get);

                Undo.RecordObject(Get, "note");
                Get.Title = title;
                Get.Note = note;
            }

            if (Get.EditMode)
            {
                serializedObject.Update();

                DrawPropertiesExcluding(serializedObject, "m_Script");

                serializedObject.ApplyModifiedProperties();
            }

        }
    }
#endif

}

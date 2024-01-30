using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FIMSpace.FEditor
{
#if UNITY_EDITOR
    public static class FInspectorHiderExt
    {
        // Helper toolbar popup for aduio sources with RMB
        [MenuItem("CONTEXT/FInspectorHider/Inspector Hider: Unhide all found components")]
        private static void IHRevealAll(MenuCommand menuCommand)
        {
            FInspectorHider fih = menuCommand.context as FInspectorHider;
            if (fih)
            {
                foreach (var item in fih.GetComponents<Component>())
                {
                    item.hideFlags = HideFlags.None;
                    EditorUtility.SetDirty(item);
                }
            }
        }

        [MenuItem("CONTEXT/FInspectorHider/Inspector Hider: Switch Draw Version")]
        private static void IHDrawVersion(MenuCommand menuCommand)
        {
            FInspectorHider fih = menuCommand.context as FInspectorHider;
            if (fih)
            {
                fih.V = !fih.V;
            }
        }
    }
#endif

    [ExecuteInEditMode]
    public class FInspectorHider : MonoBehaviour
    {
#if UNITY_EDITOR
        //[FPD_SingleLineTwoProps("V", 150, 30, 40)] public bool AutoUnhideOnFoldout = false;
        //[Tooltip("Display V2")]
        [HideInInspector] public bool V = false;

        public List<Component> ToHide = new List<Component>();
        public List<HideGroup> ToHideGroups = new List<HideGroup>();

        [System.Serializable]
        public class HideGroup
        {
            public string Title = "Group 1";
            public List<Component> ToHide;

            public HideFlags GetFlag
            {
                get
                {
                    if (ToHide.Count == 0) return HideFlags.None;
                    if (ToHide[0] == null) return HideFlags.None;
                    return ToHide[0].hideFlags;
                }
            }

            public void SwitchVisibility(bool visible)
            {
                for (int h = 0; h < ToHide.Count; h++)
                {
                    if (ToHide[h] == null) continue;
                    ToHide[h].hideFlags = visible ? HideFlags.None : HideFlags.HideInInspector;
                    EditorUtility.SetDirty(ToHide[h]);
                }
            }

            public void SwitchVisibility()
            {
                if (ToHide.Count <= 0) return;
                if (ToHide[0] == null) return;
                SwitchVisibility(ToHide[0].hideFlags == HideFlags.None ? false : true);
            }
        }

        public void SwitchVisibility(Component cmp, bool visible)
        {
            cmp.hideFlags = visible ? HideFlags.None : HideFlags.HideInInspector;
            EditorUtility.SetDirty(cmp);
        }

        public void SwitchVisibility(Component cmp)
        {
            SwitchVisibility(cmp, cmp.hideFlags == HideFlags.None ? false : true);
        }


        #region Commented but can be helpful later

        //public void SwitchVisibilityMemo()
        //{
        //    for (int i = 0; i < ToHide.Count; i++)
        //    {
        //        if (ToHide[i] == null) continue;
        //        if (ToHide[i].hideFlags != HideFlags.None) ToHide[i].hideFlags = HideFlags.HideInHierarchy;
        //    }

        //    for (int g = 0; g < ToHideGroups.Count; g++)
        //    {
        //        for (int i = 0; i < ToHideGroups[g].ToHide.Count; i++)
        //        {
        //            if (ToHideGroups[g].ToHide[i] == null) continue;
        //            if (ToHideGroups[g].ToHide[i].hideFlags != HideFlags.None) ToHideGroups[g].ToHide[i].hideFlags = HideFlags.HideInHierarchy;
        //        }
        //    }
        //}

        //public void RestoreVisibilityMemo()
        //{
        //    for (int i = 0; i < ToHide.Count; i++)
        //    {
        //        if (ToHide[i] == null) continue;
        //        SwitchVisibility(ToHide[i], ToHide[i].hideFlags == HideFlags.None);
        //    }

        //    for (int g = 0; g < ToHideGroups.Count; g++)
        //    {
        //        for (int i = 0; i < ToHideGroups[g].ToHide.Count; i++)
        //        {
        //            if (ToHideGroups[g].ToHide[i] == null) continue;
        //            SwitchVisibility(ToHideGroups[g].ToHide[i], ToHideGroups[g].ToHide[i].hideFlags == HideFlags.None);
        //        }
        //    }
        //}

        #endregion


        public void SwitchAllVisibility(bool visible)
        {
            for (int i = 0; i < ToHide.Count; i++)
            {
                if (ToHide[i] == null) continue;
                SwitchVisibility(ToHide[i], visible);
            }

            for (int g = 0; g < ToHideGroups.Count; g++)
            {
                ToHideGroups[g].SwitchVisibility(visible);
            }
        }

        private void OnDestroy()
        {
            SwitchAllVisibility(true);
        }

#endif
    }


    #region Editor Class
#if UNITY_EDITOR
    [UnityEditor.CanEditMultipleObjects]
    [UnityEditor.CustomEditor(typeof(FInspectorHider))]
    public class FInspectorHiderEditor : UnityEditor.Editor
    {
        public FInspectorHider Get { get { if (_get == null) _get = (FInspectorHider)target; return _get; } }
        private FInspectorHider _get;

        string[] _hide = new string[] { "m_Script" };
        string[] _hideV = new string[] { "m_Script", "ToHide", "ToHideGroups" };


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, Get.V ? _hideV : _hide);
            serializedObject.ApplyModifiedProperties();

            float width = EditorGUIUtility.currentViewWidth;
            float marginWdth = 30f;
            float widthAccum = marginWdth;
            GUIContent _gc = new GUIContent();

            //if (Get.AutoUnhideOnFoldout) if (Get.V) return;

            if (Get.ToHide.Count > 0)
            {

                if (Get.V == false) FGUI_Inspector.DrawUILine(0.5f, 0.3f, 2, 6, 0.975f);

                #region Singles to hide

                EditorGUILayout.BeginHorizontal();

                for (int i = 0; i < Get.ToHide.Count; i++)
                {
                    if (Get.ToHide[i] == null) continue;

                    _gc.text = Get.ToHide[i].GetType().Name;
                    float bWidth = EditorStyles.miniButton.CalcSize(_gc).x + 4;

                    if (widthAccum + bWidth > width)
                    {
                        EditorGUILayout.EndHorizontal();
                        widthAccum = marginWdth;
                        EditorGUILayout.BeginHorizontal();
                    }

                    widthAccum += bWidth;

                    if (Get.ToHide[i].hideFlags != HideFlags.None) GUI.color = new Color(1f, 1f, 1f, 0.5f);
                    else GUI.backgroundColor = Color.green;

                    if (GUILayout.Button(_gc, EditorStyles.miniButton, GUILayout.Width(bWidth)))
                    {
                        Get.SwitchVisibility(Get.ToHide[i]);
                    }

                    GUI.color = Color.white;
                    GUI.backgroundColor = Color.white;
                }

                EditorGUILayout.EndHorizontal();

                #endregion

            }


            if (Get.ToHideGroups.Count > 0)
            {
                FGUI_Inspector.DrawUILine(0.5f, 0.3f, 1, 8, 0.975f);

                #region Groups to hide

                widthAccum = marginWdth;
                EditorGUILayout.BeginHorizontal();

                for (int i = 0; i < Get.ToHideGroups.Count; i++)
                {
                    var group = Get.ToHideGroups[i];

                    _gc.text = group.Title + " (" + group.ToHide.Count + ")";
                    float bWidth = EditorStyles.miniButton.CalcSize(_gc).x + 4;

                    if (widthAccum + bWidth > width)
                    {
                        EditorGUILayout.EndHorizontal();
                        widthAccum = marginWdth;
                        EditorGUILayout.BeginHorizontal();
                    }

                    widthAccum += bWidth;

                    if (group.GetFlag != HideFlags.None) GUI.color = new Color(1f, 1f, 1f, 0.5f);
                    else GUI.backgroundColor = Color.green;

                    if (GUILayout.Button(_gc, EditorStyles.miniButton, GUILayout.Width(bWidth)))
                    {
                        group.SwitchVisibility();
                    }

                    GUI.color = Color.white;
                    GUI.backgroundColor = Color.white;
                }

                EditorGUILayout.EndHorizontal();

                #endregion

            }

            FGUI_Inspector.DrawUILine(0.5f, 0.3f, 2, 6, 0.975f);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show All")) { Get.SwitchAllVisibility(true); }
            if (GUILayout.Button("Hide All")) { Get.SwitchAllVisibility(false); }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(-4);
        }
    }

#endif
    #endregion


}


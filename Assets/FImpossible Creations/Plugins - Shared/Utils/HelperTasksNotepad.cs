#if UNITY_EDITOR
using UnityEditor;
#endif  
using UnityEngine;

public class HelperTasksNotepad : MonoBehaviour
{
    [HideInInspector] public bool _DrawChecklist = false;
#if UNITY_EDITOR
#endif  
}


#region Editor Class
#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(HelperTasksNotepad))]
public class HelperTasksNotepadEditor : UnityEditor.Editor
{
    public HelperTasksNotepad Get { get { if (_get == null) _get = (HelperTasksNotepad)target; return _get; } }
    private HelperTasksNotepad _get;


    GUIStyle _s = null;
    private void OnEnable()
    {
        RefreshStyle();
    }

    void RefreshStyle()
    {
        if (EditorStyles.boldLabel == null) return;
        _s = new GUIStyle(EditorStyles.boldLabel);
        _s.alignment = TextAnchor.MiddleCenter;
    }

    public override void OnInspectorGUI()
    {
        if (_s == null) RefreshStyle();
        if (_s == null) return;

        int tasks = 0;
        int tasksDone = 0;

        var allTr = Get.transform.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < allTr.Length; i++)
        {
            Transform t = allTr[i];

            if (t.childCount == 0)
            {
                tasks += 1;
                if (t.gameObject.activeInHierarchy == false) tasksDone += 1;
            }
        }

        if (tasks <= 1)
        {
            EditorGUILayout.HelpBox("The tasks are generated out of child objects of this game object. Place game objects one in each other to generate nested tasks!", MessageType.Info);
        }

        GUILayout.Space(4);
        EditorGUILayout.LabelField("Tasks:    " + tasksDone + "  /  " + tasks, _s);

        if (tasks > 0)
        {
            Rect progr = GUILayoutUtility.GetLastRect();
            GUI.color = Color.white * 0.5f;
            GUI.Box(progr, GUIContent.none);
            progr.width = progr.width * ((float)tasksDone / (float)tasks);
            GUI.color = Color.green * 1.5f;
            GUI.Box(progr, GUIContent.none);
            GUI.color = Color.white;
        }

        GUILayout.Space(4);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUI.indentLevel++;
        Get._DrawChecklist = EditorGUILayout.Foldout(Get._DrawChecklist, " Checklist: ", true);

        if (Get._DrawChecklist)
        {
            GUILayout.Space(3);

            for (int i = 0; i < allTr.Length; i++)
            {
                Transform t = allTr[i];

                if (t.childCount == 0)
                {
                    string name = "";
                    bool draw = true;

                    if (t.parent != Get.transform)
                    {
                        if (t.parent.localPosition.x == 1f) draw = false;
                        else
                            name = "      └  " + t.name;
                        //name = "   " + t.parent.name + "/ " + t.name;
                    }
                    else
                        name = "   " + t.name;

                    if (draw == false) continue;

                    Event e = Event.current;
                    int button = 0;
                    if (e != null) button = e.button;

                    if (t.gameObject.activeInHierarchy == false)
                    {
                        GUI.color = Color.green * 0.8f;
                        if (GUILayout.Button(name + " ✓", EditorStyles.boldLabel))
                        {
                            if (button == 1) { RenameTransform(t); }
                            else if (button == 2) { RemoveTransform(t); }
                            else t.gameObject.SetActive(!t.gameObject.activeInHierarchy);
                            Dirty(t);
                        }
                    }
                    else
                    {
                        GUI.color = Color.white * 0.8f;
                        if (GUILayout.Button(name, EditorStyles.boldLabel))
                        {
                            if (button == 1) { RenameTransform(t); }
                            else if (button == 2) { RemoveTransform(t); }
                            else t.gameObject.SetActive(!t.gameObject.activeInHierarchy);
                            Dirty(t);
                        }
                    }


                    //if (t.gameObject.activeInHierarchy)
                    //{
                    //    var lRect = GUILayoutUtility.GetLastRect();
                    //    lRect.position = new Vector2(lRect.position.x + lRect.width - 18, lRect.position.y);
                    //    lRect.width = 20;

                    //    if (GUI.Button(lRect, "X", EditorStyles.boldLabel))
                    //    {
                    //        GameObject.Destroy(t.gameObject);
                    //    }
                    //}

                }
                else if (t != Get.transform)
                {
                    bool foldout = t.localPosition.x != 1f;

                    GUILayout.Space(3);

                    int done = 0;
                    for (int c = 0; c < t.childCount; c++)
                    {
                        if (t.GetChild(c).gameObject.activeInHierarchy == false) done += 1;
                    }

                    GUILayout.Label("  ", EditorStyles.boldLabel);
                    Rect lRect = GUILayoutUtility.GetLastRect();

                    if (t.childCount > 0)
                    {
                        if (tasks > 0)
                        {
                            Rect progr = new Rect(lRect);
                            GUI.color = Color.white * 0.35f;
                            GUI.Box(progr, GUIContent.none);
                            progr.width = progr.width * ((float)done / (float)t.childCount);
                            GUI.color = new Color(0.2f, 1f, 0.3f, 0.5f);
                            GUI.Box(progr, GUIContent.none);
                            GUI.color = Color.white;
                        }
                    }


                    string post = "";

                    if (t.gameObject.activeInHierarchy == false || done == t.childCount)
                    { GUI.color = Color.green * 0.85f; post = " ✓"; }
                    else
                        GUI.color = Color.white;

                    float initWdth = lRect.width;

                    lRect.width -= 30;

                    if (GUI.Button(lRect, (foldout ? " ▼  " : " ►  ") + t.name + " (" + (done > 0 ? (done + "/") : "") + t.childCount + ")" + post, EditorStyles.boldLabel))
                    {
                        if (t.localPosition.x == 1f)
                            t.localPosition = Vector3.zero;
                        else
                            t.localPosition = Vector3.right;

                        Dirty(t);
                    }


                    lRect.position = new Vector2(lRect.position.x + initWdth - 18, lRect.position.y);
                    lRect.width = 20;

                    if (GUI.Button(lRect, "+", EditorStyles.boldLabel))
                    {
                        string name = EditorUtility.SaveFilePanelInProject("Type your title (no file will be created)", "New Task", "", "Type your title (no file will be created)");
                        name = name.Replace("Assets/", "");
                        if (!string.IsNullOrEmpty(name))
                        {
                            GameObject go = new GameObject(name);
                            go.transform.SetParent(t);
                            go.transform.localPosition = Vector3.zero;
                        }

                        Dirty(t);
                    }

                    GUILayout.Space(2);
                }
            }

            GUILayout.Space(7);
        }

        EditorGUI.indentLevel--;

        EditorGUILayout.EndVertical();
    }

    void RenameTransform(Transform t)
    {
        string name = EditorUtility.SaveFilePanelInProject("Type your title (no file will be created)", t.name, "", "Type your title (no file will be created)");
        name = name.Replace("Assets/", "");
        if (!string.IsNullOrEmpty(name))
        {
            t.name = name;
            EditorUtility.SetDirty(t);
        }
    }

    void RemoveTransform(Transform t)
    {
        if (EditorUtility.DisplayDialog("Remove " + t.name, "Do you want to remove '" + t.name + "' ?", "Yes", "No"))
        {
            if (Application.isPlaying == false)
                GameObject.DestroyImmediate(t.gameObject);
            else
                GameObject.Destroy(t.gameObject);
        }
    }

    public void Dirty(UnityEngine.Object o)
    {
        if (o == null) return;
        EditorUtility.SetDirty(o);
    }
}
#endif
#endregion

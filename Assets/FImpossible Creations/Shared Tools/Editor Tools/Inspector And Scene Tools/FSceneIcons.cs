#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif
using UnityEngine;

public static class FSceneIcons
{
    public static void SetGizmoIconEnabled(MonoBehaviour beh, bool on)
    {
        if (beh == null) return;
        SetGizmoIconEnabled(beh.GetType(), on);
    }

    public static void SetGizmoIconEnabled(System.Type type, bool on)
    {
#if UNITY_EDITOR

        if (Application.isPlaying) return;

//#if UNITY_2022_1_OR_NEWER
        // On newer unity versions it stopped working
        // giving warning: "Warning: Annotation not found!"
        // and can't find any info in docs, how to make it work again
//#else
        // giving warning: "Warning: Annotation not found!"
        // sometimes it works, sometimes not ¯\_(ツ)_/¯ lets see how bad it goes now 

        MethodInfo setIconEnabled = Assembly.GetAssembly(typeof(Editor))
        ?.GetType("UnityEditor.AnnotationUtility")
        ?.GetMethod("SetIconEnabled", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        //?.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic);

        if (setIconEnabled == null) return;
        const int MONO_BEHAVIOR_CLASS_ID = 114; // https://docs.unity3d.com/Manual/ClassIDReference.html
        setIconEnabled.Invoke(null, new object[] { MONO_BEHAVIOR_CLASS_ID, type.Name, on ? 1 : 0 });
#endif
    }
}


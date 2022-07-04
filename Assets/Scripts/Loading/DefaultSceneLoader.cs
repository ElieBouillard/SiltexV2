#if UNITY_EDITOR
using System.ComponentModel;
using System.Xml.Schema;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class DefaultSceneLoader
{
    static DefaultSceneLoader(){
        EditorApplication.playModeStateChanged += LoadDefaultScene;
    }

    static void LoadDefaultScene(PlayModeStateChange state){
        if (state == PlayModeStateChange.ExitingEditMode) {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ();
        }

        if (state == PlayModeStateChange.EnteredPlayMode)
        {

            EditorSceneManager.LoadScene (0);
        }
    }
}
#endif
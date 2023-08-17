using UnityEngine;
using System.Collections;
using UnityEditor;

namespace NBCore {
    [CustomEditor(typeof(GUIDialogBase))]
    public class UIDialogLoaderInspector : Editor {

        private GUIDialogBase uiDialogLoader;

        public override void OnInspectorGUI () {

            base.OnInspectorGUI();
            uiDialogLoader = (GUIDialogBase)target;

            GUILayout.Label("GUI Dialog Custom Editor", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("LOAD", GUILayout.Width(150), GUILayout.Height(40))) {
                uiDialogLoader.Load();
            } // if

            if (GUILayout.Button("UNLOAD", GUILayout.Width(150), GUILayout.Height(40))) {
                uiDialogLoader.UnLoad();
            } // if
            EditorGUILayout.EndHorizontal();

//            if (GUILayout.Button("SAVE", GUILayout.Width(300), GUILayout.Height(40))) {
//                uiDialogLoader.Save();
//                Object prefab = EditorUtility.CreateEmptyPrefab("");
//            } // if
        }
    }
} // NBCore


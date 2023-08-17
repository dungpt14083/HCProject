using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tweens.Plugins
{
    [CustomEditor(typeof(TweenGroup)), CanEditMultipleObjects]
    public class TweenGroupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TweenGroup myScript = (TweenGroup)target;
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Space(50);
            if (GUILayout.Button("PLAY", GUILayout.Width(100), GUILayout.Height(50)))
            {
                myScript.OnPlay();

            }

            GUILayout.Space(50);
            if (GUILayout.Button("STOP", GUILayout.Width(100), GUILayout.Height(50)))
            {
                myScript.OnStop();
            }

            GUILayout.Space(50);
            if (GUILayout.Button("BACK", GUILayout.Width(100), GUILayout.Height(50)))
            {
                myScript.OnBack();
            }

            GUILayout.EndHorizontal();
        }
    }


}



using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CBSK
{
    [CustomEditor(typeof(PlayerPrefsPersistenceManager))]
    public class PlayerPrefsPersistenceManagerEditor : Editor
    {

        override public void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            //GUILayout.BeginArea(new Rect(0, GUILayoutUtility.GetLastRect().y, Screen.width, 20));
            if (GUILayout.Button("Clear Saved Game"))
            {
                PlayerPrefs.DeleteKey(((PlayerPrefsPersistenceManager)target).playerPrefName);
            }
            if (GUILayout.Button("Print Saved Game "))
            {
                Debug.Log(PlayerPrefs.GetString(((PlayerPrefsPersistenceManager)target).playerPrefName, "NOT FOUND"));
            }
            //GUILayout.EndArea();
            EditorGUILayout.EndHorizontal();


            DrawDefaultInspector();
        }
    }
}

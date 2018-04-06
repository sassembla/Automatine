using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CoroutineGeneratorWindow : EditorWindow
{
    public List<string> existCoroutines;
    public Action<string> enter;

    private string newCoroutineName = string.Empty;

    private void OnGUI()
    {


        using (new GUILayout.VerticalScope())
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("New Coroutine Name");
                GUI.SetNextControlName("newCoroutineName");
                newCoroutineName = GUILayout.TextField(newCoroutineName);
                GUI.FocusControl("newCoroutineName");
            }

            var notGeneratable = false;

            if (existCoroutines.Contains(newCoroutineName))
            {
                EditorGUILayout.HelpBox("Coroutine:" + newCoroutineName + " is already exists. Please just try another name.", MessageType.Error);
                notGeneratable = true;
            }

            if (newCoroutineName.Length == 0)
            {
                EditorGUILayout.HelpBox("Coroutine name:" + newCoroutineName + " is empty.", MessageType.Error);
                notGeneratable = true;
            }

            // accept enter key.
            if (!notGeneratable && (Event.current.type == EventType.KeyUp) && (Event.current.keyCode == KeyCode.Return))
            {
                enter(newCoroutineName);
                this.Close();
            }

            using (new EditorGUI.DisabledGroupScope(notGeneratable))
            {

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Add"))
                    {
                        enter(newCoroutineName);
                        this.Close();
                    }
                }

            }
        }
    }
}
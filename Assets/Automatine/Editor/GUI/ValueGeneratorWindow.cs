
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Automatine
{
    public class ValueGeneratorWindow : EditorWindow
    {

        public string type;
        public List<string> existValues;
        public Action<string> enter;

        private string newValueName = string.Empty;

        private void OnGUI()
        {
            using (new GUILayout.VerticalScope())
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Type");
                    GUILayout.Label(type);
                }

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("New Value Name");
                    GUI.SetNextControlName("newValueName");
                    newValueName = GUILayout.TextField(newValueName);
                    GUI.FocusControl("newValueName");
                }

                var notGeneratable = false;

                if (existValues.Contains(newValueName))
                {
                    EditorGUILayout.HelpBox("Value:" + newValueName + " is already exists. Please just try another name.", MessageType.Error);
                    notGeneratable = true;
                }

                if (newValueName.Length == 0)
                {
                    EditorGUILayout.HelpBox("Value:" + newValueName + " is empty.", MessageType.Error);
                    notGeneratable = true;
                }

                // accept enter key.
                if (!notGeneratable && (Event.current.type == EventType.KeyUp) && (Event.current.keyCode == KeyCode.Return))
                {
                    enter(newValueName);
                    this.Close();
                }

                using (new EditorGUI.DisabledGroupScope(notGeneratable))
                {

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Add"))
                        {
                            enter(newValueName);
                            this.Close();
                        }
                    }

                }

            }
        }
    }
}
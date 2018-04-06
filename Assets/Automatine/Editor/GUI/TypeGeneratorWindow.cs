
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Automatine
{
    public class TypeGeneratorWindow : EditorWindow
    {

        public List<string> existTypes;
        public Action<string> enter;

        private string newTypeName = string.Empty;

        private void OnGUI()
        {
            using (new GUILayout.VerticalScope())
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("New Type Name");
                    GUI.SetNextControlName("newTypeName");
                    newTypeName = GUILayout.TextField(newTypeName);
                    GUI.FocusControl("newTypeName");
                }

                var notGeneratable = false;

                if (existTypes.Contains(newTypeName))
                {
                    EditorGUILayout.HelpBox("Type:" + newTypeName + " is already exists. Please just try another name.", MessageType.Error);
                    notGeneratable = true;
                }
                if (newTypeName.Length == 0)
                {
                    EditorGUILayout.HelpBox("Type name:" + newTypeName + " is empty.", MessageType.Error);
                    notGeneratable = true;
                }

                // accept enter key.
                if (!notGeneratable && (Event.current.type == EventType.KeyUp) && (Event.current.keyCode == KeyCode.Return))
                {
                    enter(newTypeName);
                    this.Close();
                }

                using (new EditorGUI.DisabledGroupScope(notGeneratable))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Add"))
                        {
                            enter(newTypeName);
                            this.Close();
                        }
                    }

                }
            }
        }
    }
}
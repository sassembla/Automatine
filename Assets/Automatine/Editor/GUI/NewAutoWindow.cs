using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Automatine
{
    public class NewAutoWindow : EditorWindow
    {

        public List<string> autoNames;
        public Action<OnAutomatineEvent> Emit;

        private string newAutoName = string.Empty;

        private void OnGUI()
        {
            using (new GUILayout.VerticalScope())
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("New Auto Name");
                    GUI.SetNextControlName("newAutoName");
                    newAutoName = GUILayout.TextField(newAutoName);
                    GUI.FocusControl("newAutoName");
                }

                var notGeneratable = false;

                if (autoNames.Contains(newAutoName))
                {
                    EditorGUILayout.HelpBox("Auto:" + newAutoName + " is already exists. Please just try another name.", MessageType.Error);
                    notGeneratable = true;
                }

                if (newAutoName.Length == 0)
                {
                    EditorGUILayout.HelpBox("Auto name:" + newAutoName + " is empty.", MessageType.Error);
                    notGeneratable = true;
                }

                // accept enter key.
                if (!notGeneratable && (Event.current.type == EventType.KeyUp) && (Event.current.keyCode == KeyCode.Return))
                {
                    Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_AUTO_ADDAUTO, newAutoName));
                    this.Close();
                }

                using (new EditorGUI.DisabledGroupScope(notGeneratable))
                {

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Add"))
                        {
                            Emit(new OnAutomatineEvent(OnAutomatineEvent.EventType.EVENT_AUTO_ADDAUTO, newAutoName));
                            this.Close();
                        }
                    }

                }
            }
        }
    }
}
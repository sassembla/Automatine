
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Automatine
{
    public class ConditionsGUIAuxWindow : EditorWindow
    {

        public Action<OnConditionsEvent> Emit;

        public List<ConditionData> conditions;


        private Vector2 sideScrollPosition = new Vector2();

        private Dictionary<string, Vector2> valuesScrollPositions = new Dictionary<string, Vector2>();


        private TypeGeneratorWindow typeGeneratorWindow;
        private ValueGeneratorWindow valueGeneratorWindow;


        public void OnGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add New Type"))
                {
                    if (typeGeneratorWindow == null) typeGeneratorWindow = CreateInstance<TypeGeneratorWindow>();

                    typeGeneratorWindow.existTypes = conditions.Select(cond => cond.conditionType).ToList();

                    typeGeneratorWindow.enter = (string newTypeName) =>
                    {
                        Emit(new OnConditionsEvent(OnConditionsEvent.EventType.EVENT_ADDTYPE, this, newTypeName, string.Empty));
                        Repaint();
                    };

                    typeGeneratorWindow.ShowAuxWindow();
                }

                GUILayout.FlexibleSpace();

                // if (GUILayout.Button("虫眼鏡フィールド")) {
                //	 Debug.LogError("虫眼鏡検索、、、");
                // } 
            }


            sideScrollPosition = EditorGUILayout.BeginScrollView(sideScrollPosition);
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Space(10);
                    foreach (var condition in conditions)
                    {
                        var conditionType = condition.conditionType;
                        var conditionValues = condition.conditionValues;
                        DtawCondition(conditionType, conditionValues);

                        GUILayout.Space(20);
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }


        /**
		   draw vertical item of type & values. 
		*/
        private void DtawCondition(string type, List<string> values)
        {
            var conditionStyle = new GUIStyle();
            conditionStyle.normal.textColor = new Color(0.70f, 0.70f, 0.70f);
            conditionStyle.alignment = TextAnchor.MiddleCenter;

            // type and values vertical.
            using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(200)))
            {
                using (new GUILayout.HorizontalScope(GUI.skin.box))
                {
                    if (GUILayout.Button(string.Empty, "OL Minus", GUILayout.Width(20)))
                    {
                        Emit(new OnConditionsEvent(OnConditionsEvent.EventType.EVENT_DELETETYPE, this, type, string.Empty));
                    }

                    GUILayout.Label(type);
                }

                // spacer between type to values. 
                GUILayout.Space(10);


                /* 
					values & Add button.
				*/
                if (!valuesScrollPositions.ContainsKey(type)) valuesScrollPositions[type] = new Vector2();

                valuesScrollPositions[type] = EditorGUILayout.BeginScrollView(valuesScrollPositions[type]);
                {
                    // values
                    using (new GUILayout.VerticalScope())
                    {
                        foreach (var val in values)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.FlexibleSpace();

                                if (GUILayout.Button(string.Empty, "OL Minus", GUILayout.Width(20), GUILayout.Height(24)))
                                {
                                    Emit(new OnConditionsEvent(OnConditionsEvent.EventType.EVENT_DELETEVALUE, this, type, val));
                                }

                                GUILayout.Label(val, conditionStyle);

                                GUILayout.Space(20);
                            }
                        }
                    }
                }
                EditorGUILayout.EndScrollView();

                // spacer between values & Add Button.
                GUILayout.Space(10);

                // Add button.
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Add New Value", GUILayout.Width(90)))
                    {
                        if (valueGeneratorWindow == null) valueGeneratorWindow = CreateInstance<ValueGeneratorWindow>();

                        valueGeneratorWindow.type = type;

                        var valuesOfThisType = conditions.Where(condition => condition.conditionType == type).FirstOrDefault();

                        if (valuesOfThisType == null) valueGeneratorWindow.existValues = new List<string>();
                        else valueGeneratorWindow.existValues = valuesOfThisType.conditionValues;

                        valueGeneratorWindow.enter = (string newValueName) =>
                        {
                            Emit(new OnConditionsEvent(OnConditionsEvent.EventType.EVENT_ADDVALUE, this, type, newValueName));
                            Repaint();
                        };

                        valueGeneratorWindow.ShowAuxWindow();
                    }
                }
            }
        }
    }

}
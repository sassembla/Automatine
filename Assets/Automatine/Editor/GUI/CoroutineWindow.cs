using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Automatine
{
    public class CoroutineWindow
    {

        private string currentCoroutineName;

        private string code;

        private readonly List<string> coroutineIds;


        private Vector2 scrollPosY = new Vector2();

        public CoroutineWindow(List<string> coroutineIds)
        {
            this.currentCoroutineName = coroutineIds[0];
            this.code = LoadCoroutineCode(currentCoroutineName);
            this.coroutineIds = coroutineIds;
        }

        public void DrawCoroutineWindow()
        {
            if (string.IsNullOrEmpty(currentCoroutineName)) return;
            if (string.IsNullOrEmpty(code)) return;

            var countMessage = string.Empty;
            if (1 < coroutineIds.Count) countMessage = " + " + (coroutineIds.Count - 1) + " coroutines.";

            try
            {// this code causes error only first time after compiled. and it's not avoidable.
                GUILayout.Space(16);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(20);
                    if (GUILayout.Button(currentCoroutineName, "IN Popup", GUILayout.Width(200)))
                    {
                        ShowContextOfCoroutine(coroutineIds, currentCoroutineName);
                    }

                    GUILayout.Label(countMessage);

                    if (GUILayout.Button("Open In Editor", "toolbarbutton"))
                    {
                        var scriptName = AutomatineSettings.UNITY_FOLDER_SEPARATOR + currentCoroutineName + ".cs";

                        var assetPathCanditates = AssetDatabase.GetAllAssetPaths().Where(path => path.Contains(scriptName)).ToList();
                        if (!assetPathCanditates.Any())
                        {
                            Debug.LogError("Coroutine:" + currentCoroutineName + ".cs is not found. maybe script name was changed or written in another named file.");
                            return;
                        }

                        var asset = AssetDatabase.LoadAssetAtPath(assetPathCanditates[0], typeof(TextAsset));
                        if (asset == null)
                        {
                            Debug.LogError("Coroutine: failed to open " + currentCoroutineName + ".cs.");
                            return;
                        }

                        AssetDatabase.OpenAsset(asset, 0);
                    }
                    GUILayout.Space(20);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(20);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(20);

                    scrollPosY = GUILayout.BeginScrollView(scrollPosY);

                    GUILayout.TextArea(code, AutomatineGUISettings.coroutineStyle);

                    GUILayout.EndScrollView();

                    GUILayout.Space(20);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(16);
            }
            catch { }

            switch (Event.current.type)
            {
                case EventType.MouseMove:
                    {
                        Event.current.Use();
                        break;
                    }
                case EventType.ScrollWheel:
                    {
                        Event.current.Use();
                        break;
                    }
            }

            GUI.DragWindow();
        }

        private void ShowContextOfCoroutine(List<string> coroutineNames, string coroutineName)
        {
            var menu = new GenericMenu();

            // current.
            menu.AddDisabledItem(
                new GUIContent(coroutineName)
            );

            menu.AddSeparator(string.Empty);

            // other.
            foreach (var coroutineData in coroutineNames.Select((val, index) => new { index, val }))
            {
                var coroutineIndex = coroutineData.index;
                var coroutineInfo = coroutineData.val;

                if (coroutineInfo == coroutineName) continue;

                menu.AddItem(
                    new GUIContent(coroutineIndex + ":" + coroutineInfo),
                    false,
                    () =>
                    {
                        this.code = LoadCoroutineCode(coroutineInfo);
                    }
                );
            }

            menu.ShowAsContext();
        }

        private string LoadCoroutineCode(string coroutineName)
        {
            currentCoroutineName = coroutineName;

            var scriptName = AutomatineSettings.UNITY_FOLDER_SEPARATOR + currentCoroutineName + ".cs";
            try
            {
                var assetPathCanditates = AssetDatabase.GetAllAssetPaths().Where(path => path.Contains(scriptName)).ToList();
                if (!assetPathCanditates.Any())
                {
                    Debug.LogError("Coroutine:" + currentCoroutineName + ".cs is not found. maybe script name was changed or written in another named file.");
                    return "failed to open. " + currentCoroutineName + ".cs is not found. maybe script name was changed or written in another named file.";
                }

                var codeSource = string.Empty;

                using (var sr = new StreamReader(assetPathCanditates[0]))
                {
                    codeSource = sr.ReadToEnd();
                }

                return Colorized(codeSource);
            }
            catch
            {
                return "failed to load coroutine:" + coroutineName;
            }
        }

        public string Colorized(string source)
        {
            var result = new List<string>();
            var blockComment = false;

            var lines = source.Split('\n');
            foreach (var line in lines)
            {
                if (line.Contains("/*")) blockComment = true;

                var colorozedLine = ColorizedLine(line, blockComment);

                if (line.Contains("*/")) blockComment = false;
                result.Add(colorozedLine);
            }
            return string.Join("\n", result.ToArray());
        }

        public string ColorizedLine(string line, bool isUnderBlockComment)
        {
            if (isUnderBlockComment)
            {
                return "<color=#5caa6e>" + line + "</color>";
            }

            var words = line.Split(' ');
            var lineResult = new List<string>();
            var isComment = false;

            foreach (var word in words)
            {
                var tabReplacedWord = word.Replace("	", string.Empty);

                // comment.
                if (tabReplacedWord.StartsWith("//"))
                {
                    isComment = true;
                }

                if (isComment)
                {
                    lineResult.Add("<color=#5caa6e>" + word + "</color>");
                    continue;
                }

                // keywords.
                switch (tabReplacedWord)
                {
                    case "using":
                    case "namespace":
                    case "public":
                    case "class":
                    case "partial":
                    case "new":
                    case "void":
                    case "string":
                    case "int":
                    case "double":
                    case "float":
                    case "long":
                    case "var":
                    case "while":
                    case "foreach":
                    case "in":
                    case "try":
                    case "catch":
                    case "true":
                    case "false":
                    case "yield":
                    case "return":
                    case "as":
                        {
                            lineResult.Add("<color=#1396d0>" + word + "</color>");
                            break;
                        }
                    default:
                        {
                            lineResult.Add(word);
                            break;
                        }
                }
            }

            return string.Join(" ", lineResult.ToArray());
        }
    }
}
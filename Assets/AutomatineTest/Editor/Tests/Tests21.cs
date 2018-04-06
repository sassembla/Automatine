using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Automatine;

// Test

/**
	Ruotine Description
*/
public partial class Test
{

    public void _21_0_RoutineDescriptorDirectory()
    {
        // routineのファイルを作り出す。
        var routineName = "Default_dummy0";
        var comments = new List<string> { "デフォルトルーチン、", "ここでデフォルト動作を設定する" };

        var desc = AutoDescriptor.Routine("DummyRoutineContexts <InitialParamType, UpdateParamType> : RoutineBase<InitialParamType, UpdateParamType>", routineName, comments);

        var basePath = Path.Combine(Application.dataPath, "AutomatineTest/TestGenerated/Editor");
        var outputPath = Path.Combine(basePath, "_21_0_RoutineDescriptorDirectory.cs");

        // write class file
        using (var sw = new StreamWriter(outputPath))
        {
            sw.Write(desc);
        }
    }

    public void _21_1_RoutineDescriptorFromJson()
    {
        var basePath = Path.Combine(Application.dataPath, "AutomatineTest/TestGenerated/Editor");
        var autoGenOutputPath = Path.Combine(basePath, "_21_1_RoutineDescriptorFromJson.json");

        {
            // generate routine file
            var routineDict = new RoutineData(
                "Default_dummy1",
                new List<string> { "デフォルトルーチン、", "ここでデフォルト動作を設定する" }
            );

            // jsonファイルに吐き出す
            var serialized = JsonUtility.ToJson(routineDict);

            // write json file
            using (var sw = new StreamWriter(autoGenOutputPath))
            {
                sw.Write(serialized);
            }
        }

        {
            // read json file
            var body = string.Empty;
            using (var sr = new StreamReader(autoGenOutputPath))
            {
                body = sr.ReadToEnd();
            }
            var deserialized = JsonUtility.FromJson<RoutineData>(body);

            var routineName = deserialized.info;
            var comments = deserialized.comments;

            var desc = AutoDescriptor.Routine("DummyRoutineContexts <InitialParamType, UpdateParamType> : RoutineBase<InitialParamType, UpdateParamType>", routineName, comments);

            var outputPath = Path.Combine(basePath, "_21_1_RoutineDescriptorFromJson.cs");

            // write class file
            using (var sw = new StreamWriter(outputPath))
            {
                sw.Write(desc);
            }
        }
    }
}






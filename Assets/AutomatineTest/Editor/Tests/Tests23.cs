using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Automatine;



// Test

/**
	CollectedConditions Description
*/
public partial class Test
{

    public void _23_0_CollectedConditionsDescriptorDirectory()
    {
        var conditionCategories = new List<string> { "Act", "Anim" };

        var classDesc = new NoConstructorClassDescriptor(
            new List<string> { "System" },
            new List<string> { "generated CollectedConditions by Automatine." },
            "public",
            "partial",
            "CollectedConditions_dummy0",
            () =>
            {
                var source = new CollectedConditionDefineDesc("CollectedConditions_dummy0", conditionCategories);
                return source.data;
            }
        );

        var basePath = Path.Combine(Application.dataPath, "AutomatineTest/TestGenerated/Editor");
        var outputPath = Path.Combine(basePath, "_23_0_CollectedConditionsDescriptorDirectory.cs");

        // write class file
        using (var sw = new StreamWriter(outputPath))
        {
            sw.Write(classDesc.data);
        }
    }

    public void _23_1_CollectedConditionsDescriptorFromJson()
    {

        var basePath = Path.Combine(Application.dataPath, "AutomatineTest/TestGenerated/Editor");
        var autoGenOutputPath = Path.Combine(basePath, "_23_1_CollectedConditionsDescriptorFromJson.json");

        {
            var conditionCategoriesDict = new ConditionTypeData(new List<string> { "Act", "Anim", "Hit" });

            // jsonファイルに吐き出す
            var serialized = JsonUtility.ToJson(conditionCategoriesDict);

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
            var deserialized = JsonUtility.FromJson<ConditionTypeData>(body);

            var conditionCategories = deserialized.conditionTypes;


            var desc = AutoDescriptor.CollectedConditions("DummyCollectedConditions", conditionCategories);

            var outputPath = Path.Combine(basePath, "_23_1_CollectedConditionsDescriptorFromJson.cs");

            // write class file
            using (var sw = new StreamWriter(outputPath))
            {
                sw.Write(desc);
            }
        }
    }
}





public partial class CollectedConditions_dummy0
{
    private Type[] conditions;
}

public partial class DummyCollectedConditions
{
    private Type[] conditions;
}
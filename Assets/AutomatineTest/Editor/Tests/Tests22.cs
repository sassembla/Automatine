using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Automatine;



// Test

/**
	Condition Description

	出力：押すと、jsonで出力、ファイル化
	ファイル側を変更：ツール側はコンパイル結果を元に読みだす。jsonも更新(オート)。ということで、jsonが変換仲介として入れば良さそう。
*/
public partial class Test
{

    public void _22_0_ConditionDescriptorDirectory()
    {
        // AutoConditionsのファイルを作り出す。
        var conditionName = "Act_dummy";
        var comments = new List<string>{
            "comment of this conditions here",
            "yes."
        };

        var conditionAndComments = new List<ValueAndCommentData>{
            new ValueAndCommentData("SAMPLEROUTINE", "routineのサンプル"),
            new ValueAndCommentData("P0", "comment"),
            new ValueAndCommentData("P1", "comment"),
            new ValueAndCommentData("P2", "comment"),
            new ValueAndCommentData("P3", "comment"),
            new ValueAndCommentData("SPAWN", "生成アクション"),
            new ValueAndCommentData("DEFAULT", "デフォルト状態"),
            new ValueAndCommentData("DAMAGE", "ダメージ受けてる"),
            new ValueAndCommentData("FIVEFRAME_SPAN", "5Fで終わる"),
            new ValueAndCommentData("TENFRAME_SPAN", "10Fで終わる")
        };

        var desc = AutoDescriptor.AutoConditions("DummyAutoConditions0", conditionName, comments, conditionAndComments);

        var basePath = Path.Combine(Application.dataPath, "AutomatineTest/TestGenerated/Editor");
        var outputPath = Path.Combine(basePath, "_22_0_ConditionDescriptorDirectory.cs");

        // write class file
        using (var sw = new StreamWriter(outputPath))
        {
            sw.Write(desc);
        }
    }

    public void _22_1_ConditionDescriptorFromJson()
    {
        var basePath = Path.Combine(Application.dataPath, "AutomatineTest/TestGenerated/Editor");
        var autoGenOutputPath = Path.Combine(basePath, "_22_1_ConditionDescriptorFromJson.json");

        {
            var conditionDict = new ConditionValueData(
                "Act_dummy",
                new List<string> { "comment of this conditions here", "yes." },
                new List<ValueAndCommentData>{
                    new ValueAndCommentData("SAMPLEROUTINE", "routineのサンプル"),
                    new ValueAndCommentData("P0", "comment"),
                    new ValueAndCommentData("P1", "comment"),
                    new ValueAndCommentData("P2", "comment"),
                    new ValueAndCommentData("P3", "comment"),
                    new ValueAndCommentData("SPAWN", "生成アクション"),
                    new ValueAndCommentData("DEFAULT", "デフォルト状態"),
                    new ValueAndCommentData("DAMAGE", "ダメージ受けてる"),
                    new ValueAndCommentData("FIVEFRAME_SPAN", "5Fで終わる"),
                    new ValueAndCommentData("TENFRAME_SPAN", "10Fで終わる")
                }
            );

            // jsonファイルに吐き出す
            var serialized = JsonUtility.ToJson(conditionDict);

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
            var deserialized = JsonUtility.FromJson<ConditionValueData>(body);

            var conditionName = deserialized.typeId;
            var conditionComments = deserialized.comment;
            var conditionsAndComments = deserialized.valueAndComments;

            var desc = AutoDescriptor.AutoConditions("DummyAutoConditions1", conditionName, conditionComments, conditionsAndComments);

            var outputPath = Path.Combine(basePath, "_22_1_ConditionDescriptorFromJson.cs");

            // write class file
            using (var sw = new StreamWriter(outputPath))
            {
                sw.Write(desc);
            }
        }
    }
}






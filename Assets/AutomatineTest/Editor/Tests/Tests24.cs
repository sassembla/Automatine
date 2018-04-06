using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Automatine;



// Test

/**
	Mapper Description
*/
public partial class Test
{

    public void _24_0_ChangerDescriptorDirectory()
    {

        var routeName = "TestRoute0";
        var ChangerComments = new List<string> { "this changer is for change auto from move to other." };
        var branches = new List<ChangerBranchData>();
        // branch0
        {
            var branchComments = new List<string> { "comment for branchName0" };
            var branchConditions = new List<ConditionBindData>{
                new ConditionBindData(ChangerCondKey.NOTCONTAINS, new List<ConditionTypeValueData>{new ConditionTypeValueData("Act", "SPAWN")}),
                new ConditionBindData(ChangerCondKey.CONTAINS, new List<ConditionTypeValueData>{new ConditionTypeValueData("Act", "DEFAULT")}),
                new ConditionBindData(ChangerCondKey.CONTAINSALL, new List<ConditionTypeValueData>{new ConditionTypeValueData("Act", "DEFAULT")}),
                new ConditionBindData(ChangerCondKey.NOTCONTAINSALL, new List<ConditionTypeValueData>{new ConditionTypeValueData("Act", "SPAWN")})
            };
            var branchNext = "MoveAuto";
            var branchContinue = false;
            var branchInherits = new List<string> { "Move" };

            branches.Add(new ChangerBranchData(branchComments, branchConditions, branchNext, branchContinue, branchInherits));
        }

        {
            // branch1	
            var branchComments = new List<string> { "comment for branchName1" };
            var branchConditions = new List<ConditionBindData>{
                new ConditionBindData(ChangerCondKey.CONTAINS, new List<ConditionTypeValueData>{new ConditionTypeValueData("Act", "SPAWN")})
            };
            var branchNext = string.Empty;
            var branchContinue = true;
            var branchInherits = new List<string>();

            branches.Add(new ChangerBranchData(branchComments, branchConditions, branchNext, branchContinue, branchInherits));
        }

        var finallyBranchDict = new FinallyBranchData();

        var classCommentBase = new List<string> { routeName, "generated Changer by Automatine." };
        classCommentBase.AddRange(ChangerComments);

        var classDesc = new NoConstructorClassDescriptor(
            new List<string> { "System", "Automatine" },
            classCommentBase,
            "public",
            string.Empty,
            "Changer_" + routeName + " : IAutoChanger",
            () =>
            {
                var source = new ChangerMethodDesc(routeName, branches, finallyBranchDict);
                return source.data;
            },
            () =>
            {
                var source = new ChangerShouldImplementMethodDesc(routeName, branches, finallyBranchDict);
                return source.data;
            }
        );

        var desc = classDesc.data;

        var basePath = Path.Combine(Application.dataPath, "AutomatineTest/TestGenerated/Editor");
        var outputPath = Path.Combine(basePath, "_24_0_ChangerDescriptorDirectory.cs");

        // write class file
        using (var sw = new StreamWriter(outputPath))
        {
            sw.Write(desc);
        }
    }

    public void _24_1_ChangerDescriptorFromJson()
    {
        var basePath = Path.Combine(Application.dataPath, "AutomatineTest/TestGenerated/Editor");
        var autoGenOutputPath = Path.Combine(basePath, "_24_1_ChangerDescriptorFromJson.json");

        {
            // write to json
            var classGenParam = new ChangerData(
                "TestRouteId1",
                "TestRoute1",
                "parentAutoId",
                new List<string> { "this changer is for change move or not." },
                new List<BranchData>{
                    new BranchData(
                        new List<string>{"comment for branchName0"},
                        new List<ConditionBindData>{
                            new ConditionBindData(
                                ChangerCondKey.NOTCONTAINS,
                                new List<ConditionTypeValueData>{
                                    new ConditionTypeValueData("Act", "SPAWN")
                                }
                            ),
                            new ConditionBindData(
                                ChangerCondKey.CONTAINS,
                                new List<ConditionTypeValueData>{
                                    new ConditionTypeValueData("Act", "DEFAULT")
                                }
                            ),
                            new ConditionBindData(
                                ChangerCondKey.CONTAINSALL,
                                new List<ConditionTypeValueData>{
                                    new ConditionTypeValueData("Act", "DEFAULT")
                                }
                            ),
                            new ConditionBindData(
                                ChangerCondKey.NOTCONTAINSALL,
                                new List<ConditionTypeValueData>{
                                    new ConditionTypeValueData("Act", "SPAWN")
                                }
                            )
                        },
                        "MoveAuto",
                        false,
                        new List<string>{"Move"}
                    ),

                    new BranchData(
                        new List<string>{"comment for branchName1"},
                        new List<ConditionBindData>{
                            new ConditionBindData(ChangerCondKey.CONTAINS, new List<ConditionTypeValueData>{new ConditionTypeValueData("Act", "SPAWN")})
                        },
                        string.Empty,
                        true,
                        new List<string>()
                    )
                },
                new FinallyBranchData()
            );

            var serialized = JsonUtility.ToJson(classGenParam);


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
            var deserialized = JsonUtility.FromJson<ChangerData>(body);

            var changerName = deserialized.changerName;
            var commentLines = deserialized.comments;
            var branches = deserialized.branchs;
            var finallyBranch = deserialized.finallyBranch;

            var desc = AutoDescriptor.Changer(changerName, commentLines, branches, finallyBranch);

            var outputPath = Path.Combine(basePath, "_24_1_MapDescriptorFromJson.cs");

            // write class file
            using (var sw = new StreamWriter(outputPath))
            {
                sw.Write(desc);
            }
        }
    }

    public void _24_2_ChangerDescriptorFromJsonWithFinally()
    {
        var basePath = Path.Combine(Application.dataPath, "AutomatineTest/TestGenerated/Editor");
        var autoGenOutputPath = Path.Combine(basePath, "_24_2_ChangerDescriptorFromJsonWithFinally.json");

        {
            // write to json.
            var classGenParam = new ChangerData(
                "TestRouteId2",
                "TestRoute2",
                "parentAutoId",
                new List<string> { "this changer is for change move or not." },
                new List<BranchData>(),
                new FinallyBranchData(
                    "MoveAuto",
                    new List<string> { "Move" }
                )
            );

            var serialized = JsonUtility.ToJson(classGenParam);


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
            var deserialized = JsonUtility.FromJson<ChangerData>(body);

            var changerName = deserialized.changerName;
            var commentLines = deserialized.comments;
            var branches = deserialized.branchs;
            var finallyBranch = deserialized.finallyBranch;

            var desc = AutoDescriptor.Changer(changerName, commentLines, branches, finallyBranch);

            var outputPath = Path.Combine(basePath, "_24_2_ChangerDescriptorFromJsonWithFinally.cs");

            // write class file
            using (var sw = new StreamWriter(outputPath))
            {
                sw.Write(desc);
            }
        }
    }

    public void _24_3_ChangerDescriptorFromJsonWithBranchesAndFinally()
    {
        var basePath = Path.Combine(Application.dataPath, "AutomatineTest/TestGenerated/Editor");
        var autoGenOutputPath = Path.Combine(basePath, "_24_3_ChangerDescriptorFromJsonWithBranchesAndFinally.json");

        {
            // write to json.
            var classGenParam = new ChangerData(
                "TestRouteId3",
                "TestRoute3",
                "parentAutoId",
                new List<string> { "this changer is for change move or not." },
                new List<BranchData>{
                    new BranchData(
                        new List<string>{"comment for branchName0"},
                        new List<ConditionBindData>{
                            new ConditionBindData(ChangerCondKey.CONTAINS, new List<ConditionTypeValueData>{new ConditionTypeValueData("Act", "SPAWN")})
                        },
                        "MoveAuto",
                        false,
                        new List<string>{"Move"}
                    ),
                    new BranchData(
                        new List<string>{"comment for branchName1"},
                        new List<ConditionBindData>{
                            new ConditionBindData(ChangerCondKey.CONTAINS, new List<ConditionTypeValueData>{new ConditionTypeValueData("Act", "SPAWN")})
                        },
                        string.Empty,
                        true,
                        new List<string>()
                    )
                },
                new FinallyBranchData(
                    "MoveAuto",
                    new List<string> { "Move" }
                )
            );

            var serialized = JsonUtility.ToJson(classGenParam);


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
            var deserialized = JsonUtility.FromJson<ChangerData>(body);

            var changerName = deserialized.changerName;
            var commentLines = deserialized.comments;
            var branches = deserialized.branchs;
            var finallyBranch = deserialized.finallyBranch;

            var desc = AutoDescriptor.Changer(changerName, commentLines, branches, finallyBranch);

            var outputPath = Path.Combine(basePath, "_24_3_ChangerDescriptorFromJsonWithBranchesAndFinally.cs");

            // write class file
            using (var sw = new StreamWriter(outputPath))
            {
                sw.Write(desc);
            }
        }
    }
}






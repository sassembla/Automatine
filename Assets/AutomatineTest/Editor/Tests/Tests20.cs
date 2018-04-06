using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Automatine;


// Test

/**
	Description
*/
public partial class Test
{

    public void _20_0_AutoGeneratorDescriptorDirectory()
    {
        var routinesDesc0 = new RoutineDesc("Default");
        var routinesDesc1 = new RoutineDesc("Default", "_Infinity");
        var tackDesc0 = new TackDesc("tackInfo_0", 0, 1, "Act", "DEFAULT", new List<RoutineDesc> { routinesDesc0 });
        var tackDesc1 = new TackDesc("tackInfo_1", 0, 1, "Anim", "DEFAULT", new List<RoutineDesc> { routinesDesc1 });
        var tackDescs = new List<TackDesc>();
        tackDescs.Add(tackDesc0);
        tackDescs.Add(tackDesc1);
        var timelineDesc0 = new TimelineDesc("timelineInfo_0", tackDescs);
        var timelineDesc1 = new TimelineDesc("timelineInfo_1", tackDescs);
        var timelineDescs = new List<TimelineDesc>();
        timelineDescs.Add(timelineDesc0);
        timelineDescs.Add(timelineDesc1);
        var autoDesc = new AutoGeneratorMethodDesc("Auto_Default0", "autoInfo", timelineDescs);

        var classDesc = new ConstructorClassDescriptor(
            new List<string> { "System", "System.Collections", "Automatine" },
            new List<string> { "generated AutoGenerator by Automatine." },
            "public",
            "Auto_Default0",
            "<InitialParamType, UpdateParamType>",
            () =>
            {
                return autoDesc.SetIdThenReturnData();
            }
        );

        var basePath = Path.Combine(Application.dataPath, "AutomatineTest/TestGenerated/Editor");
        var outputPath = Path.Combine(basePath, "_20_0_AutoGeneratorDescriptorDirectory.cs");

        // write class file
        using (var sw = new StreamWriter(outputPath))
        {
            sw.Write(classDesc.data);
        }
    }

    public void _20_1_AutoGeneratorDescriptorFromJson()
    {
        var basePath = Path.Combine(Application.dataPath, "AutomatineTest/TestGenerated/Editor");
        var autoGenOutputPath = Path.Combine(basePath, "_20_1_AutoGeneratorDescriptorFromJson.json");

        {
            // write to json file.
            var classGenParam = new AutoData(
                "2016/01/06 16:55:06",
                "Auto_Default1",
                "autoInfo",
                new List<string> { "複数行のコメント", "描きたいですね。" },

                new List<TimelineData>{
                    new TimelineData(
                        "2016/01/06 16:55:06",
                        "timelineInfo_0",
                        new List<TackData>{
                            new TackData(
                                "id",
                                "tackInfo_0",
                                0,
                                1,
                                "Act",
                                "DEFAULT",
                                new List<string>{"Default", "_Infinity"}
                            ),
                            new TackData(
                                "id0",
                                "tackInfo_1",
                                1,
                                10,
                                "Act",
                                "P0",
                                new List<string>{"Default"}
                            )
                        }
                    ),
                    new TimelineData(
                        "2016/01/06 16:55:86",
                        "timelineInfo_1",
                        new List<TackData>{
                            new TackData(
                                "id1",
                                "tackInfo_2",
                                0,
                                20,
                                "Anim",
                                "DEFAULT",
                                new List<string>{"Default"}
                            ),
                            new TackData(
                                "id2",
                                "tackInfo_3",
                                20,
                                100,
                                "Anim",
                                "SPAWN",
                                new List<string>{"Default"}
                            )
                        }
                    )
                }
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
            var deserialized = JsonUtility.FromJson<AutoData>(body);

            var desc = AutoDescriptor.Auto("<InitialParamType, UpdateParamType>", deserialized);


            var outputPath = Path.Combine(basePath, "_20_1_AutoGeneratorDescriptorFromJson.cs");

            // write class file
            using (var sw = new StreamWriter(outputPath))
            {
                sw.Write(desc);
            }
        }

    }

    public void _20_2_AutoGeneratorDescriptorOfUntypedTack()
    {
        var basePath = Path.Combine(Application.dataPath, "AutomatineTest/TestGenerated/Editor");
        var autoGenOutputPath = Path.Combine(basePath, "_20_2_AutoGeneratorDescriptorOfUntypedTack.json");

        {
            // write to json file.
            var classGenParam = new AutoData(
                "2016/01/06 16:55:06",
                "Auto_Default2",
                "autoInfo",
                new List<string> { "複数行のコメント", "描きたいですね。" },

                new List<TimelineData>{
                    new TimelineData(
                        "2016/01/06 16:55:06",
                        "timelineInfo_0",
                        new List<TackData>{
                            new TackData(
                                "id",
                                "tackInfo_0",
                                0,
                                1
                            ),
                            new TackData(
                                "id0",
                                "tackInfo_1",
                                1,
                                10,
                                new List<string>{"Default"}
                            )
                        }
                    )
                }
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
            var deserialized = JsonUtility.FromJson<AutoData>(body);

            var desc = AutoDescriptor.Auto("<InitialParamType, UpdateParamType>", deserialized);


            var outputPath = Path.Combine(basePath, "_20_2_AutoGeneratorDescriptorOfUntypedTack.cs");

            // write class file
            using (var sw = new StreamWriter(outputPath))
            {
                sw.Write(desc);
            }
        }
    }

    public void _20_3_AutoGeneratorDescriptorOfEmptyTimeline()
    {
        var basePath = Path.Combine(Application.dataPath, "AutomatineTest/TestGenerated/Editor");
        var autoGenOutputPath = Path.Combine(basePath, "_20_3_AutoGeneratorDescriptorOfEmptyTimeline.json");

        {
            // write to json file.
            var classGenParam = new AutoData(
                "2016/01/06 16:55:06",
                "Auto_Default3",
                "autoInfo",
                new List<string> { "複数行のコメント", "描きたいですね。" },

                new List<TimelineData>{
                    new TimelineData(
                        "2016/01/06 16:55:06",
                        "timelineInfo_0"
                    )
                }
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
            var deserialized = JsonUtility.FromJson<AutoData>(body);

            var desc = AutoDescriptor.Auto("<InitialParamType, UpdateParamType>", deserialized);

            var outputPath = Path.Combine(basePath, "_20_3_AutoGeneratorDescriptorOfEmptyTimeline.cs");

            // write class file
            using (var sw = new StreamWriter(outputPath))
            {
                sw.Write(desc);
            }
        }
    }

}






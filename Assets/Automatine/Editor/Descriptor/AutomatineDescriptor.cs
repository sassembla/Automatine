
using System;
using System.Linq;
using System.Collections.Generic;


namespace Automatine
{

    public class ChangerCondKey
    {
        public const string CONTAINS = "Contains";
        public const string NOTCONTAINS = "NotContains";
        public const string CONTAINSALL = "ContainsAll";
        public const string NOTCONTAINSALL = "NotContainsAll";
    }



    public class RoutineDesc
    {
        public readonly string data;

        public readonly bool isNotEmpty;

        public RoutineDesc(params string[] routineNames)
        {
            var strs = new List<string>();

            var empty = false;
            foreach (var routineName in routineNames)
            {
                var routine = new List<string>{
    "					() => {",
    "						var c = new RoutineContexts<InitialParamType, UpdateParamType>();",
    "						c.rContext = ",
    "							c." + routineName,
    "						;",
    "						return c;",
    "					}"
                };

                var transformed = string.Join("\n", routine.ToArray());

                strs.Add(transformed);
                empty = false;
            }

            if (empty) isNotEmpty = false;
            else isNotEmpty = true;

            data = string.Join(",\n", strs.ToArray());
        }
    }

    public class TackDesc
    {
        public readonly string data;
        public TackDesc(string tackInfo, int start, int span, string conditionType, string conditionValue, List<RoutineDesc> routineDescs)
        {
            var str = new List<string>{
    "			new Tack<InitialParamType, UpdateParamType>(",
    "				\"" + tackInfo + "\",",
    "				" + start + ",",
    "				" + span
            };

            if (!string.IsNullOrEmpty(conditionType) && !string.IsNullOrEmpty(conditionValue))
            {
                var typeDesc = new List<string>() {
    "			   ,",
    "				AutoConditions." + string.Join(".", new string [] {conditionType, conditionValue})
                };
                str.AddRange(typeDesc);
            }

            if (routineDescs.Where(routineDesc => routineDesc.isNotEmpty).Any())
            {
                var routineDescStrs = new List<string>{
    "			   ,",
                    string.Join(",\n", routineDescs.Select(desc => desc.data).ToArray())
                };

                str.AddRange(routineDescStrs);
            };

            str.Add(
    "			)"
            );

            data = string.Join("\n", str.ToArray());
        }
    }

    public class TimelineDesc
    {
        public readonly string data;
        public TimelineDesc(string timelineInfo, List<TackDesc> tackDescs)
        {
            var str = new List<string>{
    "		new Timeline<InitialParamType, UpdateParamType>(",
    "			\"" + timelineInfo + "\""
            };

            if (tackDescs.Any())
            {
                var tackDeskList = new List<string>{
    "			,",
                    string.Join(",\n", tackDescs.Select(desc => desc.data).ToArray())
                };
                str.AddRange(tackDeskList);
            }

            str.Add(
    "		)"
            );

            data = string.Join("\n", str.ToArray());
        }
    }

    public class AutoGeneratorMethodDesc
    {
        private readonly string autoName;
        private readonly string autoInfo;
        private readonly List<TimelineDesc> timelineDescs;
        public AutoGeneratorMethodDesc(string autoName, string autoInfo, List<TimelineDesc> timelineDescs)
        {
            this.autoName = autoName;
            this.autoInfo = autoInfo;
            this.timelineDescs = timelineDescs;
        }

        public string SetIdThenReturnData()
        {
            var str = new List<string>{
"	public " + autoName + " (int frame, InitialParamType initialParam) : base (",
"		\"" + autoInfo + "\",",
"		frame,",
"		initialParam"
            };

            if (timelineDescs.Any())
            {
                str.AddRange(
                    new List<string> {
"		,",
                        string.Join(",\n", timelineDescs.Select(desc => desc.data).ToArray()),
                    }
                );
            }

            str.AddRange(
                new List<string> {
"	){}"
                }
            );

            return string.Join("\n", str.ToArray());
        }
    }

    public class AutoConditionsDesc
    {
        public readonly string data;
        public AutoConditionsDesc(string conditionName, List<ValueAndCommentData> conditionAndComments)
        {
            var str = new List<string>{
    "	public enum " + conditionName + " : int {"
            };

            foreach (var conditionAndComment in conditionAndComments)
            {
                str.Add(
    "		" + conditionAndComment.valueName + ", // " + conditionAndComment.comment
                );
            }
            str.Add(
    "	}"
            );

            data = string.Join("\n", str.ToArray());
        }
    }

    public class CollectedConditionDefineDesc
    {
        public readonly string data;
        public CollectedConditionDefineDesc(string constructorName, List<string> conditionCategories)
        {
            var str = new List<string>{
    "	private " + constructorName + " () {",
    "		conditions = new Type [] {",
            };

            foreach (var conditionCategory in conditionCategories)
            {
                str.Add(
    "			typeof(AutoConditions." + conditionCategory + "),"
                );
            }

            str.AddRange(
                new List<string>{
    "		};",
    "	}"
                }
            );

            data = string.Join("\n", str.ToArray());
        }
    }

    public class RoutineMethodDesc
    {
        public readonly string data;
        public RoutineMethodDesc(string routineName)
        {
            var str = new List<string>{
    "	",
    "	public IEnumerator " + routineName + " (InitialParamType initialParam) {",
    "		",
    "		// please modify this code.",
    "		",
    "		// var yourInitialParameter = initialParam as YourInitialParameterClass;",
    "		",
    "		// var currentFrame = this.frame;",
    "		// var currentLoadCount = this.loadCount;",
    "		// var yourUpdateParameter = this.updateParam as YourUpdateParameterClass;",
    "		",
    "		while (true) {",
    "			yield return null;",
    "		}",
    "		",
    "	}",
    "	",
            };

            data = string.Join("\n", str.ToArray());
        }
    }

    public class ChangerBranchDesc
    {
        public readonly string data;
        public ChangerBranchDesc(List<string> branchComments, List<ConditionBindData> branchConditions, string branchNext, bool branchContinue, List<string> branchInherits)
        {
            var str = new List<string>();

            str.Add(
    "		/*"
            );

            str.AddRange(branchComments.Select(comment =>
    "			" + comment).ToList()
            );

            str.Add(
    "		*/"
            );

            str.Add(
    "		if ("
            );

            var conditionLeafs = new List<string>();
            foreach (var branchCondition in branchConditions)
            {
                var branchCategory = branchCondition.combinationKind;
                var branchValues = string.Join(", ", branchCondition.combinations.Select(typeValue => "AutoConditions." + typeValue.type + "." + typeValue.val).ToArray());

                conditionLeafs.Add(
    "			ConditionGateway." + branchCategory + "(conditions, " + branchValues + ")"
                );

            }

            str.Add(string.Join(" && \n", conditionLeafs.ToArray()));

            str.Add(
    "		) {"
            );


            if (branchContinue)
            {
                str.Add(
    "			return baseAuto;"
                );
            }
            else
            {
                str.AddRange(
                    new List<string>{
    "			var newAuto = new ",
    "				" + branchNext,
    "				<InitialParamType, UpdateParamType>(frame, fixedContext);"
                    }
                );

                if (branchInherits.Any())
                {
                    var branchInheritsWithClass = branchInherits.Select(type => "typeof(AutoConditions." + type + ")").ToArray();
                    var inheritConditions = string.Join(", ", branchInheritsWithClass);
                    str.AddRange(
                        new List<string>{
    "			newAuto.InheritTimelines(baseAuto.ExportTimelines(new Type[] {",
    "				" + inheritConditions,
    "			}));"
                        }
                    );
                }

                str.Add(
    "			return newAuto;"
                );
            }

            str.AddRange(
                new List<string>{
    "		}",
    "		"
                }
            );

            data = string.Join("\n", str.ToArray());
        }
    }


    public class EffectiveCheckerBranchDesc
    {
        public readonly string data;
        public EffectiveCheckerBranchDesc(List<string> branchComments, List<ConditionBindData> branchConditions)
        {
            var str = new List<string>();

            str.Add(
    "		/*"
            );

            str.AddRange(branchComments.Select(comment =>
    "			" + comment).ToList()
            );

            str.Add(
    "		*/"
            );

            str.Add(
    "		if ("
            );

            var conditionLeafs = new List<string>();
            foreach (var branchCondition in branchConditions)
            {
                var branchCategory = branchCondition.combinationKind;
                var branchValues = string.Join(", ", branchCondition.combinations.Select(typeValue => "AutoConditions." + typeValue.type + "." + typeValue.val).ToArray());

                conditionLeafs.Add(
    "			ConditionGateway." + branchCategory + "(conditions, " + branchValues + ")"
                );
            }

            str.Add(string.Join(" && \n", conditionLeafs.ToArray()));

            str.AddRange(
                new List<string>{
    "		) {",
    "		   return true;",
    "		}",
    "		"
                }
            );

            data = string.Join("\n", str.ToArray());
        }
    }


    public struct ChangerBranchData
    {
        public List<string> branchComments;
        public List<ConditionBindData> branchConditionBinds;
        public string branchNext;
        public bool branchContinue;
        public List<string> branchInherits;

        public ChangerBranchData(List<string> branchComments, List<ConditionBindData> branchConditionBinds, string branchNext, bool branchContinue, List<string> branchInherits)
        {
            this.branchComments = branchComments;
            this.branchConditionBinds = branchConditionBinds;
            this.branchNext = branchNext;
            this.branchContinue = branchContinue;
            this.branchInherits = branchInherits;
        }

        public ChangerBranchData(BranchData branchDict)
        {
            this.branchComments = branchDict.comments;
            this.branchConditionBinds = branchDict.conditonBinds;
            this.branchNext = branchDict.nextAutoId;
            this.branchContinue = branchDict.isContinue;
            this.branchInherits = branchDict.inheritTimelineConditions;
        }
    }


    public class ChangerMethodDesc
    {
        public readonly string data;
        public ChangerMethodDesc(string changerName, List<ChangerBranchData> brancheSources, FinallyBranchData finallyBranch)
        {
            var changerBranches = GenerateChangerBranchDescs(brancheSources);

            // add Changer method.
            var str = new List<string>{
    "	public static Auto<InitialParamType, UpdateParamType> ",
    "		" + changerName,
    "		<InitialParamType, UpdateParamType>",
    "		(Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext)",
    "	{"
            };

            if (changerBranches.Any())
            {
                str.AddRange(
                    new List<string> {
        "		var conditions = baseAuto.Conditions();",
        "		"
                    }
                );
            }

            var branchesDesc = changerBranches.Select(branch => branch.data).ToList();
            str.AddRange(branchesDesc);

            if (finallyBranch.IsExists())
            {
                var finallyAuto = finallyBranch.finallyAutoId;
                var finallyInherits = finallyBranch.inheritTimelineConditions;

                str.AddRange(
                    new List<string>{
    "		var finallyAuto = new ",
    "			" + finallyAuto,
    "			<InitialParamType, UpdateParamType>(frame, fixedContext);"
                    }
                );

                if (finallyInherits.Any())
                {
                    var inheritsWithClass = finallyInherits.Select(inheritType => "typeof(AutoConditions." + inheritType + ")").ToArray();
                    var inheritConditions = string.Join(", ", inheritsWithClass);
                    str.AddRange(
                        new List<string>{
    "			finallyAuto.InheritTimelines(baseAuto.ExportTimelines(new Type[] {",
    "				" + inheritConditions,
    "			}));"
                        }
                    );
                }

                str.Add(
    "		return finallyAuto;"
                );

            }
            else
            {
                str.Add(
    "		return baseAuto;"
                );
            }

            str.Add(
    "	}"
            );


            // add empty line.
            str.Add(string.Empty);

            var effectiveCheckerBrancheDescs = GenerateEffectiveCheckBranchDescs(brancheSources);

            /*
				add IsEffective method.
			*/
            str.Add(
    "	public static bool IsEffectiveChanger<InitialParamType, UpdateParamType> (Auto<InitialParamType, UpdateParamType> baseAuto) {"
            );

            // add branch condition.
            if (effectiveCheckerBrancheDescs.Any())
            {
                str.AddRange(
                    new List<string> {
        "		var conditions = baseAuto.Conditions();",
        "		"
                    }
                );

                foreach (var effectiveCheckerBrancheDesc in effectiveCheckerBrancheDescs)
                {
                    str.Add(effectiveCheckerBrancheDesc.data);
                }
            }

            if (finallyBranch.IsExists())
            {
                str.AddRange(
                    new List<string> {
    "	   // finally is ON.",
    "		return true;"
                    }
                );
            }
            else
            {
                str.Add(
    "		return false;"
                );
            }

            str.Add(
    "	}"
            );

            data = string.Join("\n", str.ToArray());
        }

        private List<ChangerBranchDesc> GenerateChangerBranchDescs(List<ChangerBranchData> brancheSources)
        {
            var branches = new List<ChangerBranchDesc>();

            foreach (var brancheSource in brancheSources)
            {
                branches.Add(new ChangerBranchDesc(brancheSource.branchComments, brancheSource.branchConditionBinds, brancheSource.branchNext, brancheSource.branchContinue, brancheSource.branchInherits));
            }
            return branches;
        }

        public static List<EffectiveCheckerBranchDesc> GenerateEffectiveCheckBranchDescs(List<ChangerBranchData> brancheSources)
        {
            var branches = new List<EffectiveCheckerBranchDesc>();

            foreach (var brancheSource in brancheSources)
            {
                branches.Add(new EffectiveCheckerBranchDesc(brancheSource.branchComments, brancheSource.branchConditionBinds));
            }
            return branches;
        }
    }

    public class ChangerShouldImplementMethodDesc
    {
        public readonly string data;
        public ChangerShouldImplementMethodDesc(string changerName, List<ChangerBranchData> brancheSources, FinallyBranchData finallyBranch)
        {
            var str = new List<string>{
				// add empty line.
				string.Empty,
    "	public Func<Auto<InitialParamType, UpdateParamType>, int, InitialParamType, Auto<InitialParamType, UpdateParamType>> Changer<InitialParamType, UpdateParamType> () {",
    "		return delegate (Auto<InitialParamType, UpdateParamType> auto, int frame, InitialParamType fixedContext) {",
    "			return " + changerName + "(auto, frame, fixedContext);",
    "		};",
    "	}",
    "",
    "	public string ChangerName () {",
    "		return \"" + changerName + "\";",
    "	}",
                string.Empty
            };


            /*
				add Effective checker.
			*/
            var effectiveCheckerBrancheDescs = ChangerMethodDesc.GenerateEffectiveCheckBranchDescs(brancheSources);

            str.Add(
    "	public bool IsEffective<InitialParamType, UpdateParamType> (Auto<InitialParamType, UpdateParamType> baseAuto) {"
            );

            // add branch condition.
            if (effectiveCheckerBrancheDescs.Any())
            {
                str.AddRange(
                    new List<string> {
        "		var conditions = baseAuto.Conditions();",
        "		"
                    }
                );

                foreach (var effectiveCheckerBrancheDesc in effectiveCheckerBrancheDescs)
                {
                    str.Add(effectiveCheckerBrancheDesc.data);
                }
            }

            if (finallyBranch.IsExists())
            {
                str.AddRange(
                    new List<string> {
    "	   // finally is ON.",
    "		return true;"
                    }
                );
            }
            else
            {
                str.Add(
    "		return false;"
                );
            }

            str.Add(
    "	}"
            );

            data = string.Join("\n", str.ToArray());
        }
    }

    public class NoConstructorClassDescriptor
    {
        public readonly string data;
        public NoConstructorClassDescriptor(List<string> usings, List<string> headerComments, string visibility, string attribute, string className, params Func<string>[] methodGens)
        {
            var str = new List<string>();

            // add using x n
            str.AddRange(usings.Select(use => "using " + use + ";").ToList());

            // add header comment
            str.Add("/*");
            str.AddRange(headerComments.Select(comment => "	" + comment).ToList());
            str.Add("*/");

            // add class header
            str.Add(visibility + " " + attribute + " class " + className + " {");

            foreach (var methodGen in methodGens)
            {
                str.Add(methodGen());
            }

            str.Add("}");

            data = string.Join("\n", str.ToArray());
        }
    }

    public class ConstructorClassDescriptor
    {
        public readonly string data;
        public ConstructorClassDescriptor(List<string> usings, List<string> headerComments, string visibility, string className, string genericParams, params Func<string>[] constrouctorContentGens)
        {
            var str = new List<string>();

            // add using x n
            str.AddRange(usings.Select(use => "using " + use + ";").ToList());

            // add header comment
            str.Add("/*");
            str.AddRange(headerComments.Select(comment => "	" + comment).ToList());
            str.Add("*/");

            // add class header
            str.Add(visibility + " class " + className + " " + genericParams + " : Auto " + genericParams + " {");

            foreach (var constructorContentGen in constrouctorContentGens)
            {
                str.Add(constructorContentGen());
            }

            str.Add("}");

            data = string.Join("\n", str.ToArray());
        }
    }

    public class AutoDescriptor
    {

        public static string AutoConditions(string className, string conditionName, List<string> comments, List<ValueAndCommentData> conditionAndComments)
        {
            var commentLines = new List<string> { conditionName, "generated AutoConditions by Automatine." };
            commentLines.AddRange(comments);

            var classDesc = new NoConstructorClassDescriptor(
                new List<string>(),
                commentLines,
                "public",
                "partial",
                className,
                () =>
                {
                    var source = new AutoConditionsDesc(conditionName, conditionAndComments);
                    return source.data;
                }
            );
            return classDesc.data;
        }


        public static string CollectedConditions(string className, List<string> conditionCategories)
        {
            var classDesc = new NoConstructorClassDescriptor(
                new List<string> { "System" },
                new List<string> { "generated CollectedConditions by Automatine." },
                "public",
                "partial",
                className,
                () =>
                {
                    var source = new CollectedConditionDefineDesc(className, conditionCategories);
                    return source.data;
                }
            );
            return classDesc.data;
        }


        public static string Auto(string genericParams, AutoData autoData)
        {
            var autoName = autoData.name;
            var userComments = autoData.comments;

            var commentLines = new List<string> { autoName, "generated Auto by Automatine." };
            commentLines.AddRange(userComments);

            var classDesc = new ConstructorClassDescriptor(
                new List<string> { "System", "System.Collections", "Automatine" },
                commentLines,
                "public",
                autoName,
                genericParams,
                () =>
                {
                    var autoInfo = autoData.info;
                    var timelineDescsSourceList = autoData.timelines;

                    var timelineDescs = new List<TimelineDesc>();

                    foreach (var currentTimelineDescSource in timelineDescsSourceList)
                    {
                        var tlInfo = currentTimelineDescSource.info;
                        var tlTacksSource = currentTimelineDescSource.tacks;

                        var tlTacks = new List<TackDesc>();
                        foreach (var currentTackDescSource in tlTacksSource)
                        {

                            var tackInfo = currentTackDescSource.info;
                            var tackStart = currentTackDescSource.start;
                            var tackSpan = currentTackDescSource.span;
                            var tackConditionType = currentTackDescSource.conditionType;
                            var tackConditionValue = currentTackDescSource.conditionValue;
                            var tackRoutinesSource = currentTackDescSource.routineIds;

                            var tackRoutines = new List<RoutineDesc>();
                            foreach (var routine in tackRoutinesSource)
                            {
                                var RoutineDesc = new RoutineDesc(routine);
                                tackRoutines.Add(RoutineDesc);
                            }

                            var tackDesc = new TackDesc(tackInfo, tackStart, tackSpan, tackConditionType, tackConditionValue, tackRoutines);

                            tlTacks.Add(tackDesc);
                        }

                        var t = new TimelineDesc(
                            tlInfo,
                            tlTacks
                        );
                        timelineDescs.Add(t);
                    }

                    var autoDesc = new AutoGeneratorMethodDesc(
                        autoName,
                        autoInfo,
                        timelineDescs
                    );

                    return autoDesc.SetIdThenReturnData();
                }
            );
            return classDesc.data;
        }

        public static string Routine(string className, string routineName, List<string> comments)
        {
            var commentLines = new List<string> { routineName, "generated Routine by Automatine." };
            commentLines.AddRange(comments);
            var classDesc = new NoConstructorClassDescriptor(
                new List<string> { "System", "System.Collections", "Automatine" },
                commentLines,
                "public",
                "partial",
                className,
                () =>
                {
                    var source = new RoutineMethodDesc(routineName);
                    return source.data;
                }
            );
            return classDesc.data;
        }

        public static string Changer(string changerName, List<string> routingComments, List<BranchData> brancheSourcesList, FinallyBranchData finallyBranch)
        {
            var classCommentBase = new List<string> { changerName, "generated Changer by Automatine." };
            classCommentBase.AddRange(routingComments);

            var brancheSources = new List<ChangerBranchData>();
            foreach (var branch in brancheSourcesList)
            {
                brancheSources.Add(new ChangerBranchData(branch));
            }

            var classDesc = new NoConstructorClassDescriptor(
                new List<string> { "System", "Automatine" },
                classCommentBase,
                "public",
                string.Empty,
                AutomatineDefinitions.Changer.CLASS_HEADER + changerName + " : " + AutomatineDefinitions.Changer.INTERFACE_NAME,
                () =>
                {
                    var source = new ChangerMethodDesc(changerName, brancheSources, finallyBranch);
                    return source.data;
                },
                () =>
                {
                    var source = new ChangerShouldImplementMethodDesc(changerName, brancheSources, finallyBranch);
                    return source.data;
                }
            );

            return classDesc.data;
        }
    }
}
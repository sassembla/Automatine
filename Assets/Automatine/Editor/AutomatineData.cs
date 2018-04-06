using System;
using System.Collections.Generic;

namespace Automatine
{
    [Serializable]
    public class AutomatineData
    {
        public string lastModified;

        public List<AutoData> autoDatas;

        public List<ChangersInAuto> changersDatas;

        public List<ConditionData> conditionsDatas;

        public string lastActiveAutoId;

        public float lastScrolledPosX;
        public float lastScrolledPosY;


        public AutomatineData() { }

        public AutomatineData(
            string lastModified,
            List<AutoData> autoDatas,
            List<ChangersInAuto> changersDatas,
            List<ConditionData> conditionsDatas,
            string lastActiveAutoId,
            float lastScrolledPosX,
            float lastScrolledPosY
        )
        {
            this.lastModified = lastModified;
            this.autoDatas = autoDatas;
            this.changersDatas = changersDatas;
            this.conditionsDatas = conditionsDatas;
            this.lastActiveAutoId = lastActiveAutoId;
            this.lastScrolledPosX = lastScrolledPosX;
            this.lastScrolledPosY = lastScrolledPosY;
        }
    }


    [Serializable]
    public class AutoData
    {
        public string autoId;
        public string name;
        public string info;
        public List<string> comments;
        public List<TimelineData> timelines;

        public AutoData(string autoId, string name, string info, List<string> comments, List<TimelineData> timelines)
        {
            this.autoId = autoId;
            this.name = name;
            this.info = info;
            this.comments = comments;
            this.timelines = timelines;
        }

        /**
			convert Editor automatine data to runtime auto data.
		*/
        public RuntimeAutoData GetRuntimeAutoData(List<RuntimeChangerData> rootChangers, List<RuntimeChangerData> changers)
        {
            var runtimeTimelineDatas = new List<RuntimeTimelineData>();
            foreach (var timelineData in timelines)
            {
                var runtimeTacks = new List<RuntimeTackData>();
                foreach (var tack in timelineData.tacks)
                {
                    var runtimeRoutines = new List<RuntimeRoutineData>();
                    foreach (var routineId in tack.routineIds) runtimeRoutines.Add(new RuntimeRoutineData(routineId));

                    runtimeTacks.Add(new RuntimeTackData(tack.start, tack.span, tack.conditionType, tack.conditionValue, runtimeRoutines));
                }
                runtimeTimelineDatas.Add(new RuntimeTimelineData(runtimeTacks));
            }

            return new RuntimeAutoData(rootChangers, changers, runtimeTimelineDatas);
        }
    }



    [Serializable]
    public class TimelineData
    {
        public string timelineId;
        public string info;
        public List<TackData> tacks;

        public TimelineData(string timelineId, string info, List<TackData> tacks)
        {
            this.timelineId = timelineId;
            this.info = info;
            this.tacks = tacks;
        }

        public TimelineData(string timelineId, string info)
        {
            this.timelineId = timelineId;
            this.info = info;
            this.tacks = new List<TackData>();
        }
    }


    [Serializable]
    public class TackData
    {
        public string id;
        public string info;
        public int start;
        public int span;
        public string conditionType;
        public string conditionValue;
        public List<string> routineIds;

        public TackData(string id, string info, int start, int span, string conditionType, string conditionValue, List<string> routineIds)
        {
            this.id = id;
            this.info = info;
            this.start = start;
            this.span = span;
            this.conditionType = conditionType;
            this.conditionValue = conditionValue;
            this.routineIds = routineIds;
        }

        public TackData(string id, string info, int start, int span, List<string> routineIds)
        {
            this.id = id;
            this.info = info;
            this.start = start;
            this.span = span;
            this.conditionType = string.Empty;
            this.conditionValue = string.Empty;
            this.routineIds = routineIds;
        }

        public TackData(string id, string info, int start, int span)
        {
            this.id = id;
            this.info = info;
            this.start = start;
            this.span = span;
            this.conditionType = string.Empty;
            this.conditionValue = string.Empty;
            this.routineIds = new List<string>();
        }
    }

    [Serializable]
    public class RoutineData
    {
        public string info;
        public List<string> comments;

        public RoutineData(string info, List<string> comments)
        {
            this.info = info;
            this.comments = comments;
        }
    }

    [Serializable]
    public class ChangersInAuto
    {
        public string rootAutoId;
        public List<ChangerData> changers;

        public ChangersInAuto(string rootAutoId, List<ChangerData> changers)
        {
            this.rootAutoId = rootAutoId;
            this.changers = changers;
        }
    }

    [Serializable]
    public class ChangerData
    {
        public string changerId;
        public string changerName;
        public string rootAutoId;
        public List<string> comments;
        public List<BranchData> branchs;
        public FinallyBranchData finallyBranch;

        public ChangerData(string changerId, string changerName, string rootAutoId, List<string> comments, List<BranchData> branchs, FinallyBranchData finallyBranch)
        {
            this.changerId = changerId;
            this.changerName = changerName;
            this.rootAutoId = rootAutoId;
            this.comments = comments;
            this.branchs = branchs;
            this.finallyBranch = finallyBranch;
        }
    }

    [Serializable]
    public class BranchData
    {
        public List<string> comments;
        public List<ConditionBindData> conditonBinds;
        public string nextAutoId;
        public bool isContinue;
        public List<string> inheritTimelineConditions;

        public BranchData(List<string> comments, List<ConditionBindData> conditonBinds, string nextAutoId, bool isContinue, List<string> inheritTimelineConditions)
        {
            this.comments = comments;
            this.conditonBinds = conditonBinds;
            this.nextAutoId = nextAutoId;
            this.isContinue = isContinue;
            this.inheritTimelineConditions = inheritTimelineConditions;
        }
    }

    [Serializable]
    public class ConditionBindData
    {
        public string combinationKind;
        public List<ConditionTypeValueData> combinations;

        public ConditionBindData(string combinationKind, List<ConditionTypeValueData> combinations)
        {
            this.combinationKind = combinationKind;
            this.combinations = combinations;
        }
    }

    [Serializable]
    public class ConditionTypeValueData
    {
        public string type;
        public string val;

        public ConditionTypeValueData(string type, string val)
        {
            this.type = type;
            this.val = val;
        }
    }

    [Serializable]
    public class FinallyBranchData
    {
        public string finallyAutoId;
        public List<string> inheritTimelineConditions;

        public FinallyBranchData()
        {
            this.finallyAutoId = string.Empty;
            this.inheritTimelineConditions = new List<string>();
        }

        public FinallyBranchData(string finallyAutoId, List<string> inheritTimelineConditions)
        {
            this.finallyAutoId = finallyAutoId;
            this.inheritTimelineConditions = inheritTimelineConditions;
        }

        public bool IsExists()
        {
            return !string.IsNullOrEmpty(finallyAutoId);
        }
    }

    [Serializable]
    public class ConditionTypeData
    {
        public List<string> conditionTypes;

        public ConditionTypeData(List<string> conditionTypes)
        {
            this.conditionTypes = conditionTypes;
        }
    }

    [Serializable]
    public class ConditionValueData
    {
        public string typeId;
        public List<string> comment;
        public List<ValueAndCommentData> valueAndComments;

        public ConditionValueData(string typeId, List<string> comment, List<ValueAndCommentData> valueAndComments)
        {
            this.typeId = typeId;
            this.comment = comment;
            this.valueAndComments = valueAndComments;
        }
    }

    [Serializable]
    public class ValueAndCommentData
    {
        public string valueName;
        public string comment;

        public ValueAndCommentData(string valueName, string comment)
        {
            this.valueName = valueName;
            this.comment = comment;
        }
    }


    [Serializable]
    public class ConditionData
    {
        public string conditionType;
        public List<string> conditionValues = new List<string>();

        public ConditionData(string conditionType, List<string> conditionValues)
        {
            this.conditionType = conditionType;
            this.conditionValues = conditionValues;
        }
    }
}
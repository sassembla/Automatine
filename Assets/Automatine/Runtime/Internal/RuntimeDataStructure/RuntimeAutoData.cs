using System;
using System.Collections.Generic;

[Serializable]
public class RuntimeAutoData
{
    public List<RuntimeChangerData> rootChangers;
    public List<RuntimeChangerData> changers;

    public List<RuntimeTimelineData> timelines;

    public RuntimeAutoData(List<RuntimeChangerData> rootChangers, List<RuntimeChangerData> changers, List<RuntimeTimelineData> timelines)
    {
        this.rootChangers = rootChangers;
        this.changers = changers;
        this.timelines = timelines;
    }
}

[Serializable]
public class RuntimeTimelineData
{
    public List<RuntimeTackData> tacks;
    public RuntimeTimelineData(List<RuntimeTackData> tacks)
    {
        this.tacks = tacks;
    }
}

[Serializable]
public class RuntimeTackData
{
    public int start;
    public int span;

    public string conditionType;
    public string conditionValue;

    public List<RuntimeRoutineData> routines;

    public RuntimeTackData(int start, int span, string conditionType, string conditionValue, List<RuntimeRoutineData> routines)
    {
        this.start = start;
        this.span = span;
        this.conditionType = conditionType;
        this.conditionValue = conditionValue;
        this.routines = routines;
    }
}

[Serializable]
public class RuntimeRoutineData
{
    public string routineName;
    public RuntimeRoutineData(string routineName)
    {
        this.routineName = routineName;
    }
}

[Serializable]
public class RuntimeChangerData
{
    public string changerName;

    public RuntimeChangerData(string changerName)
    {
        this.changerName = changerName;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Automatine;
using UnityEngine;


/**
	setup auto from json.
*/
public partial class Test
{
    public void _42_0_AutoFromAuto()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var sourceAuto = new AutoForTest42<string, string>(frame, string.Empty);
        var autoData = sourceAuto.RuntimeAutoData();

        var auto0SourceFunc = Auto<PlayerContext, Dictionary<string, PlayerContext>>.RuntimeAutoGenerator(autoData);
        var auto0 = auto0SourceFunc(frame, context0);

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;// frame = 1.

        if (auto0.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed.");
    }

    public void _42_1_AutoFromAutoWithCoroutine()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var sourceAuto = new AutoForTest42_1<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);
        var autoData = sourceAuto.RuntimeAutoData();

        var auto0SourceFunc = Auto<PlayerContext, Dictionary<string, PlayerContext>>.RuntimeAutoGenerator(autoData);
        var auto0 = auto0SourceFunc(frame, context0);

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;// frame = 1.

        if (auto0.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed.");
    }

    public void _42_2_AutoFromAutoWithChanger()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var sourceAuto = new AutoForTest42_2<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);
        var autoData = sourceAuto.RuntimeAutoData();

        var auto0SourceFunc = Auto<PlayerContext, Dictionary<string, PlayerContext>>.RuntimeAutoGenerator(autoData);
        var auto0 = auto0SourceFunc(frame, context0);

        if (auto0.Changers().Any())
        {
            auto0 = auto0.EmitChanger(frame, context0, auto0.Changers()[0]);
            if (auto0.autoInfo == "AutoForTest42_1 sample auto") return;
        }

        Debug.LogError("failed.");
    }


    public void _42_3_AutoFromAutoAsOtherType()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var sourceAuto = new AutoForTest42_1<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);
        var autoData = sourceAuto.RuntimeAutoData();

        var auto0SourceFunc = Auto<string, string>.RuntimeAutoGenerator(autoData);
        var auto0 = auto0SourceFunc(frame, string.Empty);

        auto0.Update(frame, string.Empty);
        frame++;

        if (auto0.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed.");
    }


    public void _42_4_AutoFromJson()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var sourceAuto = new AutoForTest42<string, string>(frame, string.Empty);
        var autoData = sourceAuto.RuntimeAutoData();

        string jsonString = AutomatineJsonConverter.AutoDataToJson(autoData);
        var autoDataFromJson = AutomatineJsonConverter.JsonToAutoData(jsonString);

        var auto0SourceFunc = Auto<PlayerContext, Dictionary<string, PlayerContext>>.RuntimeAutoGenerator(autoDataFromJson);
        var auto0 = auto0SourceFunc(frame, context0);

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;// frame = 1.

        if (auto0.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed.");
    }

    public void _42_5_AutoFromJsonWithCoroutine()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var sourceAuto = new AutoForTest42_1<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);
        var autoData = sourceAuto.RuntimeAutoData();

        string jsonString = AutomatineJsonConverter.AutoDataToJson(autoData);
        var autoDataFromJson = AutomatineJsonConverter.JsonToAutoData(jsonString);

        var auto0SourceFunc = Auto<PlayerContext, Dictionary<string, PlayerContext>>.RuntimeAutoGenerator(autoDataFromJson);
        var auto0 = auto0SourceFunc(frame, context0);

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;// frame = 1.

        if (auto0.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed.");
    }

    public void _42_6_AutoFromAutoDataWithKeepingConditions()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var sourceAuto = new AutoForTest42<string, string>(frame, string.Empty);
        var autoData = sourceAuto.RuntimeAutoData();

        var autoFunc = Auto<PlayerContext, Dictionary<string, PlayerContext>>.RuntimeAutoGenerator(autoData);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithAllTimelineConsumed",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithAllTimelineConsumedTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithAllTimelineConsumedRoutine",
                    0,
                    1,
                    AutoConditions.Act.FIVEFRAME_SPAN
                )
            )
        );

        auto0 = auto0.ChangeTo(autoFunc(frame, context0));

        if (auto0.ContainsCondition(AutoConditions.Act.FIVEFRAME_SPAN)) return;

        Debug.LogError("failed.");
    }

    public void _42_7_AutoFromAutoDataWithoutKeepingConditions()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var sourceAuto = new AutoForTest42<string, string>(frame, string.Empty);
        var autoData = sourceAuto.RuntimeAutoData();

        var autoFunc = Auto<PlayerContext, Dictionary<string, PlayerContext>>.RuntimeAutoGenerator(autoData);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithAllTimelineConsumed",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithAllTimelineConsumedTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithAllTimelineConsumedRoutine",
                    0,
                    1,
                    AutoConditions.Act.FIVEFRAME_SPAN
                )
            )
        );

        auto0 = auto0.ChangeToWithoutKeepingConditions(autoFunc(frame, context0));

        if (!auto0.ContainsCondition(AutoConditions.Act.FIVEFRAME_SPAN)) return;

        Debug.LogError("failed.");
    }
}

public class AutoForTest42<InitialParamType, UpdateParamType> : Auto<InitialParamType, UpdateParamType>
{
    public AutoForTest42(int frame, InitialParamType context) : base(
        "test41 sample auto",
        frame,
        context,
        new Timeline<InitialParamType, UpdateParamType>(
            "timelineTitle",
            new Tack<InitialParamType, UpdateParamType>(
                "tackTitle",
                0,
                1,
                AutoConditions.Anim.DEFAULT
            )
        )
    )
    { }
}
public class AutoForTest42_1<InitialParamType, UpdateParamType> : Auto<InitialParamType, UpdateParamType>
{
    public AutoForTest42_1(int frame, InitialParamType context) : base(
        "AutoForTest42_1 sample auto",
        frame,
        context,
        new Timeline<InitialParamType, UpdateParamType>(
            "timelineTitle",
            new Tack<InitialParamType, UpdateParamType>(
                "tackTitle",
                0,
                1,
                AutoConditions.Anim.DEFAULT,
                () =>
                {
                    var rc = new RoutineContexts<InitialParamType, UpdateParamType>("RoutineFromString");
                    return rc;
                }
            )
        )
    )
    { }
}

public class AutoForTest42_2<InitialParamType, UpdateParamType> : Auto<InitialParamType, UpdateParamType>
{
    public AutoForTest42_2(int frame, InitialParamType context) : base(
        "AutoForTest42_2 sample auto",
        frame,
        context,
        new List<IAutoChanger>(),
        new List<IAutoChanger> { new Changer_ChangerForTest42_2() },
        new Timeline<InitialParamType, UpdateParamType>(
            "timelineTitle",
            new Tack<InitialParamType, UpdateParamType>(
                "tackTitle",
                0,
                1,
                AutoConditions.Anim.DEFAULT,
                () =>
                {
                    var rc = new RoutineContexts<InitialParamType, UpdateParamType>("RoutineFromString");
                    return rc;
                }
            )
        )
    )
    { }
}

public class Changer_ChangerForTest42_2 : IAutoChanger
{
    public static Auto<InitialParamType, UpdateParamType>
        ChangerForTest42_2
        <InitialParamType, UpdateParamType>
        (Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext)
    {
        var conditions = baseAuto.Conditions();

        /*
			comment for branchName0
		*/
        if (
            ConditionGateway.Contains(conditions, AutoConditions.Anim.DEFAULT)
        )
        {
            var newAuto = new
                AutoForTest42_1
                <InitialParamType, UpdateParamType>(frame, fixedContext);
            return newAuto;
        }

        return baseAuto;
    }

    public Func<Auto<InitialParamType, UpdateParamType>, int, InitialParamType, Auto<InitialParamType, UpdateParamType>> Changer<InitialParamType, UpdateParamType>()
    {
        return delegate (Auto<InitialParamType, UpdateParamType> auto, int frame, InitialParamType fixedContext)
        {
            return ChangerForTest42_2(auto, frame, fixedContext);
        };
    }


    public string ChangerName()
    {
        return "ChangerForTest42_2";
    }

    public static bool IsEffectiveChanger<InitialParamType, UpdateParamType>(Auto<InitialParamType, UpdateParamType> targetAuto)
    {
        var conditions = targetAuto.Conditions();

        if (
            ConditionGateway.Contains(conditions, AutoConditions.Act.P0)
        )
        {
            return true;
        }

        return false;
    }

    public bool IsEffective<InitialParamType, UpdateParamType>(Auto<InitialParamType, UpdateParamType> baseAuto)
    {
        var conditions = baseAuto.Conditions();

        if (
            ConditionGateway.Contains(conditions, AutoConditions.Act.P0)
        )
        {
            return true;
        }

        return false;
    }
}

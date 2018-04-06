using System;
using System.Collections.Generic;
using System.Linq;
using Automatine;
using UnityEngine;


/**
	Changers, StackChangerのテスト
*/
public partial class Test
{
    public void _40_0_Changers()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest40<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

        var changers = auto0.Changers();

        if (changers.Any()) return;

        Debug.LogError("failed.");
    }

    public void _40_1_RootChangers()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest40<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

        var rootChangers = auto0.RootChangers();

        if (rootChangers.Any()) return;

        Debug.LogError("failed.");
    }


    public void _40_2_StackChangers()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest40<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

        auto0.StackChanger(new Changer_ChangerForTest40());
        if (auto0.StackedChangers().Any())
        {
            return;
        }

        Debug.LogError("failed.");
    }

    public void _40_3_CheckStackedChangers()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest40<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

        auto0.StackChanger(new Changer_ChangerForTest40());

        var stackedChangers = auto0.StackedChangers();

        foreach (var changer in stackedChangers)
        {
            var changerId = changer.ChangerName();
            if (changerId == "ChangerForTest40") return;
        }

        Debug.LogError("failed.");
    }

    public void _40_4_CheckStackedChangersThenUse()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest40<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0) as Auto<PlayerContext, Dictionary<string, PlayerContext>>;

        auto0.StackChanger(new Changer_ChangerForTest40_1());

        var stackedChangers = auto0.StackedChangers();

        /**
           choose one of changer by name.
        */
        foreach (var changer in stackedChangers)
        {
            var changerId = changer.ChangerName();
            if (changerId == "ChangerForTest40_1")
            {
                // change auto by changer.
                auto0 = changer.Changer<PlayerContext, Dictionary<string, PlayerContext>>()(auto0, frame, context0);

                if (auto0.autoInfo == "test40_2 sample auto") return;
                break;
            }
        }

        Debug.LogError("failed.");
    }

    public void _40_5_EffectiveChangers()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest40<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0) as Auto<PlayerContext, Dictionary<string, PlayerContext>>;

        var effetiveChangers = auto0.Changers().Where(changer => changer.IsEffective<PlayerContext, Dictionary<string, PlayerContext>>(auto0));

        if (effetiveChangers.Where(changer => changer.ChangerName() == "ChangerForTest40").Any()) return;

        Debug.LogError("failed.");
    }

    public void _40_6_EffectiveChangersWithFrameProgress()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest40<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0) as Auto<PlayerContext, Dictionary<string, PlayerContext>>;
        for (var i = 0; i < 4; i++)
        {
            auto0.Update(frame, new Dictionary<string, PlayerContext>());
            frame++;
        }

        var effetiveChangers = auto0.Changers().Where(changer => changer.IsEffective<PlayerContext, Dictionary<string, PlayerContext>>(auto0));

        if (!effetiveChangers.Any()) return;

        Debug.LogError("failed.");
    }


    public void _40_7_StackChangerFromCoroutine()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest40_3<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0) as Auto<PlayerContext, Dictionary<string, PlayerContext>>;

        // stack changer from coroutine.
        auto0.Update(frame, new Dictionary<string, PlayerContext>());

        var stackedChangers = auto0.StackedChangers();
        if (stackedChangers.Any()) return;

        Debug.LogError("failed.");
    }


    public void _40_8_StackChangerFromCoroutineThenEmitChanger()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest40_3<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0) as Auto<PlayerContext, Dictionary<string, PlayerContext>>;

        // stack changer from coroutine.
        auto0.Update(frame, new Dictionary<string, PlayerContext>());

        var stackedChangers = auto0.StackedChangers();
        if (stackedChangers.Any())
        {
            var stackedChanger = stackedChangers[0];
            auto0 = auto0.EmitChanger(frame, context0, stackedChanger);
            if (auto0.autoInfo == "test40_4 sample auto") return;
        }

        Debug.LogError("failed.");
    }

    public void _40_9_StackChangerFromCoroutineThenEmitEffectiveChanger()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest40_3<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0) as Auto<PlayerContext, Dictionary<string, PlayerContext>>;

        // stack changer from coroutine.
        auto0.Update(frame, new Dictionary<string, PlayerContext>());

        // choose effective (next frame of the changer stacked from coroutine.)
        var stackedChangers = auto0.StackedEffectiveChangers();
        if (stackedChangers.Any())
        {
            var stackedChanger = stackedChangers[0];
            auto0 = auto0.EmitChanger(frame, context0, stackedChanger);
            if (auto0.autoInfo == "test40_4 sample auto") return;
        }

        Debug.LogError("failed.");
    }

    public void _40_10_StackedChangerWillClear()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest40_3<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0) as Auto<PlayerContext, Dictionary<string, PlayerContext>>;

        // stack changer from coroutine.
        auto0.Update(frame, new Dictionary<string, PlayerContext>());

        frame++;

        auto0.Update(frame, new Dictionary<string, PlayerContext>());

        if (!auto0.StackedChangers().Any()) return;

        Debug.LogError("failed.");
    }

    public void _40_11_StackChangerFromCoroutineThenEmitEffectiveChangerWithProgress()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest40_4<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0) as Auto<PlayerContext, Dictionary<string, PlayerContext>>;

        // stack changer from coroutine.
        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        // effective in this frame. progress auto will break situation.

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        if (auto0.ShouldFalldown(frame))
        {
            // choose effective (next frame of the changer stacked from coroutine.)
            var stackedChangers = auto0.StackedEffectiveChangers();

            if (stackedChangers.Any())
            {
                var stackedChanger = stackedChangers[0];
                auto0 = auto0.EmitChanger(frame, context0, stackedChanger);
                if (auto0.autoInfo != "AutoForTest40_4") return;
            }
        }

        Debug.LogError("failed.");
    }

    public void _40_12_StackChangerFromCoroutineThenEmitEffectiveChangerWithoutKeepingConditionsWithProgress()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest40_5<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0) as Auto<PlayerContext, Dictionary<string, PlayerContext>>;

        // stack changer from coroutine.
        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        // effective in this frame. progress auto will break situation.

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;
        if (auto0.ShouldFalldown(frame))
        {
            // choose effective (next frame of the changer stacked from coroutine.)
            var stackedChangers = auto0.StackedEffectiveChangers();

            if (stackedChangers.Any())
            {

                var stackedChanger = stackedChangers[0];
                auto0 = auto0.EmitChangerWithoutKeepingConditions(frame, context0, stackedChanger);
                if (!auto0.ContainsCondition(AutoConditions.Act.P2)) return;
            }
        }

        Debug.LogError("failed.");
    }

    public void _40_13_StackChangerFromCoroutineThenEmitEffectiveChangerKeepingConditionsWithProgress()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest40_4<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0) as Auto<PlayerContext, Dictionary<string, PlayerContext>>;

        // stack changer from coroutine.
        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        // effective in this frame. progress auto will break situation.

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        if (auto0.ShouldFalldown(frame))
        {
            // choose effective (next frame of the changer stacked from coroutine.)
            var stackedChangers = auto0.StackedEffectiveChangers();

            if (stackedChangers.Any())
            {
                var stackedChanger = stackedChangers[0];
                auto0 = auto0.EmitChanger(frame, context0, stackedChanger);
                if (auto0.ContainsCondition(AutoConditions.Act.P2)) return;
            }
        }

        Debug.LogError("failed.");
    }

    public void _40_14_StackChangerFromCoroutineThenEmitEffectiveChangersWithProgress()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest40_4<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0) as Auto<PlayerContext, Dictionary<string, PlayerContext>>;

        // stack changer from coroutine.
        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        // effective in this frame. progress auto will break situation.

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        if (auto0.ShouldFalldown(frame))
        {
            // choose effective (next frame of the changer stacked from coroutine.)
            var stackedChangers = auto0.StackedEffectiveChangers();
            auto0 = auto0.EmitChangers(frame, context0, stackedChangers);
            if (auto0.autoInfo != "AutoForTest40_4") return;
        }

        Debug.LogError("failed.");
    }

    public void _40_15_StackChangerFromCoroutineThenEmitEffectiveChangersWithoutKeepingConditionsWithProgress()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest40_5<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0) as Auto<PlayerContext, Dictionary<string, PlayerContext>>;

        // stack changer from coroutine.
        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        // effective in this frame. progress auto will break situation.

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        if (auto0.ShouldFalldown(frame))
        {
            // choose effective (next frame of the changer stacked from coroutine.)
            var stackedChangers = auto0.StackedEffectiveChangers();
            auto0 = auto0.EmitChangersWithoutKeepingConditions(frame, context0, stackedChangers);

            if (!auto0.ContainsCondition(AutoConditions.Act.P2)) return;
        }

        Debug.LogError("failed.");
    }


    public void _40_16_StackChangerFromCoroutineThenEmitEffectiveChangersKeepingConditionsWithProgress()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest40_4<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0) as Auto<PlayerContext, Dictionary<string, PlayerContext>>;

        // stack changer from coroutine.
        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        // effective in this frame. progress auto will break situation.

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        if (auto0.ShouldFalldown(frame))
        {
            // choose effective (next frame of the changer stacked from coroutine.)
            var stackedChangers = auto0.StackedEffectiveChangers();
            auto0 = auto0.EmitChangers(frame, context0, stackedChangers);

            if (auto0.ContainsCondition(AutoConditions.Act.P2)) return;
        }

        Debug.LogError("failed.");
    }



}

/*
    test stuff.
*/

public class Changer_ChangerForTest40 : IAutoChanger
{
    public static Auto<InitialParamType, UpdateParamType>
        ChangerForTest40
        <InitialParamType, UpdateParamType>
        (Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext)
    {
        var conditions = baseAuto.Conditions();

        /*
			comment for branchName0
		*/
        if (
            ConditionGateway.Contains(conditions, AutoConditions.Act.P0)
        )
        {
            var newAuto = new
                AutoForTest40_1
                <InitialParamType, UpdateParamType>(frame, fixedContext);
            return newAuto;
        }

        return baseAuto;
    }

    public Func<Auto<InitialParamType, UpdateParamType>, int, InitialParamType, Auto<InitialParamType, UpdateParamType>> Changer<InitialParamType, UpdateParamType>()
    {
        return delegate (Auto<InitialParamType, UpdateParamType> auto, int frame, InitialParamType fixedContext)
        {
            return ChangerForTest40(auto, frame, fixedContext);
        };
    }


    public string ChangerName()
    {
        return "ChangerForTest40";
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

public class Changer_ChangerForTest40_1 : IAutoChanger
{
    public static Auto<InitialParamType, UpdateParamType>
        ChangerForTest40_1
        <InitialParamType, UpdateParamType>
        (Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext)
    {
        var conditions = baseAuto.Conditions();

        /*
			comment for branchName0
		*/
        if (
            ConditionGateway.Contains(conditions, AutoConditions.Act.P0)
        )
        {
            var newAuto = new
                AutoForTest40_2
                <InitialParamType, UpdateParamType>(frame, fixedContext);
            return newAuto;
        }

        return baseAuto;
    }

    public Func<Auto<InitialParamType, UpdateParamType>, int, InitialParamType, Auto<InitialParamType, UpdateParamType>> Changer<InitialParamType, UpdateParamType>()
    {
        return delegate (Auto<InitialParamType, UpdateParamType> auto, int frame, InitialParamType fixedContext)
        {
            return ChangerForTest40_1(auto, frame, fixedContext);
        };
    }


    public string ChangerName()
    {
        return "ChangerForTest40_1";
    }

    public static bool IsEffectiveChanger<InitialParamType, UpdateParamType>(Auto<InitialParamType, UpdateParamType> baseAuto)
    {
        var conditions = baseAuto.Conditions();

        /*
			comment for branchName1
		*/
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

        /*
			comment for branchName1
		*/
        if (
            ConditionGateway.Contains(conditions, AutoConditions.Act.P0)
        )
        {
            return true;
        }

        return false;
    }
}

public class Changer_ChangerForTest40_3 : IAutoChanger
{
    public static Auto<InitialParamType, UpdateParamType>
        ChangerForTest40_3
        <InitialParamType, UpdateParamType>
        (Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext)
    {
        var conditions = baseAuto.Conditions();

        /*
			comment for branchName0
		*/
        if (
            ConditionGateway.Contains(conditions, AutoConditions.Act.P2)
        )
        {
            var newAuto = new
                AutoForTest40_4
                <InitialParamType, UpdateParamType>(frame, fixedContext);
            return newAuto;
        }

        return baseAuto;
    }

    public Func<Auto<InitialParamType, UpdateParamType>, int, InitialParamType, Auto<InitialParamType, UpdateParamType>> Changer<InitialParamType, UpdateParamType>()
    {
        return delegate (Auto<InitialParamType, UpdateParamType> auto, int frame, InitialParamType fixedContext)
        {
            return ChangerForTest40_3(auto, frame, fixedContext);
        };
    }


    public string ChangerName()
    {
        return "ChangerForTest40_3";
    }

    public static bool IsEffectiveChanger<InitialParamType, UpdateParamType>(Auto<InitialParamType, UpdateParamType> baseAuto)
    {
        var conditions = baseAuto.Conditions();

        /*
			comment for branchName1
		*/
        if (
            ConditionGateway.Contains(conditions, AutoConditions.Act.P2)
        )
        {
            Debug.LogError("check retruns true");
            return true;
        }

        return false;
    }

    public bool IsEffective<InitialParamType, UpdateParamType>(Auto<InitialParamType, UpdateParamType> baseAuto)
    {
        var conditions = baseAuto.Conditions();

        /*
			comment for branchName1
		*/
        if (
            ConditionGateway.Contains(conditions, AutoConditions.Act.P2)
        )
        {
            return true;
        }

        return false;
    }
}

public class Changer_ChangerForTest40_5 : IAutoChanger
{
    public static Auto<InitialParamType, UpdateParamType>
        ChangerForTest40_5
        <InitialParamType, UpdateParamType>
        (Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext)
    {
        var conditions = baseAuto.Conditions();

        /*
			comment for branchName0
		*/
        if (
            ConditionGateway.Contains(conditions, AutoConditions.Act.P2)
        )
        {
            var newAuto = new
                AutoForTest40_6
                <InitialParamType, UpdateParamType>(frame, fixedContext);
            return newAuto;
        }

        return baseAuto;
    }

    public Func<Auto<InitialParamType, UpdateParamType>, int, InitialParamType, Auto<InitialParamType, UpdateParamType>> Changer<InitialParamType, UpdateParamType>()
    {
        return delegate (Auto<InitialParamType, UpdateParamType> auto, int frame, InitialParamType fixedContext)
        {
            return ChangerForTest40_5(auto, frame, fixedContext);
        };
    }


    public string ChangerName()
    {
        return "ChangerForTest40_5";
    }

    public static bool IsEffectiveChanger<InitialParamType, UpdateParamType>(Auto<InitialParamType, UpdateParamType> baseAuto)
    {
        var conditions = baseAuto.Conditions();

        /*
			comment for branchName1
		*/
        if (
            ConditionGateway.Contains(conditions, AutoConditions.Act.P2)
        )
        {
            return true;
        }

        return false;
    }

    public bool IsEffective<InitialParamType, UpdateParamType>(Auto<InitialParamType, UpdateParamType> baseAuto)
    {
        return IsEffectiveChanger(baseAuto);
    }
}


public class AutoForTest40<InitialParamType, UpdateParamType> : Auto<InitialParamType, UpdateParamType>
{
    public AutoForTest40(int frame, InitialParamType context) : base(
        "test40 sample auto",
        frame,
        context,
        new List<IAutoChanger> { new Changer_ChangerForTest40() },
        new List<IAutoChanger> { new Changer_ChangerForTest40() },
        new Timeline<InitialParamType, UpdateParamType>(
            "timelineTitle",
            new Tack<InitialParamType, UpdateParamType>(
                "tackTitle",
                0,
                3,
                AutoConditions.Act.P0,
                () =>
                {
                    var c = new RoutineContexts<InitialParamType, UpdateParamType>();
                    c.rContext = c.WalkReady;
                    return c;
                }
            )
        )
    )
    { }
}

public class AutoForTest40_1<InitialParamType, UpdateParamType> : Auto<InitialParamType, UpdateParamType>
{
    public AutoForTest40_1(int frame, InitialParamType context) : base(
        "test40_1 sample auto",
        frame,
        context,
        new List<IAutoChanger> { new Changer_ChangerForTest40_1() },
        new List<IAutoChanger> { new Changer_ChangerForTest40_1() },
        new Timeline<InitialParamType, UpdateParamType>(
            "timelineTitle",
            new Tack<InitialParamType, UpdateParamType>(
                "tackTitle",
                0,
                3,
                AutoConditions.Act.P0,
                () =>
                {
                    var c = new RoutineContexts<InitialParamType, UpdateParamType>();
                    c.rContext = c.WalkReady;
                    return c;
                }
            )
        )
    )
    { }
}

public class AutoForTest40_2<InitialParamType, UpdateParamType> : Auto<InitialParamType, UpdateParamType>
{
    public AutoForTest40_2(int frame, InitialParamType context) : base(
        "test40_2 sample auto",
        frame,
        context,
        new List<IAutoChanger> { new Changer_ChangerForTest40_1() },
        new List<IAutoChanger> { new Changer_ChangerForTest40_1() },
        new Timeline<InitialParamType, UpdateParamType>(
            "timelineTitle",
            new Tack<InitialParamType, UpdateParamType>(
                "tackTitle",
                0,
                3,
                AutoConditions.Act.P1,
                () =>
                {
                    var c = new RoutineContexts<InitialParamType, UpdateParamType>();
                    c.rContext = c.WalkReady;
                    return c;
                }
            )
        )
    )
    { }
}

public class AutoForTest40_3<InitialParamType, UpdateParamType> : Auto<InitialParamType, UpdateParamType>
{
    public AutoForTest40_3(int frame, InitialParamType context) : base(
        "test40_3 sample auto",
        frame,
        context,
        new List<IAutoChanger>(),
        new List<IAutoChanger> { new Changer_ChangerForTest40_3() },
        new Timeline<InitialParamType, UpdateParamType>(
            "timelineTitle",
            new Tack<InitialParamType, UpdateParamType>(
                "tackTitle",
                0,
                3,
                AutoConditions.Act.P2,
                () =>
                {
                    var c = new RoutineContexts<InitialParamType, UpdateParamType>();
                    c.rContext = c.StackChangerFromCoroutine;
                    return c;
                }
            )
        )
    )
    { }
}

public class AutoForTest40_4<InitialParamType, UpdateParamType> : Auto<InitialParamType, UpdateParamType>
{
    public AutoForTest40_4(int frame, InitialParamType context) : base(
        "test40_4 sample auto",
        frame,
        context,
        new List<IAutoChanger>(),
        new List<IAutoChanger> { new Changer_ChangerForTest40_3() },
        new Timeline<InitialParamType, UpdateParamType>(
            "timelineTitle",
            new Tack<InitialParamType, UpdateParamType>(
                "tackTitle",
                0,
                3,
                AutoConditions.Act.P2,
                () =>
                {
                    var c = new RoutineContexts<InitialParamType, UpdateParamType>();
                    c.rContext = c.StackChangerFromCoroutineLoop;
                    return c;
                }
            )
        )
    )
    { }
}

public class AutoForTest40_5<InitialParamType, UpdateParamType> : Auto<InitialParamType, UpdateParamType>
{
    public AutoForTest40_5(int frame, InitialParamType context) : base(
        "AutoForTest40_5 sample auto",
        frame,
        context,
        new List<IAutoChanger>(),
        new List<IAutoChanger> { new Changer_ChangerForTest40_5() },
        new Timeline<InitialParamType, UpdateParamType>(
            "timelineTitle",
            new Tack<InitialParamType, UpdateParamType>(
                "tackTitle",
                0,
                3,
                AutoConditions.Act.P2,
                () =>
                {
                    var c = new RoutineContexts<InitialParamType, UpdateParamType>();
                    c.rContext = c.StackChangerFromCoroutineLoopForAnotherChanger;
                    return c;
                }
            )
        )
    )
    { }
}


public class AutoForTest40_6<InitialParamType, UpdateParamType> : Auto<InitialParamType, UpdateParamType>
{
    public AutoForTest40_6(int frame, InitialParamType context) : base(
        "AutoForTest40_5 sample auto",
        frame,
        context,
        new Timeline<InitialParamType, UpdateParamType>(
            "timelineTitle",
            new Tack<InitialParamType, UpdateParamType>(
                "tackTitle",
                0,
                3,
                AutoConditions.Act.P3,
                () =>
                {
                    var c = new RoutineContexts<InitialParamType, UpdateParamType>();
                    c.rContext = c.StackChangerFromCoroutineLoopForAnotherChanger;
                    return c;
                }
            )
        )
    )
    { }
}
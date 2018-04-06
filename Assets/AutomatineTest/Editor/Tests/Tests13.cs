using UnityEngine;

using System;
using System.Collections.Generic;
using Automatine;



// Directing

public partial class Test
{

    public void _13_0_DirectionChangeAuto()
    {
        int frame = 100;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine0",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL_0",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    0,
                    1,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        auto = auto.Changer_ConditionChange0(frame, context0);
        var conditions = auto.Conditions();
        if (conditions.ContainsCondition(AutoConditions.Act.P0))
        {
            Debug.LogError("failed to direction");
            return;
        }

        if (conditions.ContainsAllConditions(AutoConditions.Act.P1, AutoConditions.Canc.WALK))
        {
            return;
        }

        Debug.LogError("failed to direction");
    }

    public void _13_1_DirectionChangeAutoTwice()
    {
        int frame = 100;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine0",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL_0",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    0,
                    1,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        auto = auto.Changer_ConditionChange0(frame, context0);
        auto = auto.Changer_ConditionChange1(frame, context0);

        var conditions = auto.Conditions();

        if (conditions.ContainsCondition(AutoConditions.Act.P0))
        {
            Debug.LogError("failed to change");
            return;
        }

        if (conditions.ContainsCondition(AutoConditions.Act.P1) &&
            conditions.ContainsCondition(AutoConditions.Canc.WALK))
        {
            Debug.LogError("failed to change");
            return;
        }

        if (conditions.ContainsAllConditions(AutoConditions.Act.P3, AutoConditions.Canc.DASH))
        {
            return;
        }

        Debug.LogError("failed to change");
    }

    public void _13_2_DirectionChangeCondition()
    {
        int frame = 100;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine0",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL_0",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    0,
                    1,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        auto = auto.Changer_ConditionChange0(frame, context0);
        // changed to Act.P1, Canc.WALK

        var conditions0 = auto.Conditions();
        if (conditions0.ContainsAllConditions(AutoConditions.Act.P1, AutoConditions.Canc.WALK))
        {

        }
        else
        {
            Debug.LogError("failed to direction");
        }

        // changed to Act.P1, Canc.WALK
        auto.Update(frame, dummyContexts);
        // still keep. because 1 frame is still remaining.

        frame++;
        auto.Update(frame, dummyContexts);
        // tack consumed and conditions are changed.

        var conditions1 = auto.Conditions();
        if (conditions1.ContainsAllConditions(AutoConditions.Act.P2, AutoConditions.Canc.DASH))
        {
            // do nothing.
        }
        else
        {
            conditions1.PrintConditions();
            Debug.LogError("failed to direction P2 and DASH aren't there.");
            return;
        }

        auto = auto.Changer_ConditionChange1(frame, context0);

        var conditions2 = auto.Conditions();

        if (conditions2.ContainsCondition(AutoConditions.Act.P1) &&
            conditions2.ContainsCondition(AutoConditions.Canc.WALK))
        {
            return;
        }

        Debug.LogError("failed to direction");
    }

    public void _13_3_DirectionInheritTimeline()
    {
        int frame = 100;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine0",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL_0",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    0,
                    10,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakAutoAtLastFrame;
                        return c;
                    }
                )
            )
        );

        auto = auto.Changer_ConditionChange2(frame, context0);

        for (int i = 0; i < 10; i++)
        {
            auto.Update(frame, dummyContexts);
            frame++;
        }

        if (auto.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed to direction");
    }


}

public static class Misc
{
    public static void PrintConditions(this int[] conds)
    {
        foreach (var a in conds)
        {
            Debug.LogError("a:" + a);
        }
    }
}
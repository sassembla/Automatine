using UnityEngine;

using System;
using System.Collections.Generic;
using Automatine;



// Test

/**
	routine
*/
public partial class Test
{

    public void _11_0_AutoWillNeverIgniteWithSameFrame()
    {
        int frame = 100;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine0",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    0,
                    2,
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

        var shouldFalldown = auto0.ShouldFalldown(frame);

        if (shouldFalldown)
        {
            Debug.LogError("fail.");
            return;
        }

        // 0
        auto0.Update(frame, dummyContexts);

        // 1(same frame running.)
        auto0.Update(frame, dummyContexts);

        var shouldFalldown2 = auto0.ShouldFalldown(frame);

        // span never changes.
        if (!shouldFalldown2)
        {
            return;
        }

        Debug.LogError("faild to keep Auto on same frame");
    }

    public void _11_1_AutoRunningWithCatchUpFrame()
    {
        int frame = 99;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine0",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    0,
                    101,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_1",
                    101,
                    1,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakAuto;
                        return c;
                    }
                )
            )
        );

        frame = 200;
        auto0.Update(frame, dummyContexts);
        // up to 200. run 101 times.

        frame = 201;
        if (auto0.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("faild to catch up frame");
    }

    public void _11_2_AutoRunningWithCatchUpFrame2()
    {
        int frame = 50;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine0",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    0,
                    150,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_1",
                    150,
                    1,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakAuto;
                        return c;
                    }
                )
            )
        );

        frame = 200;
        auto0.Update(frame, dummyContexts);
        // up to 200. run 150 times.

        if (auto0.ShouldFalldownInNextFrame(frame))
        {
            return;
        }

        Debug.LogError("faild to catch up frame");
    }

    public void _11_3_AutoReturnsUnUpdatedConditions()
    {
        int frame = 100;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine0",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL",
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

        var conditions = auto0.Conditions();
        if (ConditionGateway.Contains(conditions, AutoConditions.Act.P0))
        {
            return;
        }

        Debug.LogError("failed to get conditions");
    }

    public void _11_7_AutoWillChangeByAutoDotBreakTimelinesThenAutoIsBroken()
    {
        int frame = 100;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine0",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    0,
                    10,
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

        auto0.Update(frame, dummyContexts);
        auto0.BreakTimelines(typeof(AutoConditions.Act));

        if (auto0.ShouldFalldownInNextFrame(frame))
        {
            return;
        }

        Debug.LogError("failed to break timeline");
    }

    public void _11_8_AutoWillChangeByAutoDotBreakTimelinesThenAutoIsBroken2()
    {
        int frame = 100;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
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
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL_1",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_1",
                    10,
                    10,
                    AutoConditions.Anim.DEFAULT,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        auto0.Update(frame, dummyContexts);
        auto0.BreakTimelines(typeof(AutoConditions.Act), typeof(AutoConditions.Anim));

        if (auto0.ShouldFalldownInNextFrame(frame))
        {
            return;
        }

        Debug.LogError("failed to break timeline");
    }

    public void _11_9_AutoWillChangeByAutoDotBreakTimelinesThenAutoIsAlive()
    {
        int frame = 100;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
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
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL_1",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_2",
                    10,
                    10,
                    AutoConditions.Anim.DEFAULT,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL_2",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_3",
                    0,
                    10,
                    AutoConditions.Hit.ATTACK,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        auto0.Update(frame, dummyContexts);
        auto0.BreakTimelines(typeof(AutoConditions.Act), typeof(AutoConditions.Anim));

        var conditions = auto0.Conditions();
        if (conditions.ContainsAllConditions(AutoConditions.Hit.ATTACK))
        {
            return;
        }

        Debug.LogError("failed to break timeline");
    }

}






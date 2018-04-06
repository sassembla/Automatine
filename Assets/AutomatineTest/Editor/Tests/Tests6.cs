using UnityEngine;

using System;
using System.Collections.Generic;
using Automatine;



// Test

/**
	breakとコンディションの関連
*/
public partial class Test
{

    public void _6_0_ConditionWithSkipTackByBreackTack()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "6_0",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "6_0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "6_0R_0",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakTack;
                        return c;
                    }
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "6_0R_1",
                    100,
                    100,
                    AutoConditions.Act.TENFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, dummyContexts);

        // breakTackしたら何を証明したいか。


        var conditions = auto.Conditions();
        if (ConditionGateway.NotContains(conditions, AutoConditions.Act.FIVEFRAME_SPAN))
        {
            return;
        }

        Debug.LogError("failed to break tack");
    }

    public void _6_1_ConditionWithConsumeAllTimelineByBreackTimeline()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "6_1",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "6_1TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "6_1R",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakTimeline;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, dummyContexts);

        var conditions = auto.Conditions();

        // AutoConditions.Act.FIVEFRAME_SPAN Timeline is over.
        if (ConditionGateway.NotContains(conditions, AutoConditions.Act.FIVEFRAME_SPAN))
        {
            return;
        }

        Debug.LogError("failed to break timeline");
    }


    public void _6_2_ConditionWithConsumeOneTimelineByBreackTimeline()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "6_2",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "6_2TL_0",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "6_2R_0",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakTimeline;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "6_2TL_1",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "6_2R_1",
                    0,
                    100,
                    AutoConditions.Move.WALK_READY,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, dummyContexts);

        var conditions = auto.Conditions();

        // AutoConditions.Act.FIVEFRAME_SPAN Timeline is over.
        if (ConditionGateway.NotContains(conditions, AutoConditions.Act.FIVEFRAME_SPAN))
        {
            if (ConditionGateway.Contains(conditions, AutoConditions.Move.WALK_READY))
            {
                return;
            }
        }

        Debug.LogError("failed to break timeline");
    }

    public void _6_3_ConditionShouldEmptyByBreakAllTack()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithBreakTack",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithBreakTackTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTackRoutine",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakTack;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, dummyContexts);

        // breakTack is contained in routine. all tack & timelines are consumed　but still on that tack. Auto returns condition of last frame.
        var conditions = auto.Conditions();
        if (ConditionGateway.Contains(conditions, AutoConditions.Act.FIVEFRAME_SPAN))
        {
            return;
        }

        Debug.LogError("failed to break tack.");
    }

    public void _6_4_ConditionShouldKeepAfterAutoConsumed()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithBreakTack",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithBreakTackTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTackRoutine",
                    0,
                    1,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        var beforeConditions = auto.Conditions();

        // just consumed
        auto.Update(frame, dummyContexts);

        // nothing changes.
        var afterConditions = auto.Conditions();

        if (afterConditions.SameConditions(beforeConditions))
        {
            return;
        }

        Debug.LogError("conditions still contains something.");
    }

    public void _6_5_ConditionShouldFalldownAfterAutoConsumed()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithBreakTack",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithBreakTackTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTackRoutine",
                    0,
                    1,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        // just consumed
        auto.Update(frame, dummyContexts);

        // goto next frame.
        frame++;

        if (auto.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("conditions still contains something.");
    }

    public void _6_6_ConditionWithBreakTimelineSandwich()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "auto",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "tl_0",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "r_0",
                    0,
                    100,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakTimelineThenGetConditionsAndBreakAutoIfConditionIsChanged;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "tl_1",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "r_1",
                    0,
                    100,
                    AutoConditions.Move.WALK_READY,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        dummyContext.auto = auto;

        auto.Update(frame, dummyContexts);
        frame++;

        if (auto.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed to break timeline");
    }

    public void _6_7_ConditionWithBreakTackSandwich()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "auto",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "tl",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "r_0",
                    0,
                    100,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakTackThenGetConditionsAndBreakAutoIfConditionIsNotChanged;
                        return c;
                    }
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "r_1",
                    100,
                    100,
                    AutoConditions.Act.P1,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        dummyContext.auto = auto;

        auto.Update(frame, dummyContexts);
        frame++;

        if (auto.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed to break tack");
    }

    public void _6_8_ConditionWithBreakTacksSandwich()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithBreakTack",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithBreakTackTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTackRoutine",
                    0,
                    1,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakTackThenGetConditionsAndBreakAutoIfConditionIsNotChanged;
                        return c;
                    }
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTackRoutine",
                    1,
                    1,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTackRoutine",
                    2,
                    1,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTackRoutine",
                    3,
                    1,
                    AutoConditions.Act.P1,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        dummyContext.auto = auto;

        // 
        auto.Update(frame, dummyContexts);
        frame++;

        if (auto.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed to break tack");
    }

    public void _6_9_ConditionWithBreakTacksOveredSandwich()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithBreakTack",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithBreakTackTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTackRoutine",
                    0,
                    1,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakTackThenGetConditionsAndBreakAutoIfConditionIsNotChanged;
                        return c;
                    }
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTackRoutine",
                    1,
                    1,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTackRoutine",
                    2,
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

        dummyContext.auto = auto;

        auto.Update(frame, dummyContexts);
        frame++;


        if (auto.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed to keep condition");
    }

    public void _6_10_BreakAllTacksAndBreakTimelineIsNotSameConditions()
    {
        int frame0 = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithBreakTack_0",
            frame0,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithBreakTackTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTackRoutine",
                    0,
                    1,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakTackThenGetConditionsAndBreakAutoIfConditionIsNotChanged;
                        return c;
                    }
                )
            )
        );

        dummyContext.auto = auto0;
        auto0.Update(frame0, dummyContexts);
        var conditions0 = auto0.Conditions();
        frame0++;

        // conditions0 is P0 (just consumed).
        if (!conditions0.ContainsCondition(AutoConditions.Act.P0))
        {
            Debug.LogError("failed to contains.");
            return;
        }

        int frame1 = 0;
        var context1 = PlayerContext.Copy(dummyContext);

        var auto1 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithBreakTack_1",
            frame1,
            context1,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithBreakTackTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTackRoutine",
                    0,
                    1,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakTimeline;
                        return c;
                    }
                )
            )
        );


        dummyContext.auto = auto1;
        auto1.Update(frame1, dummyContexts);
        var conditions1 = auto1.Conditions();
        frame1++;

        // conditions1 is empty (timeline's break not contains conditions anymore.)
        if (conditions1.ContainsCondition(AutoConditions.Act.P0))
        {
            Debug.LogError("failed to contains.");
            return;
        }


        if (!conditions0.SameConditions(conditions1))
        {
            return;
        }

        Debug.LogError("failed to get same result");
    }

    public void _6_11_AutoConditionWithSkipTackByBreackTack()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "6_0",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "6_0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "6_0R_0",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakTack;
                        return c;
                    }
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "6_0R_1",
                    100,
                    100,
                    AutoConditions.Act.TENFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, dummyContexts);

        if (auto.NotContains(AutoConditions.Act.FIVEFRAME_SPAN))
        {
            if (auto.NotContains(AutoConditions.Act.TENFRAME_SPAN))
            {
                return;
            }
        }

        Debug.LogError("failed to break tack");
    }

    public void _6_12_AutoConditionWithConsumeAllTimelineByBreackTimeline()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "6_1",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "6_1TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "6_1R",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakTimeline;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, dummyContexts);

        // AutoConditions.Act.FIVEFRAME_SPAN Timeline is over.
        if (auto.NotContains(AutoConditions.Act.FIVEFRAME_SPAN))
        {
            return;
        }

        Debug.LogError("failed to break timeline");
    }


    public void _6_13_AutoConditionWithConsumeOneTimelineByBreackTimeline()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "6_2",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "6_2TL_0",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "6_2R_0",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakTimeline;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "6_2TL_1",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "6_2R_1",
                    0,
                    100,
                    AutoConditions.Move.WALK_READY,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, dummyContexts);

        // var conditions = auto.Conditions();

        // AutoConditions.Act.FIVEFRAME_SPAN Timeline is over.
        if (auto.NotContains(AutoConditions.Act.FIVEFRAME_SPAN))
        {
            if (auto.Contains(AutoConditions.Move.WALK_READY))
            {
                return;
            }
        }

        Debug.LogError("failed to break timeline");
    }

    public void _6_14_AutoConditionShouldEmptyByBreakAllTack()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithBreakTack",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithBreakTackTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTackRoutine",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakTack;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, dummyContexts);

        // breakTack is contained in routine. all tack & timelines are consumed　but still on that tack. Auto returns condition of last frame.

        if (auto.Contains(AutoConditions.Act.FIVEFRAME_SPAN))
        {
            return;
        }

        Debug.LogError("failed to break tack.");
    }

    public void _6_15_AutoConditionShouldKeepAfterAutoConsumed()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithBreakTack",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithBreakTackTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTackRoutine",
                    0,
                    1,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        var beforeConditions = auto.Conditions();

        // just consumed
        auto.Update(frame, dummyContexts);

        // nothing changes.

        if (auto.SameConditions(beforeConditions))
        {
            return;
        }

        Debug.LogError("conditions still contains something.");
    }

    public void _6_16_AutoConditionShouldKeepAfterChangeAuto()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithBreakTack",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithBreakTackTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTackRoutine",
                    0,
                    1,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, dummyContexts);

        // var conditions = auto.Conditions();
        auto = auto.ChangeTo(new AutoForTest6<PlayerContext, Dictionary<string, PlayerContext>>(frame, dummyContext));

        if (auto.Contains(AutoConditions.Act.FIVEFRAME_SPAN))
        {
            return;
        }

        Debug.LogError("failed.");
    }

    public class AutoForTest6<T1, T2> : Auto<T1, T2>
    {
        public AutoForTest6(int frame, T1 context) : base(
            "test40 sample auto",
            frame,
            context,
            new List<IAutoChanger> { new Changer_ChangerForTest40() },
            new List<IAutoChanger> { new Changer_ChangerForTest40() },
            new Timeline<T1, T2>(
                "timelineTitle",
                new Tack<T1, T2>(
                    "tackTitle",
                    0,
                    3,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<T1, T2>();
                        c.rContext = c.WalkReady;
                        return c;
                    }
                )
            )
        )
        { }
    }
    public void _6_17_AutoConditionDoesNotKeepAfterChangeAutoByNewWithoutKeepConditions()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithBreakTack",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithBreakTackTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTackRoutine",
                    0,
                    1,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, dummyContexts);

        // var conditions = auto.Conditions();
        auto = auto.ChangeToWithoutKeepingConditions(new AutoForTest6<PlayerContext, Dictionary<string, PlayerContext>>(frame, dummyContext));

        if (!auto.Contains(AutoConditions.Act.FIVEFRAME_SPAN))
        {
            return;
        }

        Debug.LogError("failed.");
    }
}
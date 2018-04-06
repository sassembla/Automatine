using UnityEngine;

using System;
using System.Collections.Generic;
using Automatine;



// Test

/**
	終了条件のテスト
*/
public partial class Test
{

    /**
		routineのBreakTack
	*/
    public void _4_0_ContinuationWithBreakAllTack()
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
        if (auto.ShouldFalldownInNextFrame(frame))
        {
            return;
        }

        Debug.LogError("failed to consume Auto:" + auto.autoInfo);
    }

    public void _4_1_ContinuationWithBreakAllTimeline()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithBreakTimeline",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithBreakTimelineTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTimelineRoutine",
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
        if (auto.ShouldFalldownInNextFrame(frame))
        {
            return;
        }

        Debug.LogError("failed to consume Auto:" + auto.autoInfo);
    }

    public void _4_2_ContinuationWithBreakOneTimelineThenKeepOtherTimeline()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithBreakTimeline",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithBreakTimelineTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTimelineRoutine",
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
                "ContinuationWithBreakTimelineTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakTimelineRoutine",
                    0,
                    100,
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
        if (!auto.ShouldFalldownInNextFrame(frame))
        {
            return;
        }

        Debug.LogError("failed to keep Auto:" + auto.autoInfo);
    }

    /**
		BreakAuto
	*/
    public void _4_3_ContinuationWithBreakAuto()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithBreakAuto",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithBreakTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakAutoRoutine",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakAuto;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, dummyContexts);
        if (auto.ShouldFalldownInNextFrame(frame))
        {
            return;
        }

        Debug.LogError("failed to consume Auto:" + auto.autoInfo);
    }

    public void _4_4_ContinuationWith1FrameConsume()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithBreakAuto",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithBreakTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakAutoRoutine",
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
        if (auto.ShouldFalldownInNextFrame(frame))
        {
            return;
        }

        Debug.LogError("failed to check continue.");
    }

    public void _4_5_ContinuationWithBreakTackIn1Frame()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithBreakAuto",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithBreakTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithBreakAutoRoutine",
                    0,
                    1,
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
        if (auto.ShouldFalldownInNextFrame(frame))
        {
            return;
        }

        Debug.LogError("failed to check continue.");
    }

    public void _4_6_ContinuationWithAllTimelineConsumed()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithAllTimelineConsumed",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithAllTimelineConsumedTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithAllTimelineConsumedRoutine",
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
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithAllTimelineConsumedTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithAllTimelineConsumedRoutine",
                    1,
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

        frame++;

        // every timelines are consumed.
        auto.Update(frame, dummyContexts);

        frame++;

        if (auto.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed to check continue.");
    }

    public void _4_7_ContinuationWithAllTackConsumed()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "ContinuationWithAllTimelineConsumed",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "ContinuationWithAllTimelineConsumedTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithAllTimelineConsumedRoutine",
                    0,
                    1,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "ContinuationWithAllTimelineConsumedRoutine",
                    1,
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

        frame++;

        // every tacks are consumed.
        auto.Update(frame, dummyContexts);

        frame++;

        if (auto.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed to check continue.");
    }


}
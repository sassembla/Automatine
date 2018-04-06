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
    public void _3_0_BreakTack()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "BreakTack",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "BreakTackTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "BreakTackRoutine",
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

        int i = 0;
        auto.Update(frame, dummyContexts);
        if (auto.ShouldFalldownInNextFrame(frame))
        {
            return;
        }

        i++;
        Debug.LogError("timeout i:" + i + " vs autoInfo:" + auto.autoInfo);
    }

    /**
		routineのBreakTimeline
	*/
    public void _3_1_BreakTimeline()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "BreakTimeline",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "BreakTimelineTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "BreakTimelineRoutine",
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

        int i = 0;
        auto.Update(frame, dummyContexts);
        if (auto.ShouldFalldownInNextFrame(frame))
        {
            return;
        }

        i++;
        Debug.LogError("timeout i:" + i + " vs autoInfo:" + auto.autoInfo);
    }

    /**
		routineのBreakAuto
	*/
    public void _3_2_BreakAuto()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "BreakAuto",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "BreakTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "BreakAutoRoutine",
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

        int i = 0;
        auto.Update(frame, dummyContexts);
        if (auto.ShouldFalldownInNextFrame(frame))
        {
            return;
        }


        i++;
        Debug.LogError("timeout i:" + i + " vs autoInfo:" + auto.autoInfo);
    }



}
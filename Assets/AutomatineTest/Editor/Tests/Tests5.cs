using UnityEngine;

using System;
using System.Collections.Generic;
using Automatine;



// Test

/**
	終了条件時に自身 + 後続が着火するかどうか
*/
public partial class Test
{

    public void _5_0_CancelThisRoutineByBreakTack()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "5_0",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "5_0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "5_0R",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._CancelThisRoutineByBreakTack;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, dummyContexts);
    }

    public void _5_1_CancelThisRoutineByBreakTimeline()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "5_1",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "5_1TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "5_1R",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._CancelThisRoutineByBreakTimeline;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, dummyContexts);
    }

    public void _5_2_CancelThisRoutineByBreakAuto()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "5_2",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "5_2TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "5_2R",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._CancelThisRoutineByBreakAuto;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, dummyContexts);
    }

    public void _5_3_CancelAnotherRoutineByBreakTack()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "5_3",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "5_3TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "5_3R",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._CancelRoutineByBreakTack;
                        return c;
                    },
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._NeverRun;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, dummyContexts);
    }

    public void _5_4_CancelTimelineByBreakTimeline()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "5_4",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "5_4TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "5_4R",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._CancelRoutineByBreakTimeline;
                        return c;
                    },
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

        Debug.LogError("failed to break timeline");
    }

    public void _5_5_CancelAllTimelinesByBreakTimeline()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "5_6",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "5_6TL_0",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "5_6R_0",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._CancelRoutineByBreakTimeline;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "5_6TL_1",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "5_6R_1",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._CancelRoutineByBreakTimeline;
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

        Debug.LogError("failed to break timelines");
    }

    public void _5_6_CancelAutoByBreakAuto()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "5_6",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "5_6TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "5_6R",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._CancelRoutineByBreakAuto;
                        return c;
                    },
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

        Debug.LogError("failed to break auto");
    }


    public void _5_7_CancelAnotherRoutineByBreakAutoAndBreakTack()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "5_7",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "5_7TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "5_7R",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._CancelRoutineByBreakAutoAndBreakTack;
                        return c;
                    },
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._NeverRun;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, dummyContexts);
    }

}
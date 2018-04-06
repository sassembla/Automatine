using UnityEngine;

using System;
using System.Collections.Generic;
using Automatine;



// Test

/**
	Inherit
*/
public partial class Test
{

    public void _7_0_InheritTimeline()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_0_0",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_0_0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_0_0R",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakAuto;// this routine will fire in inherited next auto.
                        return c;
                    }
                )
            )
        );

        var auto1 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_0_1",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_0_1TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_0_1R",
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

        var inheritTimelines = auto0.ExportTimelines(typeof(AutoConditions.Act));
        if (inheritTimelines.Count != 1)
        {
            Debug.LogError("failed to inherit timeline");
            return;
        }

        auto1.InheritTimelines(inheritTimelines);

        auto1.Update(frame, dummyContexts);

        if (auto1.ShouldFalldownInNextFrame(frame))
        {
            return;
        }

        Debug.LogError("failed to inherit timeline");
    }

    public void _7_1_InheritTimelines()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_1_0",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_1_0TL_0",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_1_0R",
                    0,
                    1,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakAuto;// this routine will fire in inherited next auto.
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_1_0TL_1",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_1_0R",
                    0,
                    1,
                    AutoConditions.Anim.DEFAULT,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakAuto;// this routine will fire in inherited next auto.
                        return c;
                    }
                )
            )
        );

        var auto1 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_1_1",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_1_1TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_1_1R",
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

        var inheritTimelines = auto0.ExportTimelines(typeof(AutoConditions.Act), typeof(AutoConditions.Anim));
        if (inheritTimelines.Count != 2)
        {
            Debug.LogError("failed to inherit timeline");
            return;
        }

        auto1.InheritTimelines(inheritTimelines);

        auto1.Update(frame, dummyContexts);

        if (auto1.ShouldFalldownInNextFrame(frame))
        {
            return;
        }

        Debug.LogError("failed to inherit timeline");
    }

    public void _7_2_InheritBrokeTimeline()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_2_0",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_2_0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_2_0R",
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

        // timeline will be consumed.
        auto0.Update(frame, dummyContexts);

        var inheritTimelines = auto0.ExportTimelines(typeof(AutoConditions.Act));
        if (inheritTimelines.Count != 0)
        {
            Debug.LogError("failed to inherit timeline");
        }
    }

    public void _7_3_InheritBrokeTimelines()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_3_0",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_3_0TL_0",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_3_0R_0",
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
                "7_3_0TL_1",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_3_0R_1",
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

        // timelines will be consumed.
        auto0.Update(frame, dummyContexts);

        var inheritTimelines = auto0.ExportTimelines(typeof(AutoConditions.Act));
        if (inheritTimelines.Count != 0)
        {
            Debug.LogError("failed to inherit timeline");
        }
    }

    public void _7_4_InheritTimelineFromBrokeAuto()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_4_0",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_4_0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_4_0R",
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

        auto0.Update(frame, dummyContexts);

        var inheritTimelines = auto0.ExportTimelines(typeof(AutoConditions.Act));
        if (inheritTimelines.Count != 1)
        {
            Debug.LogError("failed to inherit timeline");
        }
    }

    public void _7_5_InheritTimelinesFromBrokeAuto()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_5_0",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_5_0TL_0",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_5_0R_0",
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
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_5_0TL_1",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_5_0R_1",
                    0,
                    100,
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

        var inheritTimelines = auto0.ExportTimelines(typeof(AutoConditions.Act), typeof(AutoConditions.Anim));
        if (inheritTimelines.Count != 2)
        {
            Debug.LogError("failed to inherit timeline:" + inheritTimelines.Count);
        }
    }

    public void _7_6_InheritTimelineThenRunTack()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_6_0",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_6_0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_6_0R",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._FiveFrameThenBreakAuto;
                        return c;
                    }
                )
            )
        );

        auto0.Update(frame, dummyContexts);
        frame++;
        // rest 4 times

        var auto1 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_6_1",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_6_1TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_6_1R",
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

        var inheritTimelines = auto0.ExportTimelines(typeof(AutoConditions.Act));
        auto1.InheritTimelines(inheritTimelines);

        for (int i = 0; i < 4; i++)
        {
            auto1.Update(frame, dummyContexts);
            if (auto1.ShouldFalldown(frame))
            {
                Debug.LogError("faild to inherit frame:" + frame);
                return;
            }
            frame++;
        }

        if (auto1.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed to inherit tacks");
    }

    public void _7_7_InheritTimelineThenRunTacks()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_7_0",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_7_0TL_0",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_7_0R_0",
                    0,
                    100,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._FiveFrameThenBreakTimeline;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_7_0TL_1",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_7_0R_1",
                    0,
                    100,
                    AutoConditions.Anim.DEFAULT,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._SixFrameThenBreakTimeline;
                        return c;
                    }
                )
            )
        );

        auto0.Update(frame, dummyContexts);
        frame++;
        // rest 4 times and 5 times

        var auto1 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_7_1",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_7_1TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_7_1R",
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

        var inheritTimelines = auto0.ExportTimelines(typeof(AutoConditions.Act), typeof(AutoConditions.Anim));
        auto1.InheritTimelines(inheritTimelines);

        for (int i = 0; i < 5; i++)
        {
            auto1.Update(frame, dummyContexts);
            if (auto1.ShouldFalldown(frame))
            {
                Debug.LogError("faild to inherit frame:" + frame);
                return;
            }
            frame++;
        }

        if (auto1.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed to inherit tacks");
    }


    public void _7_8_InheritTimelineMultiTimes()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_8_0",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_8_0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_8_0R",
                    0,
                    5,
                    AutoConditions.Anim.DEFAULT,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._FiveFrameThenBreakAuto;
                        return c;
                    }
                )
            )
        );

        auto0.Update(frame, dummyContexts);
        frame++;
        // rest 4 times

        var auto1 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_8_1",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_8_1TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_8_1R",
                    0,
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

        {
            var inheritTimelines1 = auto0.ExportTimelines(typeof(AutoConditions.Anim));
            auto1.InheritTimelines(inheritTimelines1);
        }

        auto1.Update(frame, dummyContexts);
        frame++;
        // rest 3 times from auto0

        var auto2 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_8_2",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_8_2TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_8_2R",
                    0,
                    100,
                    AutoConditions.Act.P2,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        {
            var inheritTimelines2 = auto1.ExportTimelines(typeof(AutoConditions.Anim));
            auto2.InheritTimelines(inheritTimelines2);
        }

        for (int i = 0; i < 3; i++)
        {
            if (auto2.ShouldFalldown(frame))
            {
                Debug.LogError("failed to inherit");
                return;
            }

            auto2.Update(frame, dummyContexts);
            frame++;
        }

        if (auto2.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed to inherit");
    }

    public void _7_9_InheritTimelineMultiTimes2()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_9_0",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_9_0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_9_0R",
                    0,
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
        frame++;
        // rest 9 in auto0, will be inherited.

        var auto1 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_9_1",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_9_1TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_9_1R",
                    0,
                    100,
                    AutoConditions.Act.P1,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._FiveFrameThenBreakTimeline;
                        return c;
                    }
                )
            )
        );

        {
            var inheritTimelines1 = auto0.ExportTimelines(typeof(AutoConditions.Anim));
            auto1.InheritTimelines(inheritTimelines1);
        }

        auto1.Update(frame, dummyContexts);
        frame++;
        // rest 8 times from auto0, rest 4 times from auto1.

        var auto2 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "7_9_2",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "7_9_2TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "7_9_2R",
                    0,
                    5,
                    AutoConditions.Act.P2,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        {
            var inheritTimelines2 = auto1.ExportTimelines(typeof(AutoConditions.Anim));
            auto2.InheritTimelines(inheritTimelines2);
        }

        // auto0-timeline will be consumed in next 8.
        // auto1-timeline will be consumed in next 4.
        // auto2-timeline will be consumed in next 5.
        for (int i = 0; i < 8; i++)
        {
            auto2.Update(frame, dummyContexts);
            if (auto2.ShouldFalldown(frame))
            {
                Debug.LogError("failed to inherit");
                return;
            }
            frame++;
        }

        if (auto2.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed to inherit");
    }
}






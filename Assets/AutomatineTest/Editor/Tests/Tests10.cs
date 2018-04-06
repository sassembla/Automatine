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

    public void _10_0_RoutineCanRetrieveParentAutoId()
    {

        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine0",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._AutoIdSeeker;
                        return c;
                    }
                )
            )
        );

        auto0.Update(frame, dummyContexts);
        frame++;

        // inherit to other auto
        var context1 = PlayerContext.Copy(dummyContext);
        var auto1 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine1",
            frame,
            context1,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine1TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine1Tack",
                    0,
                    10,
                    AutoConditions.Act.P1,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakAutoAtLastFrame;
                        return c;
                    }
                )
            )
        );

        var inhetitTimelines = auto0.ExportTimelines(typeof(AutoConditions.Act));
        auto1.InheritTimelines(inhetitTimelines);

        auto1.Update(frame, dummyContexts);
        frame++;// rest 9 times

        for (int i = 0; i < 9; i++)
        {
            auto1.Update(frame, dummyContexts);
            if (auto1.ShouldFalldown(frame))
            {
                Debug.LogError("failed to retrieve autoId in inherited routine");
            }
            frame++;
        }

        if (auto1.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed to retrieve autoId in inherited routine");
    }

    public void _10_1_AssumeSpan()
    {
        int frame = 0;
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
                    0,
                    5,
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

        var assumeSpan = auto0.AssumedRestFrame();
        if (assumeSpan == 9)
        {
            return;
        }

        Debug.LogError("failed to assume span.");
    }

    public void _10_2_AssumeSpanWithInfinity()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine0",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
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

        var assumeSpan = auto0.AssumedRestFrame();
        if (assumeSpan == AutomatineDefinitions.Tack.LIMIT_UNLIMITED)
        {
            return;
        }

        Debug.LogError("failed to assume span. assumeSpan:" + assumeSpan);
    }

    public void _10_3_AssumeSpanWithInfinityAndLimited()
    {
        int frame = 0;
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
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
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

        var assumeSpan = auto0.AssumedRestFrame();
        if (assumeSpan == AutomatineDefinitions.Tack.LIMIT_UNLIMITED)
        {
            return;
        }

        Debug.LogError("failed to assume span. assumeSpan:" + assumeSpan);
    }

    public void _10_4_AssumeSpanWithInherit()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine0",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._AutoIdSeeker;
                        return c;
                    }
                )
            )
        );

        var context1 = PlayerContext.Copy(dummyContext);
        var auto1 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine1",
            frame,
            context1,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine1TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine1Tack",
                    0,
                    10,
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

        var inhetitTimelines = auto0.ExportTimelines(typeof(AutoConditions.Act));
        auto1.InheritTimelines(inhetitTimelines);

        var assumeSpan = auto1.AssumedRestFrame();
        if (assumeSpan == AutomatineDefinitions.Tack.LIMIT_UNLIMITED)
        {
            return;
        }

        Debug.LogError("failed to assume span. assumeSpan:" + assumeSpan);
    }

    public void _10_5_AssumeSpanInProgress()
    {
        int frame = 0;
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
                    0,
                    5,
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

        auto0.Update(frame, dummyContexts);

        var assumeSpan = auto0.AssumedRestFrame();
        if (assumeSpan == 8)
        {
            return;
        }

        Debug.LogError("failed to assume span. assumeSpan:" + assumeSpan);
    }

    public void _10_6_AssumeSpanWithInfinityInProgress()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine0",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
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

        var assumeSpan = auto0.AssumedRestFrame();
        if (assumeSpan == AutomatineDefinitions.Tack.LIMIT_UNLIMITED)
        {
            return;
        }

        Debug.LogError("failed to assume span. assumeSpan:" + assumeSpan);
    }

    public void _10_7_AssumeSpanWithInfinityAndLimitedInProgress()
    {
        int frame = 0;
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
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
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

        var assumeSpan = auto0.AssumedRestFrame();
        if (assumeSpan == AutomatineDefinitions.Tack.LIMIT_UNLIMITED)
        {
            return;
        }

        Debug.LogError("failed to assume span. assumeSpan:" + assumeSpan);
    }

    public void _10_8_AssumeSpanWithInheritInProgress()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine0",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._AutoIdSeeker;
                        return c;
                    }
                )
            )
        );

        auto0.Update(frame, dummyContexts);
        frame++;

        var context1 = PlayerContext.Copy(dummyContext);
        var auto1 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine1",
            frame,
            context1,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine1TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine1Tack",
                    0,
                    10,
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

        var inhetitTimelines = auto0.ExportTimelines(typeof(AutoConditions.Act));
        auto1.InheritTimelines(inhetitTimelines);

        var assumeSpan = auto1.AssumedRestFrame();
        if (assumeSpan == AutomatineDefinitions.Tack.LIMIT_UNLIMITED)
        {
            return;
        }

        Debug.LogError("failed to assume span. assumeSpan:" + assumeSpan);
    }

    public void _10_9_RoutineRetriveRestFrameOfTack()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine0",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack",
                    0,
                    10,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakAutoBeforeLastFrame;
                        return c;
                    }
                )
            )
        );

        for (int i = 0; i < 9; i++)
        {
            auto0.Update(frame, dummyContexts);
            if (auto0.ShouldFalldown(frame))
            {
                Debug.LogError("failed to retrieve before last frame");
                return;
            }
            frame++;
        }

        if (auto0.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed to retrieve before last frame");
    }

    public void _10_10_RoutineRetrieveCondition()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine0",
            frame,
            context0,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
                    AutoConditions.Act.P0,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakAutoIfConditionContainsSpecific;
                        return c;
                    }
                )
            )
        );

        auto0.Update(frame, dummyContexts);
        frame++;

        if (auto0.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed to retrieve condition");
    }

    public void _10_11_RoutineRetrieveConditions()
    {
        int frame = 0;
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
                    1,
                    AutoConditions.Anim.DEFAULT,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._BreakAutoIfConditionsContainsSpecific;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL_1",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_1",
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
        frame++;

        if (auto0.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed to retrieve conditions");
    }
}






using UnityEngine;

using System;
using System.Collections.Generic;
using Automatine;


/**
	spaceがある場合のAutoのrest
*/
public partial class Test
{
    public void _14_0_AutoWithSpace_AssumeSpan()
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
                    AutoConditions.Act.P0
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    100,
                    1,
                    AutoConditions.Act.P0
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
        if (assumeSpan == 100)
        {
            return;
        }

        Debug.LogError("failed to assume span. assumeSpan:" + assumeSpan);
    }

    public void _14_1_AutoWithSpace_AssumeSpan_MoreSpan()
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
                    AutoConditions.Act.P0
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    100,
                    100,
                    AutoConditions.Act.P0
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
        if (assumeSpan == 199)
        {
            return;
        }

        Debug.LogError("failed to assume span. assumeSpan:" + assumeSpan);
    }

    public void _14_2_AutoWithSpace_AssumeSpan_MoreSpan_IgnoreStartFrame()
    {
        int frame = 1000;
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
                    AutoConditions.Act.P0
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    100,
                    100,
                    AutoConditions.Act.P0
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
        if (assumeSpan == 199)
        {
            return;
        }

        Debug.LogError("failed to assume span. assumeSpan:" + assumeSpan);
    }
}
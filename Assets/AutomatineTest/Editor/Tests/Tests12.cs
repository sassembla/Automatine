using UnityEngine;

using System;
using System.Collections.Generic;
using Automatine;



// Error

public partial class Test
{

    public void _12_0_TimelineConditionCollectionError()
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
                    1,
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

        try
        {
            auto0.Conditions();
            Debug.LogError("failed to express invalid conditions");
        }
        catch
        {

        }
    }

    public void _12_1_TimelineExportError()
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
                    1,
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

        var inheritTimelines = auto0.ExportTimelines(typeof(AutoConditions.Act));

        if (inheritTimelines.Count == 1)
        {
            return;
        }


        Debug.LogError("failed to export invalid conditions");
    }


}
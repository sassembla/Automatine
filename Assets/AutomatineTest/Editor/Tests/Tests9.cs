using UnityEngine;

using System;
using System.Collections.Generic;
using Automatine;



// Test

/**
	Break
*/
public partial class Test
{

    public void _9_0_BreakTackWithInherit()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "9_0_0",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "9_0_0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "9_0_0R_1",
                    100,
                    100,
                    AutoConditions.Act.P1,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._FiveFrameThenBreakAuto;
                        return c;
                    }
                )
            )
        );

        var auto1 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "9_0_1",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "9_0_1TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "9_0_1R",
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

        var inheritTimelines = auto0.ExportTimelines(typeof(AutoConditions.Act));
        // 100 ~ 200 with tack [_FiveFrameThenBreakAuto] will export. 

        auto1.InheritTimelines(inheritTimelines);

        for (int i = 0; i < 105; i++)
        {
            if (auto1.ShouldFalldown(frame))
            {
                Debug.LogError("failed to inherit 1.");
                return;
            }
            auto1.Update(frame, dummyContexts);
            frame++;
        }

        if (auto1.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed to inherit 2.");
    }
}






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

    public void _8_0_ConditionWithInheritTimeline()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "8_0_0",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "8_0_0TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "8_0_0R",
                    0,
                    5,
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

        // rest 4 frames for tack.
        auto0.Update(frame, dummyContexts);
        frame++;

        var auto1 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "8_0_1",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "8_0_1TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "8_0_1R",
                    0,
                    5,
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

        var inheritTimelines = auto0.ExportTimelines(typeof(AutoConditions.Act));
        auto1.InheritTimelines(inheritTimelines);

        var conditions = auto1.Conditions();
        if (ConditionGateway.Contains(conditions, AutoConditions.Anim.DEFAULT))
        {
            if (ConditionGateway.NotContains(conditions, AutoConditions.Act.FIVEFRAME_SPAN))
            {
                return;
            }
        }

        Debug.LogError("not match");
    }

    public void _8_1_ConditionWithInheritTimelines()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "8_1_0",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "8_1_0TL_0",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "8_1_0R_0",
                    0,
                    5,
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
                "8_1_0TL_1",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "8_1_0R_1",
                    0,
                    5,
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

        // rest 4 frames for tack.
        auto0.Update(frame, dummyContexts);
        frame++;

        var auto1 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "8_1_1",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "8_1_1TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "8_1_1R",
                    0,
                    5,
                    AutoConditions.Anim.SPAWN,
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

        var conditions = auto1.Conditions();
        if (ConditionGateway.Contains(conditions, AutoConditions.Anim.SPAWN))
        {
            if (ConditionGateway.NotContainsAll(conditions, AutoConditions.Act.FIVEFRAME_SPAN, AutoConditions.Anim.DEFAULT))
            {
                return;
            }
        }

        Debug.LogError("not match");
    }
}
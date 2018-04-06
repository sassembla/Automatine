using UnityEngine;

using System;
using System.Collections.Generic;
using Automatine;


/**
	spaceがある場合のAutoのrest
*/
public partial class Test
{
    public void _15_0_AutoWithSpace_Conditions()
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
                    0,// from 0, 10frame -> 0 ~ 9
                    10,
                    AutoConditions.Act.P0
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    100,// from 100, 100frame -> 100 ~ 199
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
                    AutoConditions.Action.Down,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        for (var i = 0; i <= 10; i++)
        {// frame 0 ~ 9 contains P0, frame 10 is not contained.
            auto0.Update(frame, dummyContexts);
            frame++;
        }

        var cond0 = auto0.Conditions();
        if (ConditionGateway.NotContains(cond0, AutoConditions.Act.P0))
        {
            return;
        }

        Debug.LogError("not match.");
    }

    public void _15_1_AutoWithSpace_Conditions_NextIs100Frame()
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
                    0,// from 0, 10frame -> 0 ~ 9
                    10,
                    AutoConditions.Act.P0
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    100,// from 100, 100frame -> 100 ~ 199
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

        for (var i = 0; i <= 100; i++)
        {
            auto0.Update(frame, dummyContexts);
            frame++;
        }

        // frame is 100.
        auto0.Update(frame, dummyContexts);


        var cond0 = auto0.Conditions();
        if (ConditionGateway.Contains(cond0, AutoConditions.Act.P0))
        {
            return;
        }

        Debug.LogError("not match.");
    }


    public void _15_2_AutoWithSpace_Conditions_KeepCondition()
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
                    0,// from 0, 10frame -> 0 ~ 9
                    10,
                    AutoConditions.Act.P0
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    100,// from 100, 100frame -> 100 ~ 199
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

        while (frame < 199)
        {
            auto0.Update(frame, dummyContexts);
            frame++;
        }

        // frame is 199.
        auto0.Update(frame, dummyContexts);


        var cond0 = auto0.Conditions();
        if (ConditionGateway.Contains(cond0, AutoConditions.Act.P0))
        {
            return;
        }

        Debug.LogError("not match.");
    }

    public void _15_3_AutoWithSpace_Conditions_KeepCondition_Later()
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
                    0,// from 0, 10frame -> 0 ~ 9
                    10,
                    AutoConditions.Act.P0
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    100,// from 100, 100frame -> 100 ~ 199
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

        while (frame < 200)
        {
            auto0.Update(frame, dummyContexts);
            frame++;
        }

        // frame is 200.
        auto0.Update(frame, dummyContexts);

        var cond0 = auto0.Conditions();
        if (ConditionGateway.NotContains(cond0, AutoConditions.Act.P0))
        {
            return;
        }

        Debug.LogError("not match.");
    }
}
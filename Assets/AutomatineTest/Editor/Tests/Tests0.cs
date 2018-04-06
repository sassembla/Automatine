using UnityEngine;

using System;
using System.Collections.Generic;
using Automatine;



// Test

public partial class Test
{

    /**
		SampleRoutineを一回走らせる
	*/
    public void _0_0_RunRoutine()
    {
        var N = 1;

        int frame = 0;
        var context1 = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine",
            frame,
            context1,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutineTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutineTack",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
                    AutoConditions.Act.SAMPLEROUTINE,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.SampleRoutine;
                        return c;
                    }
                )
            )
        );

        for (int i = 0; i < N; i++)
        {
            auto.Update(frame + i, dummyContexts);
        }

        // if routine runs, life == N.
        if (dummyContext.life == N)
        {
            return;
        }

        Debug.LogError("failed, dummyContext.life:" + dummyContext.life);
    }

    /**
		SampleRoutineをN回走らせる
	*/
    public void _0_1_RunRoutine_NTimes()
    {
        var N = 100;

        int frame = 0;
        var context1 = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine",
            frame,
            context1,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutineTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutineTack",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
                    AutoConditions.Act.SAMPLEROUTINE,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.SampleRoutine;
                        return c;
                    }
                )
            )
        );

        for (int i = 0; i < N; i++)
        {
            auto.Update(frame + i, dummyContexts);
        }

        // if routine runs, life == N.
        if (dummyContext.life == N)
        {
            return;
        }

        Debug.LogError("failed, dummyContext.life:" + dummyContext.life);
    }


    /**
		複数のTimelineでSampleRoutineを1回走らせる
	*/
    public void _0_2_RunRoutine_2Timelines()
    {
        var N = 1;

        int frame = 0;
        var context1 = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine",
            frame,
            context1,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutineTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutineTack",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
                    AutoConditions.Act.SAMPLEROUTINE,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.SampleRoutine;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutineTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutineTack",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
                    AutoConditions.Act.SAMPLEROUTINE,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.SampleRoutine;
                        return c;
                    }
                )
            )
        );

        for (int i = 0; i < N; i++)
        {
            auto.Update(frame + i, dummyContexts);
        }

        // if routine runs, life == N * 2.
        if (dummyContext.life == N * 2)
        {
            return;
        }

        Debug.LogError("failed, dummyContext.life:" + dummyContext.life);
    }

    /**
		複数のTimelineでSampleRoutineをN回走らせる
	*/
    public void _0_3_RunRoutine_2Timelines_NTimes()
    {
        var N = 100;

        int frame = 0;
        var context1 = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine",
            frame,
            context1,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutineTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutineTack",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
                    AutoConditions.Act.SAMPLEROUTINE,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.SampleRoutine;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutineTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutineTack",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
                    AutoConditions.Act.SAMPLEROUTINE,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.SampleRoutine;
                        return c;
                    }
                )
            )
        );

        for (int i = 0; i < N; i++)
        {
            auto.Update(frame + i, dummyContexts);
        }

        // if routine runs, life == N * 2.
        if (dummyContext.life == N * 2)
        {
            return;
        }

        Debug.LogError("failed, dummyContext.life:" + dummyContext.life);
    }

    public void _0_4_RunRoutine_2Timelines_2ndTimelineFirst()
    {

        int frame = 0;
        var context1 = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine",
            frame,
            context1,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutineTL0",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutineTack",
                    1,
                    1,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.Should2nd;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutineTL1",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutineTack",
                    0,
                    1,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.Should1st;
                        return c;
                    }
                )
            )
        );


        auto.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        auto.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        var runMark = context1.runMark;
        if (runMark == "1st2nd")
        {
            return;
        }

        Debug.LogError("failed. runMark:" + runMark);
    }

    public void _0_5_RunRoutine_2Timelines_2ndTimelineFirst_MultiTack()
    {

        int frame = 0;
        var context1 = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine",
            frame,
            context1,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutineTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutineTack",
                    0,
                    1,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.Should1st;
                        return c;
                    }
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutineTack",
                    1,
                    1,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.Should3rd;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutineTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutineTack",
                    0,
                    1,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.Should2nd;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;
        auto.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        var runMark = context1.runMark;
        if (runMark == "1st2nd3rd")
        {
            return;
        }

        Debug.LogError("failed. runMark:" + runMark);
    }

    public void _0_6_RunRoutine_2Timelines_2ndTimelineFirst_Many_MultiTack()
    {

        int frame = 0;
        var context1 = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "RunRoutine",
            frame,
            context1,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutineTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutineTack",
                    2,
                    1,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.Should3rd;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutineTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutineTack",
                    1,
                    1,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.Should2nd;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutineTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutineTack",
                    0,
                    1,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.Should1st;
                        return c;
                    }
                )
            )
        );

        auto.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;
        auto.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;
        auto.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        var runMark = context1.runMark;
        if (runMark == "1st2nd3rd")
        {
            return;
        }

        Debug.LogError("failed. runMark:" + runMark);
    }
}
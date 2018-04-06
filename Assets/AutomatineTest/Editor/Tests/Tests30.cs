using UnityEngine;

using System;
using System.Collections.Generic;
using Automatine;



// Changer

public partial class Test
{

    public void _30_0_ChangerWithResultOfTest24_TestRoute0()
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
                    AutoConditions.Act.DEFAULT,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        auto0 = Changer_TestRoute0.TestRoute0(auto0, frame, context0);
        var conditions = auto0.Conditions();

        if (conditions.ContainsCondition(AutoConditions.Move.WALK_READY))
        {
            return;
        }
        Debug.LogError("failed to run behaviour");
    }

    public void _30_1_ChangerWithResultOfTest24_TestRoute1()
    {
        // int frame = 100;
        // var context0 = PlayerContext.Copy(dummyContext);

        // var auto0 = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
        // 	"RunRoutine0", 
        // 	frame,
        // 	context0,
        // 	new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
        // 		"RunRoutine0TL_0",
        // 		new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
        // 			"RunRoutine0Tack_0",
        // 			0,
        // 			1,
        // 			AutoConditions.Act.DEFAULT,
        // 			() => {
        // 				var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
        // 				c.rContext = c._Infinity;
        // 				return c;
        // 			}
        // 		)
        // 	)
        // );


        // not tested yet.

        // auto0 = Changer_TestRoute1.TestRoute1(auto0, frame, context0);
        // var conditions = auto0.Conditions();

        // if (conditions.ContainsCondition(AutoConditions.Move.WALK_READY)) {
        // 	return;
        // }
        // Debug.LogError("failed to run behaviour");
    }

    public void _30_2_ChangerWithResultOfTest24_TestRoute2()
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
                    AutoConditions.Act.DEFAULT,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        auto0 = Changer_TestRoute2.TestRoute2(auto0, frame, context0);
        var conditions = auto0.Conditions();

        if (conditions.ContainsCondition(AutoConditions.Move.WALK_READY))
        {
            return;
        }
        Debug.LogError("failed to run behaviour");
    }

    public void _30_3_ChangerWithResultOfTest24_TestRoute3()
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
            )
        );

        auto0 = Changer_TestRoute3.TestRoute3(auto0, frame, context0);
        var conditions = auto0.Conditions();

        if (conditions.ContainsCondition(AutoConditions.Move.WALK_READY))
        {
            return;
        }
        Debug.LogError("failed to run behaviour");
    }

}
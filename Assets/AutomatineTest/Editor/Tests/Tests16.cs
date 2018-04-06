using UnityEngine;
using System.Collections.Generic;
using Automatine;

/**
	typeが指定されてないTack
*/
public partial class Test
{
    public void _16_0_TackWithoutType()
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
                    10
                )
            )
        );

        var conditions = auto0.Conditions();
        foreach (var condition in conditions)
        {
            if (condition != -1) Debug.LogError("failed, contains condition:" + condition);
        }
    }

    public void _16_1_EmptyFrameType()
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
                    1,
                    1,
                    AutoConditions.Act.P0
                )
            )
        );

        // before running.
        var conditions0 = auto0.Conditions();
        if (conditions0.ContainsCondition(AutoConditions.Act.P0))
        {
            Debug.LogError("failed.");
            return;
        }

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        // 1st frame. = before of 2nd update. 
        var conditions1 = auto0.Conditions();
        if (conditions1.ContainsCondition(AutoConditions.Act.P0))
        {
            Debug.LogError("failed.");
            return;
        }

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        // 2nd frame.
        var conditions2 = auto0.Conditions();
        if (!conditions2.ContainsCondition(AutoConditions.Act.P0))
        {
            Debug.LogError("failed.");
            return;
        }
    }

    public void _16_2_EmptyFrameTypeAfterUpdate()
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
                    2,
                    1,
                    AutoConditions.Act.P0
                )
            )
        );

        // 0 should not contains condition.
        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;
        {
            var conditions = auto0.Conditions();

            if (conditions.ContainsCondition(AutoConditions.Act.P0))
            {
                Debug.LogError("failed.");
                return;
            }
        }

        // 1 should not contains condition.
        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;
        {
            var conditions = auto0.Conditions();

            if (conditions.ContainsCondition(AutoConditions.Act.P0))
            {
                Debug.LogError("failed.");
                return;
            }
        }

        // 2 should contains condition.
        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;
        {
            var conditions = auto0.Conditions();

            if (conditions.ContainsCondition(AutoConditions.Act.P0))
            {
                return;
            }
        }



        Debug.LogError("failed");
    }

    public void _16_3_EmptyFrameThenConditions()
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
                    3,
                    10,
                    AutoConditions.Act.P0
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    10,
                    5,
                    AutoConditions.Act.DAMAGE
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    15,
                    5,
                    AutoConditions.Act.FIVEFRAME_SPAN
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "RunRoutine0TL_0",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "RunRoutine0Tack_0",
                    12,
                    3,
                    AutoConditions.Anim.SPAWN
                )
            )
        );

        // before update.
        {
            var conditions = auto0.Conditions();

            if (conditions.ContainsCondition(AutoConditions.Act.P0))
            {
                Debug.LogError("should not contains.");
                return;
            }
        }


        // 0.
        auto0.Update(frame, dummyContexts);
        frame++;
        {
            var conditions = auto0.Conditions();

            if (conditions.ContainsCondition(AutoConditions.Act.P0))
            {
                Debug.LogError("should not contains.");
                return;
            }
        }


        // 1.
        auto0.Update(frame, dummyContexts);
        frame++;
        {
            var conditions = auto0.Conditions();

            if (conditions.ContainsCondition(AutoConditions.Act.P0))
            {
                Debug.LogError("should not contains.");
                return;
            }
        }


        // 2.
        auto0.Update(frame, dummyContexts);
        frame++;
        {
            var conditions = auto0.Conditions();

            if (conditions.ContainsCondition(AutoConditions.Act.P0))
            {
                Debug.LogError("should not contains.");
                return;
            }
        }

        // 3.
        auto0.Update(frame, dummyContexts);
        frame++;
        {
            var conditions = auto0.Conditions();

            if (conditions.ContainsCondition(AutoConditions.Act.P0))
            {
                return;
            }
        }

        Debug.LogError("failed.");
    }
}
using UnityEngine;

using System;
using System.Collections.Generic;
using Automatine;



// Test

public partial class Test
{

    /**
		span = 3であれば、3回実行されて終わるはず。
	*/
    public void _2_0_AtEnd()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var start = 0;
        var span = 3;

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            span + "fで終わるAuto",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    span + "フレームで終了するroutine",
                    start,
                    span,
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

        int i = 0;
        while (true)
        {
            auto.Update(frame, dummyContexts);
            if (auto.ShouldFalldownInNextFrame(frame))
            {
                break;
            }

            i++;
            if (span == i)
            {
                Debug.LogError("timeout i:" + i + " vs autoInfo:" + auto.autoInfo);
                break;
            }
            frame = frame + 1;
        }
    }

    /**
		開始フレームが異なる場合
	*/
    public void _2_1_AtEndWith1Start()
    {
        int frame = 1;
        var context = PlayerContext.Copy(dummyContext);

        var span = 3;
        var start = 0;

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            span + "fで終わるAuto",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "TL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    span + "フレームで終了するroutine",
                    start,
                    span,
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

        int i = 0;
        while (true)
        {
            auto.Update(frame, dummyContexts);
            if (auto.ShouldFalldownInNextFrame(frame))
            {
                break;
            }

            i++;
            if (span == i)
            {
                Debug.LogError("timeout i:" + i + " vs autoInfo:" + auto.autoInfo);
                break;
            }
            frame = frame + 1;
        }
    }

    /**
		特定のconditionが特定のフレーム数存在している
	*/
    public void _2_2_ConditionContinuesIn10Frame()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var start = 0;
        var span = 10;

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            span + "フレーム、特定のconditionを維持する",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                span + "フレーム、特定のconditionを維持するTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    span + "フレーム、特定のconditionを維持するTack",
                    start,
                    span,
                    AutoConditions.Act.TENFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        int count = 0;
        for (int i = 0; i < span; i++)
        {
            auto.Update(frame, dummyContexts);
            var conditions = auto.Conditions();

            if (ConditionGateway.Contains(conditions, AutoConditions.Act.TENFRAME_SPAN))
            {
                count++;
            }
            else
            {
                Debug.LogError("not contains! i:" + i);
            }
            frame = frame + 1;
        }

        if (count == span)
        {

        }
        else
        {
            Debug.LogError("failed. not match count:" + count);
        }
    }


    /**
		特定のconditionが特定のフレーム数存在している
		二つのTackにまたがる場合
	*/
    public void _2_3_ConditionContinuesIn10FrameBetween2Tack()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "5フレーム、特定のconditionを維持する",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "5フレーム、特定のconditionを維持するTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "5フレーム、特定のconditionを維持するTack",
                    0,
                    5,
                    AutoConditions.Act.TENFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "さらに5フレーム、特定のconditionを維持するTack",
                    5,
                    5,
                    AutoConditions.Act.TENFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        int count = 0;
        for (int i = 0; i < 10; i++)
        {
            auto.Update(frame, dummyContexts);
            var conditions = auto.Conditions();

            if (ConditionGateway.Contains(conditions, AutoConditions.Act.TENFRAME_SPAN))
            {
                count++;
            }
            frame = frame + 1;
        }

        if (count == 10)
        {

        }
        else
        {
            Debug.LogError("failed. not match count:" + count);
        }
    }


    /**
		特定のconditionが特定のフレーム数存在している
		二つのTackにまたがり、中間に別のものが詰まっている場合
	*/
    public void _2_4_ConditionContinuesIn15FrameBetween3Tack()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "10フレーム、特定のconditionを維持する",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "5フレーム、特定のconditionを維持するTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "5フレーム、特定のconditionを維持するTack",
                    0,
                    5,
                    AutoConditions.Act.TENFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "中間で5フレーム、特定のconditionを維持するTack",
                    5,
                    5,
                    AutoConditions.Act.FIVEFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                ),
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "さらに5フレーム、特定のconditionを維持するTack",
                    10,
                    5,
                    AutoConditions.Act.TENFRAME_SPAN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c._Infinity;
                        return c;
                    }
                )
            )
        );

        int count = 0;
        for (int i = 0; i < 15; i++)
        {
            auto.Update(frame, dummyContexts);
            var conditions = auto.Conditions();

            if (ConditionGateway.Contains(conditions, AutoConditions.Act.TENFRAME_SPAN))
            {
                count++;
            }
            frame = frame + 1;
        }

        if (count == 10)
        {

        }
        else
        {
            Debug.LogError("failed. not match count:" + count);
        }
    }



}
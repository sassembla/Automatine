using UnityEngine;

using System;
using System.Collections.Generic;
using Automatine;



// Test

public partial class Test
{

    public void _1_0_GerenateAutoThenFalldown()
    {
        int frame = 0;
        var context1 = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "spawn開始",
            frame,
            context1,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "Spawn処理",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "spawning",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
                    AutoConditions.Act.SPAWN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.Spawn;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "Spawn時のモーション",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "まだ真面目にセットしてない、Spawn時のモーション",
                    0,
                    10,
                    AutoConditions.Anim.SPAWN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.SpawnMotion;
                        return c;
                    }
                )
            )
        );

        int i = 0;
        while (true)
        {
            if (auto.ShouldFalldown(frame))
            {
                break;
            }

            auto.Update(frame, dummyContexts);

            i++;
            if (101 < i)
            {
                Debug.LogError("timeout i:" + i + " vs autoInfo:" + auto.autoInfo);
                break;
            }

            frame++;
        }
    }

    public void _1_1_RunContainsSpecificConditionInTimelines()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "spawn開始",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "Spawn処理",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "spawning",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
                    AutoConditions.Act.SPAWN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.Spawn;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "Spawn時のモーション",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "まだ真面目にセットしてない、Spawn時のモーション",
                    0,
                    10,
                    AutoConditions.Anim.SPAWN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.SpawnMotion;
                        return c;
                    }
                )
            )
        );

        var conditions = auto.Conditions();

        if (ConditionGateway.Contains(conditions, AutoConditions.Act.SPAWN))
        {
            // no problem
        }
        else
        {
            Debug.LogError("not contains act SPAWN");
        }

        if (ConditionGateway.Contains(conditions, AutoConditions.Anim.SPAWN))
        {
            // no problem
        }
        else
        {
            Debug.LogError("not contains anim SPAWN");
        }
    }

    public void _1_2_RunContainsAllSpecificConditionInTimelines()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "spawn開始",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "Spawn処理",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "spawning",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
                    AutoConditions.Act.SPAWN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.Spawn;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "Spawn時のモーション",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "まだ真面目にセットしてない、Spawn時のモーション",
                    0,
                    10,
                    AutoConditions.Anim.SPAWN,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.SpawnMotion;
                        return c;
                    }
                )
            )
        );

        var conditions = auto.Conditions();

        if (ConditionGateway.ContainsAll(conditions, AutoConditions.Act.SPAWN, AutoConditions.Anim.SPAWN))
        {
            // no problem
        }
        else
        {
            Debug.LogError("not contains act & anim SPAWN");
        }
    }

    public void _1_3_RunNotContainsSpecificConditionInTimeline()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "デフォルト状態",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "基本モーション",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "基礎行動",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
                    AutoConditions.Act.DEFAULT,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.Default;
                        return c;
                    }
                )
            )
        );

        var conditions = auto.Conditions();

        if (ConditionGateway.NotContains(conditions, AutoConditions.Act.SPAWN))
        {
            // no problem
        }
        else
        {
            Debug.LogError("not contains act SPAWN");
        }

        if (ConditionGateway.NotContains(conditions, AutoConditions.Anim.SPAWN))
        {
            // no problem
        }
        else
        {
            Debug.LogError("not contains anim SPAWN");
        }
    }

    public void _1_4_RunNotContainsAllSpecificConditionInTimelines()
    {
        int frame = 0;
        var context = PlayerContext.Copy(dummyContext);

        var auto = new Auto<PlayerContext, Dictionary<string, PlayerContext>>(
            "デフォルト状態",
            frame,
            context,
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "基本モーション",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "基礎行動",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
                    AutoConditions.Act.DEFAULT,
                    () =>
                    {
                        var c = new RoutineContexts<PlayerContext, Dictionary<string, PlayerContext>>();
                        c.rContext = c.Default;
                        return c;
                    }
                )
            ),
            new Timeline<PlayerContext, Dictionary<string, PlayerContext>>(
                "アニメーションTL",
                new Tack<PlayerContext, Dictionary<string, PlayerContext>>(
                    "アニメーションTack",
                    0,
                    AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
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

        var conditions = auto.Conditions();

        if (ConditionGateway.NotContainsAll(conditions, AutoConditions.Act.SPAWN, AutoConditions.Anim.SPAWN))
        {
            // no problem
        }
        else
        {
            Debug.LogError("not contains act & anim SPAWN");
        }
    }



}
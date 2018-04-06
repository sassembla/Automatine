using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Automatine;
using UnityEngine;


/**
	reset series
*/
public partial class Test
{
    public void _41_0_ResetFrame()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest41<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;// frame = 1.

        frame = 100;
        auto0.ResetFrame(frame);

        if (auto0.ShouldFalldown(frame))
        {
            Debug.LogError("failed, not shouldFalldown.");
            return;
        }

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;// frame = 101.

        if (auto0.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed.");
    }

    public void _41_1_ResetFrameWithSameFrame()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest41<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

        auto0.Update(frame, new Dictionary<string, PlayerContext>());

        frame = 0;
        auto0.ResetFrame(frame);

        if (auto0.ShouldFalldown(frame))
        {
            Debug.LogError("failed, not shouldFalldown.");
            return;
        }

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;// frame = 1.

        if (auto0.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed.");
    }

    public void _41_2_ResetFrameBeforeUpdateThenUpdate()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest41_1<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

        frame = 100;
        auto0.ResetFrame(frame);

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;// frame = 101.

        if (auto0.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed.");
    }

    public void _41_3_ResetFrameWithInheritTimeline()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest41_2<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

        auto0.Update(frame, new Dictionary<string, PlayerContext>());
        // rest 1 frame.

        // reset after auto0 1st update.
        frame = 100;
        auto0.ResetFrame(frame);

        // inherit afrer update.
        var inherits = auto0.ExportTimelines(typeof(AutoConditions.Act));
        var auto1 = new AutoForTest41_3<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

        auto1.InheritTimelines(inherits);
        // inherited timelise is still rest 1 frame.

        auto1.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        if (auto1.ShouldFalldown(frame)) return;

        Debug.LogError("failed.");
    }


    public void _41_4_ResetFrameWithInheritTimelineInheritBeforeUpdate()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest41_2<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

        // inherit before update.
        var inherits = auto0.ExportTimelines(typeof(AutoConditions.Act));
        var auto1 = new AutoForTest41_3<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);
        auto1.InheritTimelines(inherits);

        auto1.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;
        // rest 1 frame.

        // reset after 1st update.
        frame = 100;
        auto0.ResetFrame(frame);

        auto1.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        if (auto1.ShouldFalldown(frame)) return;

        Debug.LogError("failed.");
    }

    public void _41_5_ResetFrameWithInheritTimelineInheritAfterUpdate()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = new AutoForTest41_2<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

        // inherit before update.
        var inherits = auto0.ExportTimelines(typeof(AutoConditions.Act));
        var auto1 = new AutoForTest41_3<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);
        auto1.InheritTimelines(inherits);

        auto1.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;
        // rest 1 frame.

        auto1.Update(frame, new Dictionary<string, PlayerContext>());
        frame++;

        // reset after 2nd update.
        frame = 100;
        auto0.ResetFrame(frame);

        if (auto1.ShouldFalldown(frame)) return;

        Debug.LogError("failed.");
    }

}

public class AutoForTest41<InitialParamType, UpdateParamType> : Auto<InitialParamType, UpdateParamType>
{
    public AutoForTest41(int frame, InitialParamType context) : base(
        "test41 sample auto",
        frame,
        context,
        new Timeline<InitialParamType, UpdateParamType>(
            "timelineTitle",
            new Tack<InitialParamType, UpdateParamType>(
                "tackTitle",
                0,
                2,
                AutoConditions.Act.P0
            )
        )
    )
    { }
}

public class AutoForTest41_1<InitialParamType, UpdateParamType> : Auto<InitialParamType, UpdateParamType>
{
    public AutoForTest41_1(int frame, InitialParamType context) : base(
        "test41 sample auto",
        frame,
        context,
        new Timeline<InitialParamType, UpdateParamType>(
            "timelineTitle",
            new Tack<InitialParamType, UpdateParamType>(
                "tackTitle",
                0,
                1,
                AutoConditions.Act.P0
            )
        )
    )
    { }
}

public class AutoForTest41_2<InitialParamType, UpdateParamType> : Auto<InitialParamType, UpdateParamType>
{
    public AutoForTest41_2(int frame, InitialParamType context) : base(
        "test41 sample auto",
        frame,
        context,
        new Timeline<InitialParamType, UpdateParamType>(
            "timelineTitle",
            new Tack<InitialParamType, UpdateParamType>(
                "tackTitle",
                0,
                2,
                AutoConditions.Act.P0,
                () =>
                {
                    var c = new RoutineContexts<InitialParamType, UpdateParamType>();
                    c.rContext = c.BreakAutoAt2Frame;
                    return c;
                }
            )
        )
    )
    { }
}

public class AutoForTest41_3<InitialParamType, UpdateParamType> : Auto<InitialParamType, UpdateParamType>
{
    public AutoForTest41_3(int frame, InitialParamType context) : base(
        "test41 sample auto",
        frame,
        context,
        new Timeline<InitialParamType, UpdateParamType>(
            "timelineTitle",
            new Tack<InitialParamType, UpdateParamType>(
                "tackTitle",
                0,
                100,
                AutoConditions.Anim.DEFAULT
            )
        )
    )
    { }
}
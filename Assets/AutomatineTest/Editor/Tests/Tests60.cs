using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Automatine;
using UnityEngine;


/**
	auto with continue.
*/
public partial class Test
{
    public void _60_0_AutoWithLength10By10Update()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        /*
			0で開始して、10で終わる予定のautoを、2 x 5で回す。
		*/
        var auto0 = new AutoForTest60_10<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

        for (var i = 0; i < 10; i++)
        {
            auto0.Update(frame, new Dictionary<string, PlayerContext>());
            frame++;
        }

        if (auto0.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed.");
    }

    public void _60_1_AutoWithLength10By5Update()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        /*
			
			・0を一回回す
			・2で、1,2の2回回す
			・3で、3,4の2回回す
			・4で、5,6の2回回す
			・5で、7,8の2回回す

			とやってしまうと、なるほど足りてない。0を1回、2を４回回したことになるだけ。
			比較して、

			10回回す場合は、
			・0を1回回す
			・1を1回回す
			・2を1回回す
			・3を1回回す
			・4を1回回す
			・5を1回回す
			・6を1回回す
			・7を1回回す
			・8を1回回す
			・9を1回回す

			なので、2倍で回したければ、こう渡すのが正しい。 
			・0で、0,1を2回回す <- 初期化時の最初の値は0でも、最初に回すパラメータが1。 = 倍速で回したい場合のパラメータから1引いた値を使う。引き継ぎ値の特例がある。
			・2で、2,3の2回回す
			・3で、4,5の2回回す
			・4で、6,7の2回回す
			・5で、8,9の2回回す
		*/
        var auto0 = new AutoForTest60_10<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

        frame = auto0.NextFrame(2);// 2ずつ進めたいので、0,1の2つのframe分の処理を回す意味で、0はすでに入っているので、1を入れる。というのが帰って来れば、あとあと使えそう。

        for (var i = 0; i < 5; i++)
        {
            auto0.Update(frame, new Dictionary<string, PlayerContext>());
            frame = auto0.NextFrame(2);
        }

        if (auto0.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed.");
    }

    public void _60_2_AutoWithLength10By4Update()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = (Auto<PlayerContext, Dictionary<string, PlayerContext>>)new AutoForTest60_10<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

        frame = auto0.NextFrame(3);

        for (var i = 0; i < 4; i++)
        {// 3, 3, 3, 3,　と進むので、最後2frameが次のAutoの実行に宛てられてほしい。
            auto0.Update(frame, new Dictionary<string, PlayerContext>());
            frame = auto0.NextFrame(3);
        }

        // 0,1,2, 3,4,5, 6,7,8, 9(ここで終了),10,11(この2frameが無駄になってしまう。)
        // 無駄になったカウントが取得できる
        if (auto0.OverrunCount() == 2)
        {
            return;
        }

        Debug.LogError("failed.");
    }

    public void _60_3_AutoWithLength10By4UpdateWithRun()
    {
        int frame = 0;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = (Auto<PlayerContext, Dictionary<string, PlayerContext>>)new AutoForTest60_10<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

        frame = auto0.NextFrame(3);

        for (var i = 0; i < 4; i++)
        {// 3, 3, 3, 3,　と進むので、最後2frameが次のAutoの実行に宛てられてほしい。
            auto0.Update(frame, new Dictionary<string, PlayerContext>());
            frame = auto0.NextFrame(3);
        }

        // 0,1,2, 3,4,5, 6,7,8, 9(ここで終了),10,11(この2frameが無駄になってしまう。)

        if (auto0.ShouldFalldown(frame))
        {// この時点でのframeは11
            var overrunCount = auto0.OverrunCount();// 変更前に取得しておくと、2 という「から回った数」が手にはいる。

            auto0 = new AutoForTest60_2<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

            frame = auto0.NextFrame(overrunCount);// から回った回数ぶん回すために、frameをセット
        }

        auto0.Update(frame, new Dictionary<string, PlayerContext>());// ちょうど2回実行されれば、いい感じに完了するはず。
        frame++;

        if (auto0.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed.");
    }

    public void _60_4_AutoWithLength10By4Update_2()
    {
        int frame = 10;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = (Auto<PlayerContext, Dictionary<string, PlayerContext>>)new AutoForTest60_10<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

        frame = auto0.NextFrame(3);

        for (var i = 0; i < 4; i++)
        {// 3, 3, 3, 3,　と進むので、最後2frameが次のAutoの実行に宛てられてほしい。
            auto0.Update(frame, new Dictionary<string, PlayerContext>());
            frame = auto0.NextFrame(3);
        }

        // 0,1,2, 3,4,5, 6,7,8, 9(ここで終了),10,11(この2frameが無駄になってしまう。)
        // 無駄になったカウントが取得できる
        if (auto0.OverrunCount() == 2)
        {
            return;
        }

        Debug.LogError("failed.");
    }

    public void _60_5_AutoWithLength10By4UpdateWithRun_2()
    {
        int frame = 10;
        var context0 = PlayerContext.Copy(dummyContext);

        var auto0 = (Auto<PlayerContext, Dictionary<string, PlayerContext>>)new AutoForTest60_10<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

        frame = auto0.NextFrame(3);

        for (var i = 0; i < 4; i++)
        {// 3, 3, 3, 3,　と進むので、最後2frameが次のAutoの実行に宛てられてほしい。
            auto0.Update(frame, new Dictionary<string, PlayerContext>());
            frame = auto0.NextFrame(3);
        }

        // 0,1,2, 3,4,5, 6,7,8, 9(ここで終了),10,11(この2frameが無駄になってしまう。)

        if (auto0.ShouldFalldown(frame))
        {// この時点でのframeは11
            var overrunCount = auto0.OverrunCount();// 変更前に取得しておくと、2 という「から回った数」が手にはいる。

            auto0 = new AutoForTest60_2<PlayerContext, Dictionary<string, PlayerContext>>(frame, context0);

            frame = auto0.NextFrame(overrunCount);// から回った回数ぶん回すために、frameをセット
        }

        auto0.Update(frame, new Dictionary<string, PlayerContext>());// ちょうど2回実行されれば、いい感じに完了するはず。
        frame++;

        if (auto0.ShouldFalldown(frame))
        {
            return;
        }

        Debug.LogError("failed.");
    }


    // 別の書き方がしたい。でもうーん、adhocとしてはこんなところか。
    // 実際には、単体での倍速とかはOKなんだけど、外部から影響を受ける粒度、内部の他の箇所に影響を出す粒度が減るので、問題が出る。
    // 


    // data

    public class AutoForTest60_10<InitialParamType, UpdateParamType> : Auto<InitialParamType, UpdateParamType>
    {
        public AutoForTest60_10(int frame, InitialParamType context) : base(
            "test60 sample auto",
            frame,
            context,
            new Timeline<InitialParamType, UpdateParamType>(
                "timelineTitle",
                new Tack<InitialParamType, UpdateParamType>(
                    "tackTitle",
                    0,
                    10,
                    AutoConditions.Act.P0
                )
            )
        )
        { }
    }
    public class AutoForTest60_2<InitialParamType, UpdateParamType> : Auto<InitialParamType, UpdateParamType>
    {
        public AutoForTest60_2(int frame, InitialParamType context) : base(
            "test60 sample auto",
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
}

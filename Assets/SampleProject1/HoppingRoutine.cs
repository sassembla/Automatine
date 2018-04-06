using System.Collections;
using Automatine;
using UnityEngine;

/**
	HoppingRoutine
	original is generated Routine by Automatine.
	You can reset or add code.
*/
public partial class RoutineContexts<InitialParamType, UpdateParamType> : RoutineBase<InitialParamType, UpdateParamType>
{

    public IEnumerator HoppingRoutine(InitialParamType initialParam)
    {
        /*
            GUI上で作成したCoroutineに対して、自由にコードを追加できる。
            CoroutineはそれをセットされたTackが再生される限り実行され続ける。


            ここで渡ってくるInitialParamType initialParamには、Auto初期化時に第一型引数に渡した型の値がくる。
            ここでは、HoppingSample.csで定義したContext型が渡ってくるため、Context型でキャストしてContext型として扱うことができる。
         */
        var context = initialParam as HoppingSample.Context;

        // Context型からプレイヤーを取り出す。
        var player = context.player;

        // プレイヤーのy位置が2を超えるまで、上方向への力を加える
        while (true)
        {
            player.GetComponent<Rigidbody>().AddForce(Vector3.up * 15f);
            if (2 < player.transform.position.y)
            {
                break;
            }
            yield return null;
        }


        // それ以降は右方向の力を加える
        while (true)
        {
            var updateParam = this.updateParam as HoppingSample.Context;
            player.GetComponent<Rigidbody>().AddForce(Vector3.right * 15f);
            yield return null;
        }
    }
}
using System;
using System.Collections;
using Automatine;
using UnityEngine;
/*
Run
generated Routine by Automatine.
*/
public partial class RoutineContexts<InitialParamType, UpdateParamType> : RoutineBase<InitialParamType, UpdateParamType>
{

    public IEnumerator Run(InitialParamType initialParam)
    {
        var context = initialParam as HoppingSample.Context;
        var player = context.player;

        // 左に動く。
        while (true)
        {
            player.GetComponent<Rigidbody>().AddForce(Vector3.left * 10, ForceMode.Impulse);
            yield return null;
        }

    }

}
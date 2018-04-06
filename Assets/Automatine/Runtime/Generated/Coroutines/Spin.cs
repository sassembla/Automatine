using System;
using System.Collections;
using Automatine;
using UnityEngine;
/*
Spin
generated Routine by Automatine.
*/
public partial class RoutineContexts<InitialParamType, UpdateParamType> : RoutineBase<InitialParamType, UpdateParamType>
{

    public IEnumerator Spin(InitialParamType initialParam)
    {
        var context = initialParam as HoppingSample.Context;
        var player = context.player;

        /*
			適当な回転をする。
		*/
        while (true)
        {
            player.transform.Rotate(new Vector3(12, 3, 3));
            yield return null;
        }

    }

}
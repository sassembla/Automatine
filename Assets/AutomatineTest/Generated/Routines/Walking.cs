using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using Automatine;

/**
	実際に実行されるルーチン置き場、最終的には、
	GUI上からセット可能 = 定義を型情報から抜き出してセットできるようにしたい。
*/
public partial class RoutineContexts<InitialParamType, UpdateParamType> : RoutineBase<InitialParamType, UpdateParamType> {

	public IEnumerator Walking (InitialParamType initialParam) {
		var initialContext = initialParam as PlayerContext;
		var playerId = initialContext.playerId;
		// 移動の向きから、進む方向を決める
		var forward = 0;
		var horizon = 0;
		var vertica = 0;

		var stepSize = 2;
		
		if (initialContext.front) forward =	stepSize;
		if (initialContext.back) forward =	-stepSize;
		if (initialContext.right) horizon =	stepSize;
		if (initialContext.left) horizon =	-stepSize;

		while (true) {
			var contexts = updateParam as Dictionary<string, PlayerContext>;
			var moverContext = contexts[playerId];

			// 移動後の位置をセット
			// PlayerUpdater_2.Move(moverContext, forward, horizon, vertica);
			yield return null;
		}
	}
}
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
	/**
		Spawn用のAuto
	*/
	public IEnumerator Spawn (InitialParamType initialParam) {
		var initialContext = initialParam as PlayerContext;
		var playerId = initialContext.playerId;

		var spawnFrameWait = 100;// spawn 100 frame later!
		
		{
			initialContext.life = 100;
			initialContext.x = 0;
			initialContext.y = 0;
			initialContext.z = 0;
		}
		
		while (true) {
			var contexts = updateParam as Dictionary<string, PlayerContext>;
			contexts[playerId].life = contexts[playerId].life+1;

			if (spawnFrameWait == 0) {
				BreakAuto();
				yield break;
			}
			
			spawnFrameWait--;
			yield return null;
		}
	}
}
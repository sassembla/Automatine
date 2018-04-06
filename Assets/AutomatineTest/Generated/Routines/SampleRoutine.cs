using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using Automatine;

/**
	Sampleルーチン
*/
public partial class RoutineContexts<InitialParamType, UpdateParamType> : RoutineBase<InitialParamType, UpdateParamType> {
	/**
		PlayerContextの中身をいじるRoutine
	*/
	public IEnumerator SampleRoutine (InitialParamType initialParam) {
		var initialContext = initialParam as PlayerContext;
		var playerId = initialContext.playerId;

		while (true) {
			var contexts = updateParam as Dictionary<string, PlayerContext>;
			contexts[playerId].life = contexts[playerId].life+1;
			yield return null;
		}
	}
}
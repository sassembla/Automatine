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
		無限に保つ
	*/
	public IEnumerator _Infinity (InitialParamType initialParam) {
		int i = 0;

		while (true) {
			// Debug.Log("_Infinity i:" + i);
			i++;
			yield return null;
		}
	}

	public IEnumerator _BreakTack (InitialParamType initialParam) {
		BreakTack();
		yield return null;
	}

	public IEnumerator _BreakTimeline (InitialParamType initialParam) {
		BreakTimeline();
		yield return null;
	}

	public IEnumerator _BreakAuto (InitialParamType initialParam) {
		BreakAuto();
		yield return null;
	}

	public IEnumerator _BreakAutoAtLastFrame (InitialParamType initialParam) {
		while (true) {
			if (IsFinalCount()) {
				BreakAuto();
				yield break;
			}
			yield return null;
		}
	}

	public IEnumerator _BreakAutoBeforeLastFrame (InitialParamType initialParam) {
		while (true) {
			if (RestCount() == 1) {
				BreakAuto();
				yield break;
			}
			yield return null;
		}
	}

	public IEnumerator _BreakAutoIfConditionContainsSpecific (InitialParamType initialParam) {
		var conditionType = ConditionType();
		var conditionValue = ConditionValue();

		if (conditionType == ConditionGateway.GetIndexOfConditionType(AutoConditions.Act.P0) && 
			conditionValue == (int)AutoConditions.Act.P0) {
			BreakAuto();
		}

		yield return null;
	}

	public IEnumerator _BreakAutoIfConditionsContainsSpecific (InitialParamType initialParam) {
		var conditions = Conditions();

		if (conditions[ConditionGateway.GetIndexOfConditionType(AutoConditions.Act.P0)] == (int)AutoConditions.Act.P0) {
			BreakAuto();
		}

		yield return null;
	}

	public IEnumerator _BreakTimelineThenGetConditionsAndBreakAutoIfConditionIsChanged (InitialParamType initialParam) {
		var playerContext = initialParam as PlayerContext;
		var playerId = playerContext.playerId;

		var currentPlayerContext = updateParam as Dictionary<string, PlayerContext>;
		
		var auto = currentPlayerContext[playerId].auto;
		var beforeCondition = auto.Conditions();

		BreakTimeline();

		var afterCondition = currentPlayerContext[playerId].auto.Conditions();

		if (!beforeCondition.SameConditions(afterCondition)) BreakAuto();
		yield return null;
	}

	public IEnumerator _BreakTackThenGetConditionsAndBreakAutoIfConditionIsNotChanged (InitialParamType initialParam) {
		var playerContext = initialParam as PlayerContext;
		var playerId = playerContext.playerId;

		var currentPlayerContext = updateParam as Dictionary<string, PlayerContext>;
		
		var auto = currentPlayerContext[playerId].auto;
		var beforeCondition = auto.Conditions();

		BreakTack();

		var afterCondition = currentPlayerContext[playerId].auto.Conditions();

		if (beforeCondition.SameConditions(afterCondition)) BreakAuto();
		yield return null;
	}

	public IEnumerator _CancelThisRoutineByBreakTack (InitialParamType initialParam) {
		BreakTack();
		yield return null;
		Debug.LogError("Should never run this routine again. info:" + ParentsInfo());
	}

	public IEnumerator _CancelThisRoutineByBreakTimeline (InitialParamType initialParam) {
		BreakTimeline();
		yield return null;
		Debug.LogError("Should never run this routine again. info:" + ParentsInfo());
	}

	public IEnumerator _CancelThisRoutineByBreakAuto (InitialParamType initialParam) {
		BreakAuto();
		yield return null;
		Debug.LogError("Should never run this routine again. info:" + ParentsInfo());
	}

	public IEnumerator _CancelRoutineByBreakTack (InitialParamType initialParam) {
		BreakTack();
		yield return null;
	}

	public IEnumerator _CancelRoutineByBreakAutoAndBreakTack (InitialParamType initialParam) {
		BreakAuto();
		BreakTack();
		yield return null;
	}

	public IEnumerator _CancelRoutineByBreakTimeline (InitialParamType initialParam) {
		BreakTimeline();
		yield return null;
	}

	public IEnumerator _CancelRoutineByBreakAuto (InitialParamType initialParam) {
		BreakAuto();
		yield return null;
	}

	public IEnumerator _NeverRun (InitialParamType initialParam) {
		Debug.LogError("Should never run this routine. info:" + ParentsInfo());
		yield return null;
	}

	public IEnumerator _FiveFrameThenBreakAuto (InitialParamType initialParam) {
		int i = 0;
		while (true) {
			i++;
			if (i == 5) {
				BreakAuto();
				yield break;
			}
			yield return null;
		}
	}

	public IEnumerator _FiveFrameThenBreakTimeline (InitialParamType initialParam) {
		int i = 0;
		while (true) {
			i++;
			if (i == 5) {
				BreakTimeline();
				yield break;
			}
			yield return null;
		}
	}

	public IEnumerator _SixFrameThenBreakTimeline (InitialParamType initialParam) {
		int i = 0;
		while (true) {
			i++;
			if (i == 6) {
				BreakTimeline();
				yield break;
			}
			yield return null;
		}
	}

	public IEnumerator _AutoIdSeeker (InitialParamType initialParam) {
		var autoId = ParentAutoId();
		int i = 0;

		while (true) {
			i++;
			if (i == 2) {
				if (autoId == ParentAutoId()) {
					BreakAuto();
					yield break;
				}
			}
			yield return null;
		}
	}


}
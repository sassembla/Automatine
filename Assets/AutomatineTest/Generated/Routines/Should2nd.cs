using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using Automatine;

/**
	Should2nd
*/
public partial class RoutineContexts<InitialParamType, UpdateParamType> : RoutineBase<InitialParamType, UpdateParamType> {
	
	public IEnumerator Should2nd (InitialParamType initialParam) {
		var initialContext = initialParam as PlayerContext;
		initialContext.runMark = initialContext.runMark + "2nd";
		yield return null;
	}
}
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using Automatine;

/**
	Should1st
*/
public partial class RoutineContexts<InitialParamType, UpdateParamType> : RoutineBase<InitialParamType, UpdateParamType> {
	
	public IEnumerator Should1st (InitialParamType initialParam) {
		var initialContext = initialParam as PlayerContext;
		initialContext.runMark = initialContext.runMark + "1st"; 
		yield return null;
	}
}
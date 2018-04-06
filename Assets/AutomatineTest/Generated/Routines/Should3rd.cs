using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using Automatine;

/**
	Should3rd
*/
public partial class RoutineContexts<InitialParamType, UpdateParamType> : RoutineBase<InitialParamType, UpdateParamType> {
	
	public IEnumerator Should3rd (InitialParamType initialParam) {
		var initialContext = initialParam as PlayerContext;
		initialContext.runMark = initialContext.runMark + "3rd";
		yield return null;
	}
}
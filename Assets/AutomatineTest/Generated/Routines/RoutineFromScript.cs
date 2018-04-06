using System.Collections;
using Automatine;
using UnityEngine;

public partial class RoutineContexts<InitialParamType, UpdateParamType> : RoutineBase<InitialParamType, UpdateParamType> {
    
	public IEnumerator RoutineFromString (InitialParamType initialParam) {
		int i = 0;

		while (true) {
			// Debug.Log("RoutineFromString i:" + i);
			i++;
			yield return null;
		}
	}
}
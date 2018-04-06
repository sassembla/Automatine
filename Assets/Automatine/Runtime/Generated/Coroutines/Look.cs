using System;
using System.Collections;
using Automatine;
using UnityEngine;
/*
Look
generated Routine by Automatine.
*/
public partial class RoutineContexts <InitialParamType, UpdateParamType> : RoutineBase<InitialParamType, UpdateParamType> {
	
	public IEnumerator Look (InitialParamType initialParam) {
		var cam = initialParam as Cont;
		var target = GameObject.Find("Cube") as GameObject;
		
		while (true) {
			cam.transform.LookAt(target.transform);
			yield return null;
		}
		
	}
	
}
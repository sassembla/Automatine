using UnityEngine;

using System;
using System.Collections;
using Automatine;

/**
	実際に実行されるルーチン置き場、最終的には、
	GUI上からセット可能 = 定義を型情報から抜き出してセットできるようにしたい。
*/
public partial class RoutineContexts<InitialParamType, UpdateParamType> : RoutineBase<InitialParamType, UpdateParamType> {
	
	public IEnumerator SpawnMotion (InitialParamType initialParam) {
		yield break;
	}
}
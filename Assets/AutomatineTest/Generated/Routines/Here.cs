using UnityEngine;

using System;
using System.Collections;

using Automatine;

/**
	実際に実行されるルーチン置き場、最終的には、
	GUI上からセット可能 = 定義を型情報から抜き出してセットできるようにしたい。
*/
public partial class RoutineContexts<InitialParamType, UpdateParamType> : RoutineBase<InitialParamType, UpdateParamType> {
	/**
		到達確認用。
	*/
	public IEnumerator Here (InitialParamType initialParam) {
		while (true) {
			Debug.Log("Here:" + " routineName:" + routineName + " frame:" + frame + " loadCount:" + loadCount);
			yield return null;
		}
	}
}
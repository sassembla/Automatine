using System;
using Automatine;
using UnityEngine;

/**
	このChangerは特定の型のAutoしかうけつけない、みたいなの、行ける。特定型のメソッドを追記すれば、そこから遷移するのが正当、っていうのができる。
	constraints 句を作れた！！
	jsonにセットしといて、autoからこのchangerを指定、とかすればいいな。
	指定はチェックいれたり外したりみたいな感じか。
*/
public static class Changer_PlayerWillMoveOrNot_Class {
	private static Auto<InitialParamType, UpdateParamType> 
		PlayerWillMoveOrNot
		<InitialParamType, UpdateParamType>
		(Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext)
	{
		var conditions = baseAuto.Conditions();
		
		/*
			移動開始
		*/
		if (
			ConditionGateway.NotContains(conditions, AutoConditions.Act.SPAWN) &&
			ConditionGateway.Contains(conditions, AutoConditions.Act.DEFAULT) &&
			ConditionGateway.ContainsAll(conditions, AutoConditions.Act.DEFAULT) &&
			ConditionGateway.NotContainsAll(conditions, AutoConditions.Act.SPAWN)
		) {
			Debug.Log("歩行条件を満たした!");
			
			var newAuto = new 
				MoveAuto
				<InitialParamType, UpdateParamType>(frame, fixedContext);

			// inheritがある場合、ここにinherit処理が付く。
			newAuto.InheritTimelines(baseAuto.ExportTimelines(new Type[] {
				typeof(AutoConditions.Move)
			}));

			return newAuto;
		}
		
		return baseAuto;
	}
	public static Auto<InitialParamType, UpdateParamType> Changer_PlayerWillMoveOrNot<InitialParamType, UpdateParamType> (this Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext) {
		return PlayerWillMoveOrNot(baseAuto, frame, fixedContext);
	}

}
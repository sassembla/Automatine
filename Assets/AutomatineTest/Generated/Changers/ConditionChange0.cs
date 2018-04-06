using System;
using Automatine;
using UnityEngine;
/*
	ConditionChange
	test classes for Changer. made by hand. 
*/
public static class Changer_ConditionChange0_Class {
	public static Auto<InitialParamType, UpdateParamType> 
		ConditionChange0
		<InitialParamType, UpdateParamType>
		(Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext)
	{
		var conditions = baseAuto.Conditions();
		
		/*
			comment for branchName0
		*/
		if (
			ConditionGateway.Contains(conditions, AutoConditions.Act.P0)
		) {
			var newAuto = new 
				ConditionChangeAuto0
				<InitialParamType, UpdateParamType>(frame, fixedContext);
			return newAuto;
		}
		
		return baseAuto;
	}
	public static Auto<InitialParamType, UpdateParamType> Changer_ConditionChange0<InitialParamType, UpdateParamType> (this Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext) {
		return ConditionChange0(baseAuto, frame, fixedContext);
	}

	/**
		1
	*/
	public static Auto<InitialParamType, UpdateParamType> 
		ConditionChange1
		<InitialParamType, UpdateParamType>
		(Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext)
	{
		var conditions = baseAuto.Conditions();

		/*
			comment for branchName0
		*/
		if (
			ConditionGateway.Contains(conditions, AutoConditions.Canc.DASH)
		) {
			var newAuto = new 
				ConditionChangeAuto0
				<InitialParamType, UpdateParamType>(frame, fixedContext);
			return newAuto;
		}

		var finallyAuto = new 
			ConditionChangeAuto1
			<InitialParamType, UpdateParamType>(frame, fixedContext);
			finallyAuto.InheritTimelines(baseAuto.ExportTimelines(new Type[] {
				typeof(AutoConditions.Move)
			}));
		return finallyAuto;
	}
	public static Auto<InitialParamType, UpdateParamType> Changer_ConditionChange1<InitialParamType, UpdateParamType> (this Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext) {
		return ConditionChange1(baseAuto, frame, fixedContext);
	}


	/*
		2
	*/
	public static Auto<InitialParamType, UpdateParamType> 
		ConditionChange2
		<InitialParamType, UpdateParamType>
		(Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext)
	{
		var conditions = baseAuto.Conditions();

		var finallyAuto = new 
			ConditionChangeAuto2
			<InitialParamType, UpdateParamType>(frame, fixedContext);
			finallyAuto.InheritTimelines(baseAuto.ExportTimelines(new Type[] {
				typeof(AutoConditions.Act)
			}));
		return finallyAuto;
	}
	public static Auto<InitialParamType, UpdateParamType> Changer_ConditionChange2<InitialParamType, UpdateParamType> (this Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext) {
		return ConditionChange2(baseAuto, frame, fixedContext);
	}
}
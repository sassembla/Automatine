using System;
using Automatine;
/*
	TestRoute1
	generated Changer by Automatine.
	this changer is for change move or not.
*/
public  class Changer_TestRoute1 : IAutoChanger {
	public static Auto<InitialParamType, UpdateParamType> 
		TestRoute1
		<InitialParamType, UpdateParamType>
		(Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext)
	{
		var conditions = baseAuto.Conditions();
		
		/*
			comment for branchName0
		*/
		if (
			ConditionGateway.NotContains(conditions, AutoConditions.Act.SPAWN) && 
			ConditionGateway.Contains(conditions, AutoConditions.Act.DEFAULT) && 
			ConditionGateway.ContainsAll(conditions, AutoConditions.Act.DEFAULT) && 
			ConditionGateway.NotContainsAll(conditions, AutoConditions.Act.SPAWN)
		) {
			var newAuto = new 
				MoveAuto
				<InitialParamType, UpdateParamType>(frame, fixedContext);
			newAuto.InheritTimelines(baseAuto.ExportTimelines(new Type[] {
				typeof(AutoConditions.Move)
			}));
			return newAuto;
		}
		
		/*
			comment for branchName1
		*/
		if (
			ConditionGateway.Contains(conditions, AutoConditions.Act.SPAWN)
		) {
			return baseAuto;
		}
		
		return baseAuto;
	}

	public static bool IsEffectiveChanger<InitialParamType, UpdateParamType> (Auto<InitialParamType, UpdateParamType> baseAuto) {
		var conditions = baseAuto.Conditions();
		
		/*
			comment for branchName0
		*/
		if (
			ConditionGateway.NotContains(conditions, AutoConditions.Act.SPAWN) && 
			ConditionGateway.Contains(conditions, AutoConditions.Act.DEFAULT) && 
			ConditionGateway.ContainsAll(conditions, AutoConditions.Act.DEFAULT) && 
			ConditionGateway.NotContainsAll(conditions, AutoConditions.Act.SPAWN)
		) {
		   return true;
		}
		
		/*
			comment for branchName1
		*/
		if (
			ConditionGateway.Contains(conditions, AutoConditions.Act.SPAWN)
		) {
		   return true;
		}
		
		return false;
	}

	public Func<Auto<InitialParamType, UpdateParamType>, int, InitialParamType, Auto<InitialParamType, UpdateParamType>> Changer<InitialParamType, UpdateParamType> () {
		return delegate (Auto<InitialParamType, UpdateParamType> auto, int frame, InitialParamType fixedContext) {
			return TestRoute1(auto, frame, fixedContext);
		};
	}

	public string ChangerName () {
		return "TestRoute1";
	}

	public bool IsEffective<InitialParamType, UpdateParamType> (Auto<InitialParamType, UpdateParamType> baseAuto) {
		var conditions = baseAuto.Conditions();
		
		/*
			comment for branchName0
		*/
		if (
			ConditionGateway.NotContains(conditions, AutoConditions.Act.SPAWN) && 
			ConditionGateway.Contains(conditions, AutoConditions.Act.DEFAULT) && 
			ConditionGateway.ContainsAll(conditions, AutoConditions.Act.DEFAULT) && 
			ConditionGateway.NotContainsAll(conditions, AutoConditions.Act.SPAWN)
		) {
		   return true;
		}
		
		/*
			comment for branchName1
		*/
		if (
			ConditionGateway.Contains(conditions, AutoConditions.Act.SPAWN)
		) {
		   return true;
		}
		
		return false;
	}
}
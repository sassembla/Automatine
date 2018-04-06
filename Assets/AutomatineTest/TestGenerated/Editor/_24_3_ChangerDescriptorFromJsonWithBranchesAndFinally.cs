using System;
using Automatine;
/*
	TestRoute3
	generated Changer by Automatine.
	this changer is for change move or not.
*/
public  class Changer_TestRoute3 : IAutoChanger {
	public static Auto<InitialParamType, UpdateParamType> 
		TestRoute3
		<InitialParamType, UpdateParamType>
		(Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext)
	{
		var conditions = baseAuto.Conditions();
		
		/*
			comment for branchName0
		*/
		if (
			ConditionGateway.Contains(conditions, AutoConditions.Act.SPAWN)
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
		
		var finallyAuto = new 
			MoveAuto
			<InitialParamType, UpdateParamType>(frame, fixedContext);
			finallyAuto.InheritTimelines(baseAuto.ExportTimelines(new Type[] {
				typeof(AutoConditions.Move)
			}));
		return finallyAuto;
	}

	public static bool IsEffectiveChanger<InitialParamType, UpdateParamType> (Auto<InitialParamType, UpdateParamType> baseAuto) {
		var conditions = baseAuto.Conditions();
		
		/*
			comment for branchName0
		*/
		if (
			ConditionGateway.Contains(conditions, AutoConditions.Act.SPAWN)
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
		
	   // finally is ON.
		return true;
	}

	public Func<Auto<InitialParamType, UpdateParamType>, int, InitialParamType, Auto<InitialParamType, UpdateParamType>> Changer<InitialParamType, UpdateParamType> () {
		return delegate (Auto<InitialParamType, UpdateParamType> auto, int frame, InitialParamType fixedContext) {
			return TestRoute3(auto, frame, fixedContext);
		};
	}

	public string ChangerName () {
		return "TestRoute3";
	}

	public bool IsEffective<InitialParamType, UpdateParamType> (Auto<InitialParamType, UpdateParamType> baseAuto) {
		var conditions = baseAuto.Conditions();
		
		/*
			comment for branchName0
		*/
		if (
			ConditionGateway.Contains(conditions, AutoConditions.Act.SPAWN)
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
		
	   // finally is ON.
		return true;
	}
}
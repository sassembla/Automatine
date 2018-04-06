using System;
using Automatine;
/*
	TestRoute2
	generated Changer by Automatine.
	this changer is for change move or not.
*/
public  class Changer_TestRoute2 : IAutoChanger {
	public static Auto<InitialParamType, UpdateParamType> 
		TestRoute2
		<InitialParamType, UpdateParamType>
		(Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext)
	{
		var finallyAuto = new 
			MoveAuto
			<InitialParamType, UpdateParamType>(frame, fixedContext);
			finallyAuto.InheritTimelines(baseAuto.ExportTimelines(new Type[] {
				typeof(AutoConditions.Move)
			}));
		return finallyAuto;
	}

	public static bool IsEffectiveChanger<InitialParamType, UpdateParamType> (Auto<InitialParamType, UpdateParamType> baseAuto) {
	   // finally is ON.
		return true;
	}

	public Func<Auto<InitialParamType, UpdateParamType>, int, InitialParamType, Auto<InitialParamType, UpdateParamType>> Changer<InitialParamType, UpdateParamType> () {
		return delegate (Auto<InitialParamType, UpdateParamType> auto, int frame, InitialParamType fixedContext) {
			return TestRoute2(auto, frame, fixedContext);
		};
	}

	public string ChangerName () {
		return "TestRoute2";
	}

	public bool IsEffective<InitialParamType, UpdateParamType> (Auto<InitialParamType, UpdateParamType> baseAuto) {
	   // finally is ON.
		return true;
	}
}
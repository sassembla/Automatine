// デフォルトのautoを返す機構、Falldownの先に自分で書けばよさそうだが、falldownがあることで描きやすくなった。

using Automatine;

public static class Changer_PlayerDefault_Class {
	private static Auto<InitialParamType, UpdateParamType> 
		PlayerDefault
		<InitialParamType, UpdateParamType>
		(Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext)
	{
		var conditions = baseAuto.Conditions();
		
		return new DefaultAuto<InitialParamType, UpdateParamType>(frame, fixedContext);
	}
	public static Auto<InitialParamType, UpdateParamType> Changer_PlayerDefault<InitialParamType, UpdateParamType> (this Auto<InitialParamType, UpdateParamType> baseAuto, int frame, InitialParamType fixedContext) {
		return PlayerDefault(baseAuto, frame, fixedContext);
	}

}
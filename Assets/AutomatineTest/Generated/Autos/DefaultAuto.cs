using System;
using System.Collections;
using Automatine;

/**
	generated Auto by Automatine.
*/
public class DefaultAuto <InitialParamType, UpdateParamType> : Auto <InitialParamType, UpdateParamType> {
	public DefaultAuto (int frame, InitialParamType context) : base (
		"デフォルト状態", 
		frame,
		context,
		new Timeline<InitialParamType, UpdateParamType>(
			"基本モーション",
			new Tack<InitialParamType, UpdateParamType>(
				"基礎行動",
				0,
				AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
				AutoConditions.Act.DEFAULT,
				() => {
					var c = new RoutineContexts<InitialParamType, UpdateParamType>();
					c.rContext = c.Default;
					return c;
				}
			)
		)
	){}
}
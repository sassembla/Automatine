using System;
using System.Collections;
using Automatine;
/*
	Auto_Default2
	generated Auto by Automatine.
	複数行のコメント
	描きたいですね。
*/
public class Auto_Default2 <InitialParamType, UpdateParamType> : Auto <InitialParamType, UpdateParamType> {
	public Auto_Default2 (int frame, InitialParamType initialParam) : base (
		"autoInfo",
		frame,
		initialParam
		,
		new Timeline<InitialParamType, UpdateParamType>(
			"timelineInfo_0"
			,
			new Tack<InitialParamType, UpdateParamType>(
				"tackInfo_0",
				0,
				1
			),
			new Tack<InitialParamType, UpdateParamType>(
				"tackInfo_1",
				1,
				10
			   ,
					() => {
						var c = new RoutineContexts<InitialParamType, UpdateParamType>();
						c.rContext = 
							c.Default
						;
						return c;
					}
			)
		)
	){}
}
using System;
using System.Collections;
using Automatine;
/*
	Auto_Default1
	generated Auto by Automatine.
	複数行のコメント
	描きたいですね。
*/
public class Auto_Default1 <InitialParamType, UpdateParamType> : Auto <InitialParamType, UpdateParamType> {
	public Auto_Default1 (int frame, InitialParamType initialParam) : base (
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
			   ,
				AutoConditions.Act.DEFAULT
			   ,
					() => {
						var c = new RoutineContexts<InitialParamType, UpdateParamType>();
						c.rContext = 
							c.Default
						;
						return c;
					},
					() => {
						var c = new RoutineContexts<InitialParamType, UpdateParamType>();
						c.rContext = 
							c._Infinity
						;
						return c;
					}
			),
			new Tack<InitialParamType, UpdateParamType>(
				"tackInfo_1",
				1,
				10
			   ,
				AutoConditions.Act.P0
			   ,
					() => {
						var c = new RoutineContexts<InitialParamType, UpdateParamType>();
						c.rContext = 
							c.Default
						;
						return c;
					}
			)
		),
		new Timeline<InitialParamType, UpdateParamType>(
			"timelineInfo_1"
			,
			new Tack<InitialParamType, UpdateParamType>(
				"tackInfo_2",
				0,
				20
			   ,
				AutoConditions.Anim.DEFAULT
			   ,
					() => {
						var c = new RoutineContexts<InitialParamType, UpdateParamType>();
						c.rContext = 
							c.Default
						;
						return c;
					}
			),
			new Tack<InitialParamType, UpdateParamType>(
				"tackInfo_3",
				20,
				100
			   ,
				AutoConditions.Anim.SPAWN
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
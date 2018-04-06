using System;
using System.Collections;
using Automatine;
/*
	generated AutoGenerator by Automatine.
*/
public class Auto_Default0 <InitialParamType, UpdateParamType> : Auto <InitialParamType, UpdateParamType> {
	public Auto_Default0 (int frame, InitialParamType initialParam) : base (
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
					}
			),
			new Tack<InitialParamType, UpdateParamType>(
				"tackInfo_1",
				0,
				1
			   ,
				AutoConditions.Anim.DEFAULT
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
			)
		),
		new Timeline<InitialParamType, UpdateParamType>(
			"timelineInfo_1"
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
					}
			),
			new Tack<InitialParamType, UpdateParamType>(
				"tackInfo_1",
				0,
				1
			   ,
				AutoConditions.Anim.DEFAULT
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
			)
		)
	){}
}
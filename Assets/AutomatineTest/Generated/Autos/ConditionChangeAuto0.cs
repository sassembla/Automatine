using System;
using System.Collections;
using Automatine;

/**
	generated Auto by Automatine.
	(this file is hand made.)
*/
public class ConditionChangeAuto0 <InitialParamType, UpdateParamType> : Auto <InitialParamType, UpdateParamType> {
	public ConditionChangeAuto0 (int frame, InitialParamType context) : base (
		"ConditionChangeAuto0", 
		frame,
		context,
		new Timeline<InitialParamType, UpdateParamType>(
			"ConditionChangeAuto0TL0",
			new Tack<InitialParamType, UpdateParamType>(
				"ConditionChangeAuto0Tack0",
				0,
				1,
				AutoConditions.Act.P1,
				() => {
					var c = new RoutineContexts<InitialParamType, UpdateParamType>();
					c.rContext = c._Infinity;
					return c;
				}
			),
			new Tack<InitialParamType, UpdateParamType>(
				"ConditionChangeAuto0Tack1",
				1,
				1,
				AutoConditions.Act.P2,
				() => {
					var c = new RoutineContexts<InitialParamType, UpdateParamType>();
					c.rContext = c._Infinity;
					return c;
				}
			)
		),
		new Timeline<InitialParamType, UpdateParamType>(
			"ConditionChangeAuto0TL1",
			new Tack<InitialParamType, UpdateParamType>(
				"ConditionChangeAuto0Tack2",
				0,
				1,
				AutoConditions.Canc.WALK,
				() => {
					var c = new RoutineContexts<InitialParamType, UpdateParamType>();
					c.rContext = c._Infinity;
					return c;
				}
			),
			new Tack<InitialParamType, UpdateParamType>(
				"ConditionChangeAuto0Tack3",
				1,
				1,
				AutoConditions.Canc.DASH,
				() => {
					var c = new RoutineContexts<InitialParamType, UpdateParamType>();
					c.rContext = c._Infinity;
					return c;
				}
			)
		)
	){}
}

public class ConditionChangeAuto1 <InitialParamType, UpdateParamType> : Auto <InitialParamType, UpdateParamType> {
	public ConditionChangeAuto1 (int frame, InitialParamType context) : base (
		"ConditionChangeAuto1", 
		frame,
		context,
		new Timeline<InitialParamType, UpdateParamType>(
			"ConditionChangeAuto1TL0",
			new Tack<InitialParamType, UpdateParamType>(
				"ConditionChangeAuto1Tack0",
				0,
				1,
				AutoConditions.Act.P3,
				() => {
					var c = new RoutineContexts<InitialParamType, UpdateParamType>();
					c.rContext = c._Infinity;
					return c;
				}
			),
			new Tack<InitialParamType, UpdateParamType>(
				"ConditionChangeAuto1Tack1",
				1,
				1,
				AutoConditions.Act.P4,
				() => {
					var c = new RoutineContexts<InitialParamType, UpdateParamType>();
					c.rContext = c._Infinity;
					return c;
				}
			)
		),
		new Timeline<InitialParamType, UpdateParamType>(
			"ConditionChangeAuto1TL1",
			new Tack<InitialParamType, UpdateParamType>(
				"ConditionChangeAuto1Tack2",
				0,
				1,
				AutoConditions.Canc.DASH,
				() => {
					var c = new RoutineContexts<InitialParamType, UpdateParamType>();
					c.rContext = c._Infinity;
					return c;
				}
			),
			new Tack<InitialParamType, UpdateParamType>(
				"ConditionChangeAuto1Tack2",
				2,
				1,
				AutoConditions.Canc.C0,
				() => {
					var c = new RoutineContexts<InitialParamType, UpdateParamType>();
					c.rContext = c._Infinity;
					return c;
				}
			),
			new Tack<InitialParamType, UpdateParamType>(
				"ConditionChangeAuto1Tack3",
				3,
				1,
				AutoConditions.Canc.C1,
				() => {
					var c = new RoutineContexts<InitialParamType, UpdateParamType>();
					c.rContext = c._Infinity;
					return c;
				}
			)
		)
	){}
}

public class ConditionChangeAuto2 <InitialParamType, UpdateParamType> : Auto <InitialParamType, UpdateParamType> {
	/**
		2
	*/
	public ConditionChangeAuto2 (int frame, InitialParamType context) : base (
		"ConditionChangeAuto2", 
		frame,
		context,
		new Timeline<InitialParamType, UpdateParamType>(
			"ConditionChangeAuto2TL0",
			new Tack<InitialParamType, UpdateParamType>(
				"ConditionChangeAuto2Tack0",
				0,
				-1,
				AutoConditions.Act.P0,
				() => {
					var c = new RoutineContexts<InitialParamType, UpdateParamType>();
					c.rContext = c._Infinity;
					return c;
				}
			)
		)
	){}
}
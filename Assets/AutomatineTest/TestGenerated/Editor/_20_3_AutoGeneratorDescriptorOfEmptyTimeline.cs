using System;
using System.Collections;
using Automatine;
/*
	Auto_Default3
	generated Auto by Automatine.
	複数行のコメント
	描きたいですね。
*/
public class Auto_Default3 <InitialParamType, UpdateParamType> : Auto <InitialParamType, UpdateParamType> {
	public Auto_Default3 (int frame, InitialParamType initialParam) : base (
		"autoInfo",
		frame,
		initialParam
		,
		new Timeline<InitialParamType, UpdateParamType>(
			"timelineInfo_0"
		)
	){}
}
using UnityEngine;

using System;
using System.Collections;
using Automatine;

/**
	Automatine用でいうと、フラグの確認をするメソッドを持っている。
	条件として吐き出すクラスがあればいいのか、条件突入に関してのメソッドを持てばいいのか、悩ましい。
*/
public class SpawnAuto <InitialParamType, UpdateParamType> : Auto <InitialParamType, UpdateParamType> {
	/**
		set new "spawn" auto to this player.
	*/
	public SpawnAuto (int frame, InitialParamType context) : base (
		"spawn開始", 
		frame,
		context,
		new Timeline<InitialParamType, UpdateParamType>(
			"Spawn処理",
			new Tack<InitialParamType, UpdateParamType>(
				"spawning",
				0,
				AutomatineDefinitions.Tack.LIMIT_UNLIMITED,
				AutoConditions.Act.SPAWN,
				() => {
					var c = new RoutineContexts<InitialParamType, UpdateParamType>();
					c.rContext = c.Spawn;
					return c;
				}
			)
		),
		new Timeline<InitialParamType, UpdateParamType>(
			"Spawn時のモーション",
			new Tack<InitialParamType, UpdateParamType>(
				"まだ真面目にセットしてない、Spawn時のモーション",
				0,
				10,
				AutoConditions.Anim.SPAWN,
				() => {
					var c = new RoutineContexts<InitialParamType, UpdateParamType>();
					c.rContext = c.SpawnMotion;
					return c;
				}
			)
		)
	){}
}
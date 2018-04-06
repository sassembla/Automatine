using Automatine;

/**
	Automatine用の開発用、MoveAutoにどうするための前段のAuto。
*/
public class BeforeMoveAuto <InitialParamType, UpdateParamType> : Auto <InitialParamType, UpdateParamType> {
	/**
		start "move" auto.
	*/
	public BeforeMoveAuto (int frame, InitialParamType context) : base (
		"移動",
		frame,
		context,
		new Timeline<InitialParamType, UpdateParamType>(
			"移動のタイムライン",
			new Tack<InitialParamType, UpdateParamType>(
				"移動待ち",
				0,
				3,
				AutoConditions.Move.WALK_READY,
				() => {
					var c = new RoutineContexts<InitialParamType, UpdateParamType>();
					c.rContext = c.WalkReady;
					return c;
				}
			),
			new Tack<InitialParamType, UpdateParamType>(
				"移動中", 
				3,
				30, 
				AutoConditions.Move.WALK_ON,
				() => {
					var c = new RoutineContexts<InitialParamType, UpdateParamType>();
					c.rContext = c.Walking;
					return c;
				}
			),
			new Tack<InitialParamType, UpdateParamType>(
				"移動完了！の硬直、Walkでのキャンセルが可能", 
				33,
				5, 
				AutoConditions.Move.WALK_END,
				() => {
					var c = new RoutineContexts<InitialParamType, UpdateParamType>();
					c.rContext = c.WalkEnd;
					return c;
				}
			)
		),
		new Timeline<InitialParamType, UpdateParamType>(
			"キャンセル可能性",
			new Tack<InitialParamType, UpdateParamType>(
				"キャンセル不可",
				0,
				3+30,
				AutoConditions.Canc.TYPEDEF
			)
			,
			new Tack<InitialParamType, UpdateParamType>(
				"キャンセル可能",
				33,
				5,
				AutoConditions.Canc.WALK
				// ,
				// () => {
				// 	var c = new RoutineContexts();
				// 	c.rContext = c.Here;
				// 	return c;
				// }
			)

		)
	){}
}
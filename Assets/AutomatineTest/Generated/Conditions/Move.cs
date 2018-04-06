using System;
/**
	this file is written by Automatine.
*/
public partial class AutoConditions {
	/**
		移動判定
	*/
	public enum Move : int {
		TYPEDEF,

		WALK_READY,
		WALK_ON,
		WALK_END,

		DASH_READY,
		DASH_ON,
		DASH_END,



		ENEMY_MOVE_READY,
		ENEMY_MOVE_ON,
		ENEMY_MOVE_AFTER,
	}
}
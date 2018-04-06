using UnityEngine;

using System;
using System.Collections.Generic;
using Automatine;

/**
	Spawn時に生成されるプレイヤーデータ
		プレイヤーの現在の状態を保持するもので、歴史的な情報は含まない。
		Serializeすることができる前提。
		すべてに[latest]が着く。
*/
public class PlayerContext {
	// settings
	public readonly string playerId;
	public Auto<PlayerContext, Dictionary<string, PlayerContext>> auto;

	public string runMark;
	
	// screenshot parameters
	public int life = 0;

	public int x = 0;
	public int y = 0;
	public int z = 0;
	public int dir = 0;// 真正面


	// flags
	public bool front;
	public bool back;

	public bool right;
	public bool left;

	public bool attack;
	public bool fire;
	public float fx, fy, fz;


	public PlayerContext (string playerId) {
		this.playerId = playerId;

		this.life = 0;
		
		this.x = 0;
		this.y = 0;
		this.z = 0;
		this.dir = 0;

		// flags
		this.front = false;
		this.back = false;
		this.right = false;
		this.left = false;
		
		this.attack = false;
		this.fire = false;
	}

	public PlayerContext (PlayerContext source) {
		this.playerId = source.playerId;

		this.life = source.life;
		
		this.x = source.x;
		this.y = source.y;
		this.z = source.z;
		this.dir = source.dir;

		// flags
		this.front	= source.front;
		this.back	= source.back;
		this.right	= source.right;
		this.left	= source.left;
		
		this.attack	= source.attack;
		this.fire	= source.fire;
	}

	public static PlayerContext Copy (PlayerContext source) {
		return new PlayerContext(source);
	}

	public void Reset () {
		this.front = false;
		this.back = false;
		this.right = false;
		this.left = false;
		
		this.attack = false;
		this.fire = false;
	}

}
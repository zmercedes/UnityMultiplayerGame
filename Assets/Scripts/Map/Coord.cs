using UnityEngine;
using System;

public class Coord : IEquatable<Coord> {
	public int tileX;
	public int tileY;

	public string print(){ return "(" + this.tileX + ", " + this.tileY +")"; }

	public Coord(){ // default constructor
		tileX = 0;
		tileY = 0;
	}
	public Coord(int x, int y) { // two param constructor
		tileX = x;
		tileY = y;
	}
	public Coord(Coord other){ // copy constructor
		tileX = other.tileX;
		tileY = other.tileY;
	}

	// returns coordinate from a unity world point
	// hard coded for now, add height/width parameters later
	public static Coord FromWorldPoint(Vector3 pos){
		float percentX = (pos.x + 40) / 80;
		float percentY = (pos.y + 40) / 80;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((100 -1) * percentX);
		int y = Mathf.RoundToInt((100 -1) * percentY);
		return new Coord(x,y);
	}

	// returns unity world point from coord
	// hard coded for now, add height/width parameters later
	public static Vector3 ToWorldPoint(Coord tile){
		return new Vector3(-40f + .5f + tile.tileX, -40f + .5f + tile.tileY,-1);
	}

	public bool Equals(Coord other){
		return tileX == other.tileX && tileY == other.tileY;
	}
}
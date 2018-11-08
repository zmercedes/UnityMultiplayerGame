using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(MeshGen))]
public class MapGen : NetworkBehaviour {

	public Biome currentBiome;

	[SyncVar] // generates same map across server/clients
	string seed;

	[SyncVar] // tells me if server has generated a map
	bool ready;

	// radius of passages between rooms
	int passageRad = 2;

	// coordinates of current map
	int[,] map;

	// rooms in map
	List<Room> currentRooms;

	// tiles that can be walked on (not a wall)
	List<Coord> walkableTiles;

	void Start(){
		GenerateMap();
	}

	// read property for walkableTiles
	// used to generate a copy for other utilities
	public List<Coord> WalkableTiles{
		get {
			return walkableTiles;
		}
	}

	// read property for ready state
	// used to check if map has been generated on server
	public bool Ready{
		get{
			return ready;
		}
	}

	// sets random seed
	public void SetSeed(){
		seed = DateTime.Now.Millisecond.ToString();
	}

	// generates map using seed
	public void GenerateMap(){
		currentBiome = Biome.forest;
		currentRooms = new List<Room>();
		walkableTiles = new List<Coord>();
		// GetComponent<Renderer>().material = walls;
		map = new int[currentBiome.width,currentBiome.height];
		RandomFillMap();

		for(int i = 0; i<currentBiome.smoothingIterations;i++) 
			SmoothMap();

		ProcessMap();

		int borderSize = 10; // sets border THICCness
		int[,] borderedMap = new int[currentBiome.width + borderSize*2,currentBiome.height + borderSize*2];

		for (int x = 0; x < borderedMap.GetLength(0); x++){
			for (int y = 0; y <  borderedMap.GetLength(1); y++){
				if(x >= borderSize && x < currentBiome.width + borderSize && y >= borderSize && y < currentBiome.height + borderSize)
					borderedMap[x,y] = map[x - borderSize,y - borderSize];
				else
					borderedMap[x,y] = 1;
			}
		}

		MeshGen meshGen = GetComponent<MeshGen>();
		meshGen.GenerateMesh(borderedMap, 1);

	}

	// fills map randomly with values randomly generated
	// from seed
	void RandomFillMap(){
		System.Random rng = new System.Random(seed.GetHashCode());
	
		for (int x = 0; x < currentBiome.width; x++){
			for (int y = 0; y < currentBiome.height; y++){
				if(x == 0 || x == currentBiome.width-1 || y == 0 || y == currentBiome.height-1) // border
					map[x,y] = 1;
				else
					map[x,y] = (rng.Next(0,100) < currentBiome.randomFillPercent) ? 1:0;
			}
		}
	}

	// smoothes the map using cellular automata 
	// this creates cave like pockets/islands
	void SmoothMap() {
		for (int x = 0; x < currentBiome.width; x ++) {
			for (int y = 0; y < currentBiome.height; y ++) {
				int neighborWallTiles = GetSurroundingWallCount(x,y);

				if (neighborWallTiles > currentBiome.automataRule) // rules to mess with for diff shapes
					map[x,y] = 1;
				else if (neighborWallTiles < currentBiome.automataRule) // rules to mess with for diff shapes
					map[x,y] = 0;

			}
		}
	}

	// returns the number of walls that surround given
	// coordinate
	int GetSurroundingWallCount(int gridX, int gridY){ 
		int wallCount = 0;
		for (int neighborX = gridX - currentBiome.totalX; neighborX <= gridX+ currentBiome.totalX ; neighborX++){
			for (int neighborY = gridY-currentBiome.totalY; neighborY <= gridY+currentBiome.totalY; neighborY++){
				if(IsInMapRange(neighborX, neighborY)){
					if(neighborX != gridX || neighborY != gridY)
						wallCount += map[neighborX,neighborY];	
				} else 
					wallCount++;
			}
		}
		return wallCount;
	}

	// removes small cave pockets/islands, connects
	// all remaining rooms
	void ProcessMap(){
		List<List<Coord>> wallRegions = GetRegions(1);

		foreach(List<Coord> wallRegion in wallRegions){
			if(wallRegion.Count < currentBiome.wallThreshold){
				foreach(Coord tile in wallRegion)
					map[tile.tileX,tile.tileY] = 0;
			}
		}

		List<List<Coord>> roomRegions = GetRegions(0);
		List<Room> survivingRooms = new List<Room>();

		foreach(List<Coord> roomRegion in roomRegions){
			if(roomRegion.Count < currentBiome.roomThreshold){
				foreach(Coord tile in roomRegion)
					map[tile.tileX,tile.tileY] = 1;
			} else {
				survivingRooms.Add(new Room(roomRegion, map));
			}
		}
		survivingRooms.Sort();                         // will put biggest room at [0].
		survivingRooms[0].isMain = true;               // biggest room is main room.
		survivingRooms[0].isAccessibleFromMain = true; // the main room is accessible from itself

		if(currentRooms.Count > 0)
			currentRooms.Clear();

		currentRooms = survivingRooms;
		foreach(Room roomTiles in currentRooms){
			foreach(Coord coord in roomTiles.tiles){
				if(!roomTiles.edgeTiles.Contains(coord))
					walkableTiles.Add(coord);
			}
		}

		ConnectClosestRooms(survivingRooms);
		ready = true;
	}

	// connect rooms closest to each other
	void ConnectClosestRooms(List<Room> allRooms, bool forceAccessFromMain = false){
		List<Room> roomListA = new List<Room>();
		List<Room> roomListB = new List<Room>();

		if(forceAccessFromMain){
			foreach(Room room in allRooms){
				if(room.isAccessibleFromMain)
					roomListB.Add(room);
				else
					roomListA.Add(room);
			}
		} else {
			roomListA = allRooms;
			roomListB = allRooms;
		}

		int bestDistance = 0;
		Coord bestTileA = new Coord();
		Coord bestTileB = new Coord();
		Room bestRoomA = new Room();
		Room bestRoomB = new Room();
		bool possibleConnection = false;

		foreach(Room roomA in roomListA){
			if(!forceAccessFromMain){
				possibleConnection = false; // watch ep 7(~9:00) to understand this logic
				if(roomA.connectedRooms.Count > 0)
					continue;
			}

			foreach(Room roomB in roomListB){
				if(roomA == roomB || roomA.IsConnected(roomB))
					continue; // goes to next roomB

				foreach(Coord tileA in roomA.edgeTiles){
					foreach(Coord tileB in roomB.edgeTiles){
						int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));
					
						if(distanceBetweenRooms < bestDistance || !possibleConnection){
							bestDistance = distanceBetweenRooms;
							possibleConnection = true;
							bestTileA = tileA;
							bestTileB = tileB;
							bestRoomA = roomA;
							bestRoomB = roomB;
						}
					}
				}
			}
			if(possibleConnection && !forceAccessFromMain)
				CreatePassage(bestRoomA,bestRoomB,bestTileA,bestTileB);
		}
		if(possibleConnection && forceAccessFromMain){
			CreatePassage(bestRoomA,bestRoomB,bestTileA,bestTileB);
			ConnectClosestRooms(allRooms, true);
		}

		if(!forceAccessFromMain){
			ConnectClosestRooms(allRooms, true);
		}
	}

	void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB){
		Room.ConnectRooms(roomA, roomB);
		//Debug.DrawLine(Coord.ToWorldPoint(tileA),Coord.ToWorldPoint(tileB), Color.green, 100);

		List<Coord> line = GetLine(tileA, tileB);
		foreach(Coord c in line)
			DrawCircle(c,passageRad);
	}

	void DrawCircle(Coord c, int r){
		for(int x = -r; x <= r; x++){
			for(int y = -r; y <= r; y++){
				if(x*x + y*y <= r*r){
					int drawX = c.tileX + x;
					int drawY = c.tileY + y;
					if(IsInMapRange(drawX,drawY)){
						map[drawX,drawY] = 0;
						walkableTiles.Add(new Coord(drawX,drawY));
					}
				}
			}
		}
	}

	List<Coord> GetLine(Coord from, Coord to){
		List<Coord> line = new List<Coord>();

		int x = from.tileX;
		int y = from.tileY;

		int dx = to.tileX - from.tileX;
		int dy = to.tileY - from.tileY;

		bool inverted = false; // toggles ascent/descent, false is ascent

		int step = Math.Sign(dx);
		int gradientStep = Math.Sign(dy);

		int longest = Mathf.Abs(dx);
		int shortest = Mathf.Abs(dy);

		if(longest < shortest){
			inverted = true;
			longest = Mathf.Abs(dy);
			shortest = Mathf.Abs(dx);

			step = Math.Sign(dy);
			gradientStep = Math.Sign(dx);
		}

		int gradientAcc = longest / 2;
		for(int i = 0; i < longest; i++){
			line.Add(new Coord(x,y));

			if(inverted)
				y += step;
			else
				x += step;

			gradientAcc += shortest;
			if(gradientAcc >= longest){
				if(inverted)
					x += gradientStep;
				else
					y += gradientStep;

				gradientAcc -= longest;
			}
		}
		return line;
	}

	List<List<Coord>> GetRegions(int tileType){
		List<List<Coord>> regions = new List<List<Coord>>();
		int[,] mapFlags = new int[currentBiome.width,currentBiome.height];

		for(int x = 0; x < currentBiome.width; x++){
			for(int y = 0; y < currentBiome.height; y++){
				if(mapFlags[x,y] == 0 && map[x,y] == tileType){
					List<Coord> newRegion = GetRegionTiles(x,y);
					regions.Add(newRegion);

					foreach(Coord tile in newRegion){
						mapFlags[tile.tileX,tile.tileY] = 1;
					}
				}
			}
		}
		return regions;
	}

	// gets all the tiles of a pocket/island
	List<Coord> GetRegionTiles(int startX, int startY){
		List<Coord> tiles = new List<Coord>();
		int[,] mapFlags = new int[currentBiome.width,currentBiome.height]; // determine which tiles have been looked at
		int tileType = map[startX,startY];

		Queue<Coord> queue = new Queue<Coord>();
		queue.Enqueue(new Coord(startX,startY));
		mapFlags[startX,startY] = 1;

		while(queue.Count > 0){
			Coord tile = queue.Dequeue();
			tiles.Add(tile);

			for(int x = tile.tileX -1; x <= tile.tileX +1; x++){
				for(int y = tile.tileY -1; y <= tile.tileY+1; y++){
					// checks for non diagonal tiles
					if(IsInMapRange(x,y) && (y == tile.tileY || x == tile.tileX)){
						if(mapFlags[x,y] == 0 && map[x,y] == tileType){
							mapFlags[x,y] = 1;
							queue.Enqueue(new Coord(x,y));
						}
					}
				}
			}
		}
		return tiles;
	}

	public bool IsInMapRange(int x, int y){
		return x >= 0 && x < currentBiome.width && y >= 0 && y < currentBiome.height;
	}

	public class Room : IComparable<Room>, IEnumerable<Coord> {
		public List<Coord> tiles; // tiles that belong to room
		public HashSet<Coord> edgeTiles; // edges of room
		public List<Room> connectedRooms;
		public int roomSize;
		public bool isAccessibleFromMain;
		public bool isMain;
		public static List<Room> currentRooms; // keeps track of currently generated rooms

		public Room(){}

		public Room(List<Coord> roomTiles, int[,] map){
			tiles = roomTiles;
			roomSize = tiles.Count;
			connectedRooms = new List<Room>();

			edgeTiles = new HashSet<Coord>();
			foreach(Coord tile in tiles){
				for(int x = tile.tileX-1; x<= tile.tileX+1; x++){
					for(int y = tile.tileY-1; y<= tile.tileY+1; y++){
						if( x == tile.tileX || y == tile.tileY){ // if tile being checked isn't diagonal
							if(map[x,y] == 1){
								edgeTiles.Add(tile);
							}
						}
					}
				}
			}
		}

		public IEnumerator<Coord> GetEnumerator(){
			return edgeTiles.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator(){
			return edgeTiles.GetEnumerator();
		}

		// 
		public void SetAccessibleFromMainRoom(){
			if(!isAccessibleFromMain){
				isAccessibleFromMain = true;
				foreach(Room connectedRoom in connectedRooms)
					connectedRoom.SetAccessibleFromMainRoom();
			}
		}

		public static void ConnectRooms(Room roomA, Room roomB){
			if(roomA.isAccessibleFromMain)
				roomB.SetAccessibleFromMainRoom();
			else if(roomB.isAccessibleFromMain)
				roomA.SetAccessibleFromMainRoom();

			roomA.connectedRooms.Add(roomB);
			roomB.connectedRooms.Add(roomA);
		}

		public bool IsConnected(Room otherRoom){
			return connectedRooms.Contains(otherRoom);
		}

		// returns tile that is good distance from wall
		public Coord GetPlaceCoord(){ 

			int randTile = UnityEngine.Random.Range(0, tiles.Count - 1);
			int count = 0;
			Coord returnTile = this.tiles[randTile++];

			while(count++ <= this.tiles.Count){
				if(FarFromWall(returnTile)) // was having trouble getting this logic
					break;
				returnTile = tiles[randTile++ % tiles.Count];
			}

			return returnTile;
		}

		// true on tile being 5x5 grid away from wall
		bool FarFromWall(Coord tile){
			if(edgeTiles.Contains(tile))
				return false;

			for(int x = tile.tileX - 2; x <= tile.tileX + 2; x++){
				for(int y = tile.tileY - 2; y <= tile.tileY + 2; y++){
					if(tile.Equals(new Coord(x,y)))
						continue;
					if(edgeTiles.Contains(new Coord(x,y)))
						return false;
				}
			}
			return true;
		}
		
		// when using IComparable<obj> interface, must define CompareTo
		// primitives(?) have compareto defined already
		public int CompareTo(Room otherRoom){ 
			return otherRoom.roomSize.CompareTo(roomSize);
		}
	}
}
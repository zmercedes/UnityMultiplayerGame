using UnityEngine;

[System.Serializable]
public class Biome {

	public int width = 80;             // map width
	public int height = 80;            // map height
	public int smoothingIterations = 5; // # of smoothing iterations
	[Range(0,100)]
	public int randomFillPercent = 73;  // percent to fill map with
	public int automataRule = 5;        // number of adjacent wall tiles
	                                    // used in cell automata 
	public int wallThreshold = 50;      // wall tile island must be > to remain
	public int roomThreshold = 50;      // room must be > to remain
	public int totalX = 1;              // used in automata to check 2x + 1 neighbors
	public int totalY = 1;              // used in automata to check 2y + 1 neighbors
	public int walls;

	private static readonly Biome snowBiome = new Biome(0);
	private static readonly Biome cityBiome = new Biome(1);
	private static readonly Biome forestBiome = new Biome(2);
	private static readonly Biome desertBiome = new Biome(3);

	public Biome(int w) {
		walls = w;
	}

	public static Biome snow{
		get
		{
			return Biome.snowBiome;
		}
	}
	public static Biome city{
		get
		{
			return Biome.cityBiome;
		}
	}
	public static Biome forest{
		get
		{
			return Biome.forestBiome;
		}
	}
	public static Biome desert{
		get
		{
			return Biome.desertBiome;
		}
	}
}
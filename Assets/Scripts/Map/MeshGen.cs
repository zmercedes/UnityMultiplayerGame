/* Zoilo Merdedes
 * Marching Squares Mesh Generator, credit Sebastian Lague
 * Definitions:
 * 	   vertex: point on graph.
 *     edge: two connected vertices
 *     outline edge: two vertices which share exactly one triangle
 * To Do:
 *     CleanUp
 *     Comment
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGen : MonoBehaviour {

	public SquareGrid squareGrid;
	MeshFilter cave;
	Transform caveMesh;

	List<Vector3> vertices;
	List<int> triangles;

	// pass in vertexIndex and get a list of all triangles which vertex is part of.
	Dictionary<int,List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
	List<List<int>> outlines = new List<List<int>>();

	// optimization: contains already checked vertices to make sure they aren't checked more
	// than once. contain() is much faster on hashsets. Used in CalculateMeshOutlines()
	HashSet<int> checkedVertices = new HashSet<int>();

	public void GenerateMesh(int[,] map, float squareSize){
		caveMesh = transform.GetChild(0);
		caveMesh.Rotate(new Vector3(270f, 0f,0f));

		cave = caveMesh.gameObject.GetComponent<MeshFilter>();

		triangleDictionary.Clear();
		outlines.Clear();
		checkedVertices.Clear();

		squareGrid = new SquareGrid(map, squareSize);

		vertices = new List<Vector3>();
		triangles = new List<int>();

		for(int x = 0;x < squareGrid.squares.GetLength(0);x++){
			for(int y = 0;y < squareGrid.squares.GetLength(1);y++){
					TriangulateSquare(squareGrid.squares[x,y]);
			}
		}

		Mesh mesh = new Mesh();
		cave.mesh = mesh;

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();

		Vector2[] uvs = new Vector2[vertices.Count];
		for(int i = 0; i < vertices.Count; i++){
			float percentX = Mathf.InverseLerp(-map.GetLength(0)/2 * squareSize, map.GetLength(0)/2 * squareSize, vertices[i].x);
			float percentY = Mathf.InverseLerp(-map.GetLength(0)/2 * squareSize, map.GetLength(0)/2 * squareSize, vertices[i].z);
			uvs[i] = new Vector2(percentX,percentY);
		}
		mesh.uv = uvs;

		GenerateColliders();
	}

	// generates colliders based on mesh outlines. will generate multiple colliders
	// depending on how many wall islands there are
	void GenerateColliders(){
		EdgeCollider2D[] currentColliders = gameObject.GetComponents<EdgeCollider2D>();
		for(int i=0;i<currentColliders.Length;i++)
			Destroy(currentColliders[i]);

		CalculateMeshOutlines();

		foreach(List<int> outline in outlines){
			EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
			Vector2[] edgePoints = new Vector2[outline.Count];

			for(int i = 0; i < outline.Count; i++)
				edgePoints[i] = new Vector2(vertices[outline[i]].x,vertices[outline[i]].z);
			
			edgeCollider.points = edgePoints;
		}
	}

	void TriangulateSquare(Square square){
		switch (square.configuration){
			case 0:
				break;
			case 1: // bottom left (0001)
				MeshFromPoints(square.centerLeft,square.centerBottom,square.bottomLeft);
				break;
			case 2: // bottom right (0010)
				MeshFromPoints(square.bottomRight,square.centerBottom,square.centerRight);
				break;
			case 4: // top right (0100)
				MeshFromPoints(square.topRight,square.centerRight,square.centerTop);
				break;
			case 8: // top left (1000)
				MeshFromPoints(square.topLeft,square.centerTop,square.centerLeft);
				break;

			case 3: // bottom right, bottom left (0011)
				MeshFromPoints(square.centerRight,square.bottomRight,square.bottomLeft, square.centerLeft);
				break;
			case 6: // top right, bottom right (0110)
				MeshFromPoints(square.centerTop,square.topRight,square.bottomRight, square.centerBottom);
				break;
			case 12: // top left, top right (1100)
				MeshFromPoints(square.topLeft,square.topRight,square.centerRight, square.centerLeft);
				break;
			case 9: // top left, bottom left (1001)
				MeshFromPoints(square.topLeft,square.centerTop,square.centerBottom, square.bottomLeft);
				break;
			case 5: // bottom left, top right (0101)
				MeshFromPoints(square.centerTop,square.topRight,square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
				break;
			case 10: // top left, bottom right (1010)
				MeshFromPoints(square.topLeft,square.centerTop,square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
				break;

			case 7: // top right, bottom right, bottom left (0111)
				MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
				break;
			case 11: // top left, bottom right, bottom left (1011)
				MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
				break; 
			case 13: // top left, top right, bottom left (1101)
				MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
				break;
			case 14: // top right, top left, bottom left (1110)
				MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
				break;

			case 15: // all control points (1111)
				MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
				checkedVertices.Add(square.topLeft.vertexIndex);
				checkedVertices.Add(square.topRight.vertexIndex);
				checkedVertices.Add(square.bottomRight.vertexIndex);
				checkedVertices.Add(square.bottomLeft.vertexIndex);
				break;
		}
	}
	
	void MeshFromPoints(params Node[] points){
		AssignVertices(points);

		if(points.Length >= 3)
			CreateTriangle(points[0],points[1],points[2]);
		if(points.Length >= 4)
			CreateTriangle(points[0],points[2],points[3]);
		if(points.Length >= 5)
			CreateTriangle(points[0],points[3],points[4]);
		if(points.Length >= 6)
			CreateTriangle(points[0],points[4],points[5]);
	}

	void AssignVertices(Node[] points){
		for(int i = 0; i < points.Length; i++){
			if(points[i].vertexIndex == -1){
				//Debug.Log("[AssignVertices()] Vertex: "+vertices.Count);
				points[i].vertexIndex = vertices.Count;
				vertices.Add(points[i].position);
			}
		}
	}

	void CreateTriangle(Node a, Node b, Node c){
		triangles.Add(a.vertexIndex);
		triangles.Add(b.vertexIndex);
		triangles.Add(c.vertexIndex);

		Triangle triangle = new Triangle(a.vertexIndex,b.vertexIndex,c.vertexIndex);
		AddTriangleToDictionary(triangle.vertexIndexA,triangle);
		AddTriangleToDictionary(triangle.vertexIndexB,triangle);
		AddTriangleToDictionary(triangle.vertexIndexC,triangle);		
	}

	// check if triangle dictionary contains vertex index as key,
	// if it does, just add triangle to list of triangles already containing vertex index 
	// otherwise, create new list of triangles and add to dictionary
	void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle){
		if(triangleDictionary.ContainsKey(vertexIndexKey))
			triangleDictionary[vertexIndexKey].Add(triangle);
		else {
			List<Triangle> triangleList = new List<Triangle>();
			triangleList.Add(triangle);
			triangleDictionary.Add(vertexIndexKey,triangleList);
		}
	}

	// vertex indexes c
	void CalculateMeshOutlines(){ // study this method
		for(int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++){
			if(!checkedVertices.Contains(vertexIndex)){
				int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
				if(newOutlineVertex != -1){
					checkedVertices.Add(vertexIndex);

					List<int> newOutline = new List<int>();
					newOutline.Add(vertexIndex);
					outlines.Add(newOutline);
					FollowOutline(newOutlineVertex, outlines.Count - 1);
					outlines[outlines.Count - 1].Add(vertexIndex);
				}
			}
		}
	}

	void FollowOutline(int vertexIndex, int outlineIndex){
		outlines[outlineIndex].Add(vertexIndex);
		checkedVertices.Add(vertexIndex);
		//Debug.Log(vertexIndex);
		int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

		if(nextVertexIndex != -1)
			FollowOutline(nextVertexIndex, outlineIndex);
	}

	int GetConnectedOutlineVertex(int vertexIndex){
		List<Triangle> containingVertex = triangleDictionary[vertexIndex];

		for(int i = 0; i < containingVertex.Count; i++){
			Triangle triangle = containingVertex[i];

			for(int j = 0; j < 3; j++){
				int vertexB = triangle[j];
				if(vertexB != vertexIndex && !checkedVertices.Contains(vertexB)){ // optimization using hashset
					if(IsOutlineEdge(vertexIndex, vertexB))
						return vertexB;
				}
			}
		}

		//Debug.Log("[GetConnectedOutlineVertex()] returning -1 with "+vertexIndex);
		return -1;
	}

	bool IsOutlineEdge(int vertexA, int vertexB){
		List<Triangle> containingVertexA = triangleDictionary[vertexA];
		int sharedTriangleCount = 0;

		for(int i = 0;i < containingVertexA.Count; i++){
			if(containingVertexA[i].Contains(vertexB)){
				sharedTriangleCount++;
				if(sharedTriangleCount > 1)
					break;
			}
		}
		return sharedTriangleCount == 1;
	}

	struct Triangle{
		public int vertexIndexA;
		public int vertexIndexB;
		public int vertexIndexC;
		int[] vertices;

		public Triangle(int a,int b,int c){
			vertexIndexA = a;
			vertexIndexB = b;
			vertexIndexC = c;

			vertices = new int[3];
			vertices[0] = a;
			vertices[1] = b;
			vertices[2] = c;
		}

		public int this[int i]{
			get{ return vertices[i]; }
		}
		
		public bool Contains(int vertexIndex){
			return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
		}
	}

	public class SquareGrid {
		public Square[,] squares;

		public SquareGrid(int[,] map, float squareSize) {
			int nodeCountX = map.GetLength(0);
			int nodeCountY = map.GetLength(1);
			float mapWidth = nodeCountX * squareSize;
			float mapHeight = nodeCountY * squareSize;

			ControlNode[,] controlNodes = new ControlNode[nodeCountX,nodeCountY];

			for(int x = 0;x < nodeCountX;x++){
				for(int y = 0;y < nodeCountY;y++){
					Vector3 pos = new Vector3(-mapWidth/2 + x * squareSize + squareSize/2,0,-mapHeight/2 + y * squareSize + squareSize/2);
					controlNodes[x,y] = new ControlNode(pos,map[x,y] == 1,squareSize);
				}
			}

			squares = new Square[nodeCountX -1,nodeCountY -1];
			// initialize squares in squares[,]
			for(int x = 0;x < nodeCountX-1;x++){
				for(int y = 0;y < nodeCountY-1;y++){
					squares[x,y] = new Square(controlNodes[x,y+1],controlNodes[x+1,y+1],controlNodes[x+1,y], controlNodes[x,y]);
				}
			}
		}
	}

	public class Square {
		public ControlNode topLeft, topRight, bottomLeft, bottomRight;
		public Node centerTop, centerRight, centerBottom, centerLeft;
		public int configuration;

		public Square(ControlNode _tL,ControlNode _tR,ControlNode _bR,ControlNode _bL) {
			topLeft = _tL;
			topRight = _tR;
			bottomLeft = _bL;
			bottomRight = _bR;

			centerTop = topLeft.right;
			centerRight = bottomRight.above;
			centerLeft = bottomLeft.above;
			centerBottom = bottomLeft.right;

			if(topLeft.active)
				configuration += 8;
			if(topRight.active)
				configuration += 4;
			if(bottomRight.active)
				configuration += 2;
			if(bottomLeft.active)
				configuration += 1;
		}
	}

	public class Node {
		public Vector3 position;
		public int vertexIndex = -1;

		public Node(Vector3 _pos){
			position = _pos;
		}
	}

	public class ControlNode : Node {

		public bool active;
		public Node above, right;

		public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) {
			active = _active;
			above = new Node(position + Vector3.forward * squareSize/2f);
			right = new Node(position + Vector3.right * squareSize/2f);
		}
	}
}
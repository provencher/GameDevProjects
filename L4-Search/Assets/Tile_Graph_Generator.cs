using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile_Graph_Generator : MonoBehaviour {

	//Fields
	public Tile_Node node;
	public Vector3 levelSize;
	public float radius;
	public Vector3 tileSize;
	public float tileSize_Density;
	public List<Tile_Node> nodeList = new List<Tile_Node>();
	bool executeOnceFlag = false;

	public Tile_Node startNode, endNode;

	public List<Tile_Node> pathList = new List<Tile_Node>();
	public List<Tile_Node> openList = new List<Tile_Node>();
	public List<Tile_Node> closedList = new List<Tile_Node>();
	public NPC npc;
	public Tile_Node endNodeIndicator;

	public enum searchAlgorithm {BreadthFirst, DepthFirst};
	public searchAlgorithm algorithm;


	// Use this for initialization
	void Start ()
	{
		levelSize = new Vector3(50, 0, 50);
		tileSize = new Vector3(levelSize.x / tileSize_Density, 0, levelSize.z / tileSize_Density);
		radius = tileSize.x / 2;

		algorithm = searchAlgorithm.BreadthFirst;

		//Generate the tiles, initialize the start node, and make the first calculation
		Scan ();
		FindStartNode ();
		FindEndNode ();
		startNode.costSoFar = 0;
		startNode.heuristicValue = (endNode.transform.position - startNode.transform.position).magnitude;
		calculateBreadthFirst ();
	}


	// Update is called once per frame
	public int pathCounter = 0;
	void Update ()
	{
		//Check if 'a' is pressed. If it is, cycle the algorithm
		if (Input.GetKeyDown (KeyCode.A)) 
		{
			algorithm++;
			if ((int)algorithm > 1)
			{
				algorithm = 0;
			}
		}

		//Check if the left mouse button is pressed. If it is,
		//turn the node that it selected into the end node
		if (Input.GetMouseButtonDown (0)) 
		{
			Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit[] rayHits = Physics.RaycastAll (mouseRay);
			foreach (RaycastHit rayHit in rayHits) 
			{
				if (rayHit.collider.GetComponent (typeof(Tile_Node))) 
				{
					Tile_Node newEndNode = (Tile_Node)rayHit.collider.GetComponent (typeof(Tile_Node));
					endNode = newEndNode;
				}
			}
			print ("Mouse Pressed!");
		}

		//If the NPC is in the middle of a pathfinding journey, let it continue
		if (pathList.Count > pathCounter && endNode == pathList[pathList.Count - 1])
		{
			endNodeIndicator.transform.position = endNode.transform.position;
			if (Vector3.Angle (npc.transform.forward, (pathList[pathCounter].transform.position - npc.transform.position)) > 35)
			{
				npc.Steering_Stop ();
				npc.rotateTowards (pathList[pathCounter].transform.position);
			}
			else {
				if (pathCounter == pathList.Count - 1) 
				{
					npc.Steering_Arrive (pathList[pathCounter].transform.position, true);
				}
				else 
				{
					npc.Steering_Arrive (pathList[pathCounter].transform.position, false);
				}
			}
			bool nodeAttained = false;
			Collider[] collisionArray = Physics.OverlapSphere (npc.transform.position, 0.2f);
			for (int i = 0; i < collisionArray.Length; i++) {
				if (collisionArray[i].GetComponent (typeof(Tile_Node)) == pathList[pathCounter]) {
					nodeAttained = true;
				}
			}
			
			if (nodeAttained) {
				pathCounter++;
			}
		}
		//If a new end node has been selected, recalculate route.
		else if ( pathList.Count == 0 || endNode != pathList[pathList.Count - 1]) {
			try {
				FindStartNode ();
				openList.Clear ();
				closedList.Clear ();
				pathList.Clear ();
				startNode.costSoFar = 0;
				startNode.heuristicValue = (endNode.transform.position - startNode.transform.position).magnitude;
				pathCounter = 0;
				switch(algorithm)
				{
					case searchAlgorithm.BreadthFirst:
						calculateBreadthFirst ();
						break;

					default:
						break;
				}
			}
			catch (System.Exception e) {
				print (e.GetType ());
				FindEndNode ();
				return;
			}
		}

		//Color the nodes according to their nature.
		foreach (Tile_Node node in nodeList) {
			node.TurnVisible ();
			node.GetComponent<Renderer>().material.color = Color.white;
		}
		foreach (Tile_Node node in openList) {
			node.GetComponent<Renderer>().material.color = Color.yellow;
		}
		foreach (Tile_Node node in closedList) {
			node.GetComponent<Renderer>().material.color = Color.yellow;
		}
		foreach (Tile_Node node in pathList) {
			node.GetComponent<Renderer>().material.color = Color.green;
		}
		startNode.GetComponent<Renderer>().material.color = Color.blue;
		endNode.GetComponent<Renderer>().material.color = Color.red;
	}

	//Scan the graph for tile placements
	void Scan() {
		for (int i = 0; i < tileSize_Density; i++) {
			for (int j = 0; j < tileSize_Density; j++) {
				Vector3 center = new Vector3(j*tileSize.x + ((-levelSize.x / 2) + (tileSize.x / 2)), 0, (-i*tileSize.z) + (levelSize.z / 2 - tileSize.z / 2));
				//layerMask parameter has to be in binary
				if (Physics.OverlapSphere (center, /*radius*/0.50f, 1000).Length == 0) {
					Tile_Node tempNode;
					//Instantiate(node, center, new Quaternion());
					nodeList.Add ((Tile_Node)Instantiate(node, center, new Quaternion()));
				}
			}
		}
		for (int i = 0; i < nodeList.Count; i++) {
			GenerateNeighbors (nodeList[i]);
		}
	}

	//Find the start node according to the position of the NPC
	void FindStartNode() {
		for (int i = 0; i < nodeList.Count; i++) {
			if (i == 0) {
				startNode = nodeList[i];
			}
			else {
				if ((npc.transform.position - nodeList[i].transform.position).magnitude < (npc.transform.position - startNode.transform.position).magnitude) {
					startNode = nodeList[i];
				}
			}
		}
	}


	//Find the end node according to the position end node indicator
	void FindEndNode() {
		for (int i = 0; i < nodeList.Count; i++) 
		{
			if (i == 0) 
			{
				endNode = nodeList[i];
			}
			else {
				if ((endNodeIndicator.transform.position - nodeList[i].transform.position).magnitude < (endNodeIndicator.transform.position - endNode.transform.position).magnitude) {
					endNode = nodeList[i];
				}
			}
		}
	}

	//Generate the neighbors for a Tile_Node
	void GenerateNeighbors(Tile_Node node) {
		node.neighborList[(int)Tile_Node.NeighborNodes.UpLeft] = nodeList.Find (n => (n.transform.position - node.transform.position) == (new Vector3(-tileSize.x, 0, tileSize.z)));
		node.neighborList[(int)Tile_Node.NeighborNodes.Up] = nodeList.Find (n => (n.transform.position - node.transform.position) == (new Vector3(0, 0, tileSize.z)));
		node.neighborList[(int)Tile_Node.NeighborNodes.UpRight] = nodeList.Find (n => (n.transform.position - node.transform.position) == (new Vector3(tileSize.x, 0, tileSize.z)));
		node.neighborList[(int)Tile_Node.NeighborNodes.Right] = nodeList.Find (n => (n.transform.position - node.transform.position) == (new Vector3(tileSize.x, 0, 0)));
		node.neighborList[(int)Tile_Node.NeighborNodes.DownRight] = nodeList.Find (n => (n.transform.position - node.transform.position) == (new Vector3(tileSize.x, 0, -tileSize.z)));
		node.neighborList[(int)Tile_Node.NeighborNodes.Down] = nodeList.Find (n => (n.transform.position - node.transform.position) == (new Vector3(0, 0, -tileSize.z)));
		node.neighborList[(int)Tile_Node.NeighborNodes.DownLeft] = nodeList.Find (n => (n.transform.position - node.transform.position) == (new Vector3(-tileSize.x, 0, -tileSize.z)));
		node.neighborList[(int)Tile_Node.NeighborNodes.Left] = nodeList.Find (n => (n.transform.position - node.transform.position) == (new Vector3(-tileSize.x, 0, 0)));
	}

	IComparer<Tile_Node> comparer = new Tile_Node();

	//Breadth First visit node
	void visitNode_BreadthFirst(Tile_Node node) {
		for (int i = 0; i < node.neighborList.Length; i++) {
			if (node.neighborList[i] && !Physics.Linecast (node.transform.position, node.neighborList[i].transform.position, 1000)) {
				if (!closedList.Contains (node.neighborList[i]) && !openList.Contains (node.neighborList[i])) {
					node.neighborList[i].prevNode = node;
                    openList.Insert (openList.Count - 1, node.neighborList[i]);
                    //openList.Insert(0, node.neighborList[i]);

                }
			}
		}
		closedList.Add (node);
		openList.Remove(node);
	}


	//Breadth First algorithm
	void calculateBreadthFirst() {
		openList.Add (startNode);
		
		while (openList[0] != endNode) {
			visitNode_BreadthFirst (openList[0]);
		}
		
		pathList.Add (openList [0]);
		while(true) {
			
            
			if (pathList[pathList.Count - 1].prevNode == startNode) {
				pathList.Add (pathList[pathList.Count - 1].prevNode);
				pathList.Reverse ();
				return;
			}
			else {
				pathList.Add (pathList[pathList.Count - 1].prevNode);
			}
            
		}
	}

	void OnGUI() 
	{   
		// Make a background box
		GUI.Box(new Rect(10, 10, 275, 120), "Game Info");
		GUI.Box(new Rect(10, 30, 275, 30), "(press 'a' to change) Algorithm: " + algorithm.ToString ());
		GUI.Box(new Rect(10, 60, 275, 30), "Start node -> Blue, End node -> Red");
		GUI.Box(new Rect(10, 90, 275, 30), "Visited node -> Yellow, Path node -> Green");

	}
}

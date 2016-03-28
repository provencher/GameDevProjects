using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour {
    [SerializeField]
    public GameObject ToggleHeuristicButton;
    private Text buttonText;

    public enum heuristics { NULL, EUCLIDIAN, CLUSTER};
    public heuristics heuristic;
    
	public Grid grid;
    public static Pathfinding instance;

    public void ToggleHeuristic()
    {
        heuristic = (heuristic == heuristics.NULL ? heuristics.EUCLIDIAN : heuristics.NULL);
        PlayerPrefs.SetInt("Heuristic", (int)heuristic);
        buttonText.text = (heuristic == heuristics.NULL ? "NULL" : "EUCLIDIAN");
    }

    void Awake() {
		grid = GetComponent<Grid>();
		instance = this;
        buttonText = ToggleHeuristicButton.GetComponent<Text>();
        if (PlayerPrefs.HasKey("Heuristic"))
        {
            heuristic = (heuristics)PlayerPrefs.GetInt("Heuristic", 0);
        }        

        buttonText.text = (heuristic == heuristics.NULL ? "NULL" : "EUCLIDIAN");
    }
    
	public static Vector2[] RequestPath(Vector2 from, Vector2 to) {
		return instance.FindPath (from, to);
	}
	
	Vector2[] FindPath(Vector2 from, Vector2 to) {       

        Stopwatch sw = new Stopwatch();
		sw.Start();
		
		Vector2[] waypoints = new Vector2[0];
		bool pathSuccess = false;
		
		Node startNode = grid.NodeFromWorldPoint(from);
		Node targetNode = grid.NodeFromWorldPoint(to);        
		startNode.parent = startNode;
		
		
		if (startNode.walkable && targetNode.walkable) {
			Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
            startNode.debugNode = true;     

            while (openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst();
                currentNode.debugNode = true;
				closedSet.Add(currentNode);              
				
				if (currentNode == targetNode) {
					sw.Stop();				
					pathSuccess = true;
					break;
				}
				
				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
					if (!neighbour.walkable || closedSet.Contains(neighbour)) {
						continue;
					}

                    int newMovementCostToNeighbour =  currentNode.gCost + GetDistance(currentNode, neighbour)+TurningCost(currentNode,neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                        neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = (GetDistance(neighbour, targetNode) * neighbour.wallCost);
						neighbour.parent = currentNode;
						
						if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                            neighbour.debugNode = true;
                        }							
						else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
				}                
            }           
        }  
        
      

		if (pathSuccess) {
			waypoints = RetracePath(startNode,targetNode);
		}

		return waypoints;
		
	}

	
	int TurningCost(Node from, Node to) {
		return 0;
		Vector2 dirOld = new Vector2(from.gridX - from.parent.gridX, from.gridY - from.parent.gridY);
		Vector2 dirNew = new Vector2(to.gridX - from.gridX, to.gridY - from.gridY);
		if (dirNew == dirOld)
			return 0;
		else if (dirOld.x != 0 && dirOld.y != 0 && dirNew.x != 0 && dirNew.y != 0) {
			return 5;
		}
		else {
			return 10;
		}
	}
	
	Vector2[] RetracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		
		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		Vector2[] waypoints = MakeArrayFromGridPositions(path);
		Array.Reverse(waypoints);
		return waypoints;
		
	}
	
	Vector2[] MakeArrayFromGridPositions(List<Node> path) {
		List<Vector2> waypoints = new List<Vector2>();
		
		for (int i = 1; i < path.Count; i ++) {			
			waypoints.Add(path[i].worldPosition);			
		}
		return waypoints.ToArray();
	}
	
	int GetDistance(Node nodeA, Node nodeB) {   
        switch(heuristic)
        {
            case heuristics.CLUSTER:                
            case heuristics.EUCLIDIAN:
                return Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow((nodeA.gridX - nodeB.gridX), 2) + Mathf.Pow((nodeA.gridY - nodeB.gridY), 2)));
            default:
                return 1;
        }  
	}
	
	
}

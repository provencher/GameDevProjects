using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// An implementation of the flocking algorithm: http://www.red3d.com/cwr/boids/
// Additional resources:
// http://harry.me/2011/02/17/neat-algorithms---flocking/
public class SwarmBehavior : MonoBehaviour {
    /// <summary>
    /// the number of drones we want in this swarm
    /// </summary>
    /// 

    public int numberOfDrones;

    [SerializeField]
    public GameObject gameBoundary;

    [SerializeField]
    public GameObject target;    

    [SerializeField]
    public GameObject dronePrefab;

    List<GameObject> drones;
    bool started = false;

    [SerializeField]
    public Slider cohesion;

    [SerializeField]
    public Slider sepDistance;

    [SerializeField]
    public Slider nRadius;


    // Use this for initialization
    void Start ()
	{
        drones = new List<GameObject>();
    }


    void SetupGame()
    {
        started = true;
        Vector3 topRight = gameBoundary.GetComponent<ScreenEdge>().topRight;
        Vector3 bottomLeft = gameBoundary.GetComponent<ScreenEdge>().bottomLeft;

        //Spawn Players
        for (int i = 0; i < numberOfDrones; i++)
        {
            float spawnX = Random.Range(bottomLeft.x, topRight.x);
            float spawnY = Random.Range(bottomLeft.y, topRight.y);

            Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);

            drones.Add((GameObject)Instantiate(dronePrefab, spawnPos, Quaternion.identity));
            drones[i].GetComponent<DroneBehavior>().target = target.transform;            
        }        
    }

    public void UpdateWeights()
    {
        foreach(var d in drones)
        {
            d.GetComponent<DroneBehavior>().neighbourRadius = nRadius.value;
            d.GetComponent<DroneBehavior>().cohesionWeight = cohesion.value;
            d.GetComponent<DroneBehavior>().separationWeight = sepDistance.value;
        }
    }


    // Update is called once per frame
    void Update ()
	{
	    if(!started)
        {
            SetupGame();
            started = true;
        }
	}
}

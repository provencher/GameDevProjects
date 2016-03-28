using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class TagGameLogic : NetworkBehaviour
{
    
    public int numberOfPlayers = 4;
    public UnitContoller[] players;

    [SerializeField]
    public GameObject gameBoundary;

    [SerializeField]
    public GameObject unitPrefab;
   

    public bool started = false;
    public bool oneRemaining = false;
    public int frozenPlayers = 0;

    public bool viewWaypoints = true;
    public List<GameObject> landmarkObjs;

    public void ToggleViewWayPoints()
    {
        viewWaypoints = (viewWaypoints ? false : true);
        foreach (var l in landmarkObjs)
        {            
            l.GetComponent<MeshRenderer>().enabled = viewWaypoints;           
        }      
    }
	

    public void SetupGame()
    {

        foreach (var l in GameObject.FindGameObjectsWithTag("Landmark"))
        {            
            landmarkObjs.Add(l);
            l.GetComponent<MeshRenderer>().enabled = false;      
        }

        players = FindObjectsOfType<UnitContoller>();

        //Select first non-seeker
        int seekerIndex = Random.Range(0, players.Length);

        for (var i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<UnitContoller>().initialize();
            //players[i].GetComponent<Complete.TankMovement>().Initialize();

            if (i != seekerIndex)
            {
                players[i].gameObject.tag = "Seeker";
                players[i].GetComponent<SteeringController>().maxVelocity = 2;
                players[i].GetComponent<SteeringController>().maxAcceleration = 5;

            }
            else
            {
                players[i].gameObject.tag = "Unit";
                players[i].GetComponent<SteeringController>().maxAcceleration = 10;
                players[i].GetComponent<SteeringController>().maxVelocity = 5;
            }
        }
        started = true;
    }
	
    void CheckIfAllFrozen()
    {
        if(frozenPlayers == numberOfPlayers - 1)
        {
            foreach(var p in players)
            {       
                p.GetComponent<UnitContoller>().frozen = false;
                if(p.GetComponent<UnitContoller>().lastFrozen)
                {
                    GameObject curSeeker = GameObject.FindGameObjectWithTag("Seeker");
                    curSeeker.tag = "Unit";
                    curSeeker.GetComponent<UnitContoller>().seeker = false;

                    p.GetComponent<UnitContoller>().lastFrozen = false;
                    p.GetComponent<UnitContoller>().seeker = true;
                    p.tag = "Seeker";

                    frozenPlayers = 0;
                }
            }
        }
    }

    void UpdateUnitVelocities()
    {
        foreach (var p in players)
        {
            //Balance game with different max velocities
            if (p.GetComponent<UnitContoller>().seeker)
            {
                p.GetComponent<SteeringController>().maxVelocity = 3.5f;
            }
            else
            {
                p.GetComponent<SteeringController>().maxVelocity = 2.0f;
            }
        }
    }
	// Update is called once per frame
	void Update () {
        if(!started)
        {
            SetupGame();
        }       
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TagGameLogic : MonoBehaviour {
    
    public int numberOfPlayers = 4;
    public List<GameObject> players;

    [SerializeField]
    public GameObject gameBoundary;

    [SerializeField]
    public GameObject unitPrefab;

    private bool started = false;
    public bool oneRemaining = false;
    public int frozenPlayers = 0;

    public bool isKinetic;
    public bool isStopGo; 

    [SerializeField]
    public GameObject button;

    [SerializeField]
    public GameObject stopButton;

	// Use this for initialization
	void Start () {
        players = new List<GameObject>();

        isKinetic = false;
        isStopGo = false;
    }

    public void ToggleKinematic()
    {
        if(isKinetic)
        {
            isKinetic = false;
            button.GetComponent<Text>().text = "Steering";
        }
        else
        {
            isKinetic = true;
            button.GetComponent<Text>().text = "Kinematic";
        }

        SetKinematic(isKinetic);
    }

    void SetKinematic(bool kinematic)
    {
        SteeringController[] units = FindObjectsOfType<SteeringController>();
        foreach (var unit in units)
        {
            unit.notKinetic = (kinematic ? 0 : 1);
        }
    }

    public void StopGo()
    {
        if (!isStopGo)
        {
            isStopGo = true;
            stopButton.GetComponent<Text>().text = "Stop & Go";
        }
        else
        {
            isStopGo = false;
            stopButton.GetComponent<Text>().text = "Smooth";
        }

        SetStopGo(isStopGo);
    }

    void SetStopGo(bool stopgo)
    {
        UnitContoller[] units = FindObjectsOfType<UnitContoller>();
        foreach (var unit in units)
        {
            unit.stopGo = stopgo;
        }
    }

    void SetupGame()
    {
        started = true;
        Vector3 topRight = gameBoundary.GetComponent<ScreenEdge>().topRight;
        Vector3 bottomLeft = gameBoundary.GetComponent<ScreenEdge>().bottomLeft;       

        //Spawn Players
        for(int i = 0; i < numberOfPlayers; i++ )
        {
            float spawnX = Random.Range(bottomLeft.x, topRight.x);
            float spawnY = Random.Range(bottomLeft.y, topRight.y);

            Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);

            players.Add((GameObject)Instantiate(unitPrefab, spawnPos, Quaternion.identity));

            if (isKinetic)
            {
                players[players.Count - 1].GetComponent<SteeringController>().notKinetic = 0;
            }
        }

        //Select first seeker
        int seekerIndex = Random.Range(0, players.Count - 1);
        players[seekerIndex].GetComponent<UnitContoller>().seeker = true;
        players[seekerIndex].gameObject.tag = "Seeker";


        SetStopGo(isStopGo);
        SetKinematic(isKinetic);
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
            //SetupGame();
        }
        //CheckIfAllFrozen();
        //UpdateUnitVelocities();
    }
}

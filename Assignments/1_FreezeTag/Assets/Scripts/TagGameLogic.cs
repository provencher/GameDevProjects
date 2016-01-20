using UnityEngine;
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

	// Use this for initialization
	void Start () {
        players = new List<GameObject>();       
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
        }

        //Select first seeker
        int seekerIndex = Random.Range(0, players.Count - 1);
        players[seekerIndex].GetComponent<UnitContoller>().seeker = true;
        players[seekerIndex].gameObject.tag = "Seeker";
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

	// Update is called once per frame
	void Update () {
        if(!started)
        {
            SetupGame();
        }
        CheckIfAllFrozen();
    }
}

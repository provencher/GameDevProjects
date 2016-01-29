using UnityEngine;
using System.Collections;

public class spawnNPC : MonoBehaviour {
	
	public Vector3 spawnLocation;
	public GameObject myCube;
	public float timer = 0.0f;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	void spawn_NPC()
	{
        CreateCube();
	}
	
	void CreateCube() 
	{
		spawnLocation = new Vector3(Random.value * 10, 0, 0);
		Instantiate(myCube, spawnLocation, Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () 
	{
		timer += Time.deltaTime;
		
		if (timer > 2)
		{
			spawn_NPC ();
			timer = 0.0f;
		}
	}
}
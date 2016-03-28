using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DestroyWall : NetworkBehaviour
{
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D coll)
    {       
        if (coll.gameObject.tag == "Bullet")
        {
            CmdBreakWall();
            Destroy(coll.gameObject);
        }              
    }

    [Command]
    void CmdBreakWall()
    {
        Destroy(gameObject);
        Pathfinding.instance.grid.CreateGrid();
    }
}

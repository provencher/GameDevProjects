using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Power : NetworkBehaviour
{
    [Command]
    public void CmdPickup()
    {
        GetComponent<AudioSource>().Play();
        transform.localScale = Vector3.zero;
        Invoke("Reset", 5f);
    }

    void Reset()
    {
        transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }

}

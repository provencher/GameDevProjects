using UnityEngine;
using System.Collections;

public class FallRespawn : MonoBehaviour
{
    public Transform respawnPoint;

    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.ToLowerInvariant() == "player")
        {
            Instantiate(other, respawnPoint.position, other.transform.rotation);
            Destroy(other.gameObject);
        }
    }
}

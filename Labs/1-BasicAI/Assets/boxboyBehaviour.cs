using UnityEngine;
using System.Collections;

public class boxboyBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        transform.Translate(horizontal, vertical, 0);
    }

    void OnCollisionEnter(Collision col)
    {       

        if (col.gameObject.tag == "Cube")
        {
            GameObject.Find("NPCObject").GetComponent<score>().value++;
            col.gameObject.GetComponent<Rigidbody>().useGravity = true;
            //Destroy(col.gameObject);
        }
    }
}

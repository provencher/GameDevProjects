using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NearSensor : MonoBehaviour {

	public HashSet<Rigidbody> targets = new HashSet<Rigidbody>();


    /*
	void OnTriggerEnter(Collider other) {
		targets.Add (other.gameObject.transform.root.GetComponent<Rigidbody>());
	}

	
	void OnTriggerExit(Collider other) {
		targets.Remove (other.gameObject.transform.root.GetComponent<Rigidbody>());
	}
    */
}

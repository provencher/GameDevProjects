using UnityEngine;
using System.Collections;

public class Align : MonoBehaviour
{
    public Transform target;

    public SteeringController control;
    public Rigidbody rb;
    Vector3 targetVector;

    void Start()
    {
        control = GetComponent<SteeringController>();
        rb = GetComponent<Rigidbody>();
        target = GameObject.Find("Stationary Car").transform;
        targetVector = target.GetComponent<Rigidbody>().velocity;
    }

    void Update()
    {
        targetVector = (target.GetComponent<Rigidbody>().velocity - rb.velocity).normalized;      
        
        rb.AddTorque(targetVector);
                  
    }
}

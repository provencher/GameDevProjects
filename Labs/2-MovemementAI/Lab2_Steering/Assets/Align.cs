using UnityEngine;
using System.Collections;

public class Align : MonoBehaviour
{
    public Transform target;

    public SteeringController control;
    public Rigidbody rb;
    Quaternion targetRotation;

    void Start()
    {
        control = GetComponent<SteeringController>();
        rb = GetComponent<Rigidbody>();
        target = GameObject.Find("Stationary Car").transform;
        targetRotation = target.GetComponent<Rigidbody>().rotation;
    }

    void Update()
    {
        rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.deltaTime);

        //targetVector = (target.GetComponent<Rigidbody>().velocity - rb.velocity).normalized;              
        //rb.AddTorque(targetVector);
                  
    }
}

using UnityEngine;
using System.Collections;

public class Arrive : MonoBehaviour
{
    public Transform target;
    public float moveSpeed;
    public float nearSpeed;
    public float nearRadius;

    public SteeringController control;
    public Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        control = GetComponent<SteeringController>();
        rb = GetComponent<Rigidbody>();
        target = GameObject.Find("Stationary Car").transform;
    }

    void Update()
    {
        //Debug.Log("Arrive: " + rb.velocity.ToString());
        if(moveSpeed > 0.01f)
        {
            if (Vector3.Distance(transform.position, target.position) < nearRadius * 2)
            {
                moveSpeed /= 2;
            }                                          
        }

        if (control.notKinetic == 0)
        {
            //transform.position += moveSpeed / 10 * control.arrive(target.position) * Time.deltaTime;
            rb.velocity = moveSpeed * control.arrive(target.position).normalized;
        }
        else
        {
            if(rb.velocity.magnitude < control.maxVelocity)
            {
                rb.AddForce(moveSpeed * control.arrive(target.position).normalized, ForceMode.VelocityChange);
            }
        }
    }
}

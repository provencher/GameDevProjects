using UnityEngine;
using System.Collections;

public class Wander : MonoBehaviour
{
    public float moveSpeed;

    public SteeringController control;
    public Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        control = GetComponent<SteeringController>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(control.notKinetic == 0)
        {
            //transform.position += moveSpeed / 10 * (transform.position - control.wanderPosition()) * Time.deltaTime;
            rb.velocity = moveSpeed * control.seek(control.wanderPosition()).normalized;
        }
        else
        {
            if (rb.velocity.magnitude < control.maxVelocity)
            {
                rb.AddForce(moveSpeed * control.seek(control.wanderPosition()).normalized, ForceMode.VelocityChange);
            }
        }
    }
}

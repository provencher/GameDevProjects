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
            transform.position += moveSpeed / 10 * (transform.position - control.wanderPosition()) * Time.deltaTime;
        }
        else
        {
            rb.AddForce((transform.position - control.wanderPosition())*25, ForceMode.Impulse);
        }
    }
}

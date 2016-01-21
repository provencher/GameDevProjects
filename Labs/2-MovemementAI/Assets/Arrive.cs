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
    }

    void Update()
    {
        if(moveSpeed > 0.01f)
        {
            if (Vector3.Distance(transform.position, target.position) < nearRadius)
            {
                moveSpeed /= 2;
            }
            transform.position += moveSpeed / 10 * control.arrive(target.position) * Time.deltaTime;
        }
        
    }
}

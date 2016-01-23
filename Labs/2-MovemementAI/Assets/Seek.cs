using UnityEngine;
using System.Collections;

public class Seek : MonoBehaviour
{
    public Transform target;
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
        // transform.position +=  moveSpeed / 20 * control.seek(target.position) * Time.deltaTime;
        //Debug.Log(control.seek(target.position));
        //rb.AddForce(-1 * moveSpeed * control.seek(target.position), ForceMode.Impulse);
        if (control.notKinetic == 0)
        {
            //transform.position += moveSpeed / 10 * control.seek(target.position) * Time.deltaTime;
            rb.velocity = moveSpeed * control.seek(target.position).normalized;
        }
        else
        {
            if (rb.velocity.magnitude < control.maxVelocity)
            {
                rb.AddForce(moveSpeed * control.seek(target.position).normalized, ForceMode.VelocityChange);
            }
        }
    }
}

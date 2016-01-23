using UnityEngine;
using System.Collections;

public class Flee : MonoBehaviour
{
    public Transform pursuer;
    public float moveSpeed;

    public SteeringController control;
    public Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        control = GetComponent<SteeringController>();
        rb = GetComponent<Rigidbody>();
        pursuer = GameObject.Find("Seeking Car").transform;
    }

    void Update()
    {
        //Debug.Log(control.seek(pursuer.position));
        if (control.notKinetic == 0)
        {
            //transform.position += -1 * moveSpeed / 10 * control.seek(pursuer.position) * Time.deltaTime;
            rb.velocity = moveSpeed * -1 *  control.seek(pursuer.position).normalized;
        }
        else
        {
            if (rb.velocity.magnitude < control.maxVelocity)
            {
                rb.AddForce(-1 * moveSpeed *  control.seek(pursuer.position).normalized, ForceMode.VelocityChange);
            }
        }
    }
}

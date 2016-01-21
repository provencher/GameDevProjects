using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float Speed = 10000f;
    public float TurnSpeed = 10f;
    public Transform centerOfMass;
    public Transform wheelBackLeft;
    public Transform wheelBackRight;

    private bool grounded = false;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
        grounded = false;
    }

    void FixedUpdate()
    {
        if (!grounded)
            return;

        float force = Input.GetAxis("Vertical");
        force *= Speed;
        GetComponent<Rigidbody>().AddRelativeForce(0, 0, force);

        /*if (Mathf.Abs(force) > 0)
            Debug.Log("force = " + force);*/


        float torque = Input.GetAxis("Horizontal");
        torque *= TurnSpeed;
        GetComponent<Rigidbody>().AddRelativeTorque(0, torque, 0);

        grounded = false;
    }

    void OnTriggerStay(Collider collider)
    {

        //Debug.Log("OnTriggerStay!");
        if (collider.transform == wheelBackLeft ||
            collider.transform == wheelBackRight)
        {
            grounded = true;
            //Debug.Log("grounded!");
        }
    }
}

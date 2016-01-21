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
    }

    void Update()
    {
        transform.position += -1* moveSpeed/10 * control.seek(pursuer.position)*Time.deltaTime;
    }
}

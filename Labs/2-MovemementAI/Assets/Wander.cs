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
        transform.position += moveSpeed/10 * (transform.position - control.wanderPosition())* Time.deltaTime;
    }
}

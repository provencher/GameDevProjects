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
        transform.position += moveSpeed/10 *control.seek(target.position)*Time.deltaTime;
    }
}

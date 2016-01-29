using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (SteeringBasics))]
public class DroneBehavior : MonoBehaviour {

    public Transform target;
    public float neighbourRadius = 2;
    public float cohesionWeight = 1.5f;
    public float separationWeight = 2f;
    public float velocityMatchWeight = 1f;

    private Rigidbody rb;

    private SteeringBasics steeringBasics;

    //Seperation Detection
    private float boundingRadius;
    public float sepMaxAcceleration = 25;
    public float maxSepDist = 1f;

    //Velocity Matching
    public float facingCosineVel = 90;
    private float facingCosineValVel;
    public float timeToTarget = 0.1f;
    public float maxAcceleration = 4f;  

    //Cohesion
    public float facingCosineCohesion = 120f;
    private float facingCosineValCohesion;

    public List<GameObject> targets;

    // Use this for initialization
    void Start()
    {
        steeringBasics = GetComponent<SteeringBasics>();  
        rb = GetComponent<Rigidbody>();

        targets = new List<GameObject>();


        boundingRadius = SteeringBasics.getBoundingRadius(transform);

        facingCosineValVel = Mathf.Cos(facingCosineVel * Mathf.Deg2Rad);
        facingCosineValCohesion = Mathf.Cos(facingCosineCohesion * Mathf.Deg2Rad);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNearDronesList();

        Vector3 accel = Vector3.zero;        
        accel += CalculateCohesionSteering(targets) * cohesionWeight;
        accel += CalculateSperationSteering(targets) * separationWeight;
        accel += CalculateVelocityMatchingSteering(targets) * velocityMatchWeight;

        if (target != null)
        {
            accel += steeringBasics.arrive(target.position);
        }

        steeringBasics.steer(accel);
        steeringBasics.lookAtDirection(accel);
    }

    void UpdateNearDronesList()
    {
        DroneBehavior[] drones = FindObjectsOfType<DroneBehavior>();
        targets = new List<GameObject>();

        foreach(var d in drones)
        {
            if(Vector3.Distance(d.gameObject.transform.position, transform.position) < neighbourRadius)
            {                
                targets.Add(d.gameObject);          
            }            
        }
    }

    public Vector3 CalculateSperationSteering(List<GameObject> targets)
    {
        Vector3 acceleration = Vector3.zero;

        foreach (GameObject t in targets)
        {
            /* Get the direction and distance from the target */
            Vector3 direction = transform.position - t.transform.position;
            float dist = direction.magnitude;

            if (dist < maxSepDist)
            {
                float targetRadius = SteeringBasics.getBoundingRadius(t.transform.transform);

                /* Calculate the separation strength (can be changed to use inverse square law rather than linear) */
                var strength = sepMaxAcceleration * (maxSepDist - dist) / (maxSepDist - boundingRadius - targetRadius);

                /* Added separation acceleration to the existing steering */
                direction.Normalize();
                acceleration += direction * strength;
            }
        }

        return acceleration;
    }

    public Vector3 CalculateVelocityMatchingSteering(List<GameObject> targets)
    {
        Vector3 accel = Vector3.zero;
        int count = 0;

        foreach (GameObject t in targets)
        {
            if (steeringBasics.isFacing(t.transform.position, facingCosineValVel))
            {
                /* Calculate the acceleration we want to match this target */
                Vector3 a = t.transform.GetComponent<Rigidbody>().velocity - rb.velocity;
                         
                a = a / timeToTarget;

                accel += a;

                count++;
            }
        }

        if (count > 0)
        {
            accel = accel / count;

            /* Make sure we are accelerating at max acceleration */
            if (accel.magnitude > maxAcceleration)
            {
                accel = accel.normalized * maxAcceleration;
            }
        }

        return accel;
    }


    public Vector3 CalculateCohesionSteering(List<GameObject> targets)
    {
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;

        /* Sums up everyone's position who is close enough and in front of the character */
        foreach (GameObject t in targets)
        {
            if (steeringBasics.isFacing(t.transform.position, facingCosineValCohesion))
            {
                centerOfMass += t.transform.position;
                count++;
            }
        }

        if (count == 0)
        {
            return Vector3.zero;
        }
        else
        {
            centerOfMass = centerOfMass / count;

            return steeringBasics.arrive(centerOfMass);
        }
    }
}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SteeringController : MonoBehaviour {

    public float maxVelocity = 3.5f;
    public float halfVelocity = 3.5f;
    public float regularVelocity = 3.5f;

    /* How far ahead the ray should extend */
    public float mainWhiskerLen = 0.2f;
    public float sideWhiskerLen = 0.2f;
    public float sideWhiskerAngle = 45f;
    /* The distance away from the collision that we wish go */
    public float wallAvoidDistance = 0.1f;
    

    /* The maximum acceleration */
    public float maxAcceleration = 5f;

    /* The radius from the target that means we are close enough and have arrived */
    public float targetRadius = 0.005f;

    /* The radius from the target where we start to slow down  */
    public float slowRadius = 1f;

    /* The time in which we want to achieve the targetSpeed */
    public float timeToTarget = 0.1f;

    public float turnSpeed = 20f;

    private Rigidbody2D rb;

    public bool smoothing = true;
    public int numSamplesForSmoothing = 5;
    private Queue<Vector2> velocitySamples = new Queue<Vector2>();

    //Sets to 1 to do velocity smoothin - 0 for acceleration-free movement
    public int notKinetic = 1;

    //Wander Variables
    public float wanderRadius = 1.2f;
    public float wanderDistance = 2f;

    //maximum amount of random displacement a second
    public float wanderJitter = 40f;
    private Vector3 wanderTarget;


    

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //stuff for the wander behavior
        float theta = Random.value * 2 * Mathf.PI;

        //create a vector to a target position on the wander circle
        wanderTarget = new Vector3(wanderRadius * Mathf.Cos(theta), wanderRadius * Mathf.Sin(theta), 0f);      
    }

    /* Updates the velocity of the current game object by the given linear acceleration */
    public void steer(Vector3 linearAcceleration)
    {
        rb.velocity += (Vector2)linearAcceleration * Time.deltaTime;

        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }

        //If Kinetic immidiately achieve max speed
        if(notKinetic == 0)
        {
            rb.velocity = linearAcceleration.normalized * maxVelocity;
            //rb.velocity = Vector3.zero;
        }
    }

    public Vector3 wanderPosition()
    {
        //get the jitter for this time frame
        float jitter = wanderJitter * Time.deltaTime;

        //add a small random vector to the target's position
        wanderTarget += new Vector3(Random.Range(-1f, 1f) * jitter, Random.Range(-1f, 1f) * jitter, 0f);

        //make the wanderTarget fit on the wander circle again
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        //move the target in front of the character
        Vector3 targetPosition = transform.position + transform.right * wanderDistance + wanderTarget;

        //Debug.DrawLine(transform.position, targetPosition);

        return targetPosition;
    }


    /* A seek steering behavior. Will return the steering for the current game object to seek a given position */
    public Vector3 seek(Vector3 targetPosition, float maxSeekAccel)
    {
        //Get the direction
        Vector3 acceleration = targetPosition - transform.position;

        //Remove the z coordinate
        acceleration.z = 0;

        acceleration.Normalize();

        //Accelerate to the target
        acceleration *= maxSeekAccel;  

        return acceleration;
    }

    public Vector3 seek(Vector3 targetPosition)
    {
        return seek(targetPosition, maxAcceleration);
    }

    public void lookAtDirection(Vector2 direction)
    {
        direction.Normalize();

        // If we have a non-zero direction then look towards that direciton otherwise do nothing
        if (direction.sqrMagnitude > 0.001f)
        {
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),Time.deltaTime*turnSpeed);

            
            float toRotation = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            float rotation = Mathf.LerpAngle(transform.rotation.eulerAngles.z, toRotation, Time.deltaTime * turnSpeed);

            transform.rotation = Quaternion.Euler(0, 0, rotation);            
        }
    }

    public void lookAtDirection(Quaternion toRotation)
    {
        lookAtDirection(toRotation.eulerAngles.z);
    }

    public void lookAtDirection(float toRotation)
    {
        float rotation = Mathf.LerpAngle(transform.rotation.eulerAngles.z, toRotation, Time.deltaTime * turnSpeed);

        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    /* Returns the steering for a character so it arrives at the target */
    public Vector3 arrive(Vector3 targetPosition)
    {
        /* Get the right direction for the linear acceleration */
        Vector3 targetVelocity = targetPosition - transform.position;

        // Remove the z coordinate
        targetVelocity.z = 0;

        /* Get the distance to the target */
        float dist = targetVelocity.magnitude;

        /* If we are within the stopping radius then stop */
        if (dist < targetRadius)
        {
            rb.velocity = Vector2.zero;
            return Vector2.zero;
        }

        /* Calculate the target speed, full speed at slowRadius distance and 0 speed at 0 distance */
        float targetSpeed;
        if (dist > slowRadius)
        {
            targetSpeed = maxVelocity;
        }
        else
        {
            targetSpeed = maxVelocity * (dist / slowRadius);
        }

        /* Give targetVelocity the correct speed */
        targetVelocity.Normalize();
        targetVelocity *= targetSpeed;

        /* Calculate the linear acceleration we want */
        Vector3 acceleration = targetVelocity - new Vector3(rb.velocity.x, rb.velocity.y, 0);
        /*
		 Rather than accelerate the character to the correct speed in 1 second, 
		 accelerate so we reach the desired speed in timeToTarget seconds 
		 (if we were to actually accelerate for the full timeToTarget seconds).
		*/
        acceleration *= 1 / timeToTarget;

        /* Make sure we are accelerating at max acceleration */
        if (acceleration.magnitude > maxAcceleration)
        {
            acceleration.Normalize();
            acceleration *= maxAcceleration;
        }

        return acceleration;
    }

    /* Makes the current game object look where he is going */
    public void lookWhereYoureGoing()
    {
        Vector2 direction = rb.velocity;

        if (smoothing)
        {
            if (velocitySamples.Count == numSamplesForSmoothing)
            {
                velocitySamples.Dequeue();
            }

            velocitySamples.Enqueue(rb.velocity);

            direction = Vector2.zero;

            foreach (Vector2 v in velocitySamples)
            {
                direction += v;
            }

            direction /= velocitySamples.Count;
        }

        lookAtDirection(direction);
    }


    /* Checks to see if the target is in front of the character */
    public bool isInFront(Vector3 target)
    {
        return isFacing(target, 0);
    }

    public bool isFacing(Vector3 target, float cosineValue)
    {
        Vector2 facing = transform.right.normalized;

        Vector2 directionToTarget = (target - transform.position);
        directionToTarget.Normalize();

        return Vector2.Dot(facing, directionToTarget) >= cosineValue;
    }

    public static float getBoundingRadius(Transform t)
    {
        SphereCollider col = t.GetComponent<SphereCollider>();
        return Mathf.Max(t.localScale.x, t.localScale.y, t.localScale.z) * col.radius;
    }

    /* Returns the orientation as a unit vector */
    private Vector3 orientationToVector(float orientation)
    {
        return new Vector3(Mathf.Cos(orientation), Mathf.Sin(orientation), 0);
    }

    //2D wallavoidance
    public Vector2 WallAvoidanceSteering()
    {
        Vector3 acceleration = Vector3.zero;

        /* Creates the ray direction vector */
        Vector3[] rayDirs = new Vector3[3];
        rayDirs[0] = rb.velocity.normalized;

        float orientation = Mathf.Atan2(rb.velocity.y, rb.velocity.x);

        rayDirs[1] = orientationToVector(orientation + sideWhiskerAngle * Mathf.Deg2Rad);
        rayDirs[2] = orientationToVector(orientation - sideWhiskerAngle * Mathf.Deg2Rad);

        RaycastHit2D hit;

        /* If no collision do nothing */
        if (!findObstacle(rayDirs, out hit))
        {
            return acceleration;
        }

        /* Create a target away from the wall to seek */
        Vector2 targetPostition = hit.point + hit.normal * wallAvoidDistance;

        /* If velocity and the collision normal are parallel then move the target a bit to
         the left or right of the normal */
        Vector3 cross = Vector3.Cross(rb.velocity, hit.normal);
        if (cross.magnitude < 0.005f)
        {
            targetPostition = targetPostition + new Vector2(-hit.normal.y, hit.normal.x);
        }

        return seek(targetPostition, maxAcceleration);        
    }

    private bool findObstacle(Vector3[] rayDirs, out RaycastHit2D firstHit)
    {
        firstHit = new RaycastHit2D();
        bool foundObs = false;

        for (int i = 0; i < rayDirs.Length; i++)
        {
            float rayDist = (i == 0) ? mainWhiskerLen : sideWhiskerLen;

            RaycastHit2D hit;

            if (hit = Physics2D.Raycast(transform.position, rayDirs[i], rayDist))
            {
                foundObs = true;
                firstHit = hit;
                break;
            }

            //Debug.DrawLine(transform.position, transform.position + rayDirs[i] * rayDist);
        }

        return foundObs;
    }

}

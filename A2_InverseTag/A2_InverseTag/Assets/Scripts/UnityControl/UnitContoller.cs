using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SteeringController))]
public class UnitContoller : MonoBehaviour
{   
    public bool seeker = false;  
    public bool frozen = false;
    public bool lastFrozen = false;
    bool hasHalted = false;
    bool newDirective = false;
    public bool stopGo;
    Vector3 acceleration;

    Transform target;
    List<Transform> seekers;
    Vector3 centerOfMass = Vector3.zero;
    Vector3 targetCoord;
    SteeringController control;
    Vector3 lastCalculatedTarget = Vector3.zero;

    Vector2[] path;
    int pathIndex = 0;
    Vector2 currentWaypoint = Vector2.zero;
    TagGameLogic game;

    float hazardDistance;

    // Use this for initialization
    void Start()
    {
        game = FindObjectOfType<TagGameLogic>();
        acceleration = Vector3.zero;
        targetCoord = Vector3.zero;
        seekers = new List<Transform>();
        control = GetComponent<SteeringController>();
        path = new Vector2[0];
        hazardDistance = 2 * transform.localScale.x * GetComponent<CircleCollider2D>().radius;
    }

    void FindTarget()
    {
        
        if (!target && gameObject.tag == "Seeker")
        {
            seeker = true;
            GetComponent<MeshRenderer>().material.color = Color.red;
            control.maxVelocity = 2;
            target = GameObject.FindGameObjectWithTag("Unit").transform;
        }
        else if (seekers.Count == 0 && gameObject.tag == "Unit")
        {
            seeker = false;
            GetComponent<MeshRenderer>().material.color = Color.yellow;
            GameObject[] units = GameObject.FindGameObjectsWithTag("Seeker");
            foreach(var u in units)
            {
                if(u.tag == "Seeker")
                {
                    seekers.Add(u.transform);
                }                    
            }
        }                
    }
    bool nearEnemies = false;
    Vector2 FindCenterOfMass()
    {
        var center = Vector3.zero;

        var surroundingEnemies = Physics2D.OverlapCircleAll(transform.position, hazardDistance*4);
        int proximityEnemies = 0;
        foreach (var s in surroundingEnemies)
        {
            if(s.gameObject.tag == "Seeker")
            {
                center += s.transform.position;
                proximityEnemies++;
            }
        }


        if(proximityEnemies > 0)
        {
            center /= proximityEnemies;
            nearEnemies = true;
        }
        else
        {
            nearEnemies = false;
            foreach (var s in seekers)
            {
                center += s.position;
            }
            center /= seekers.Count;
        }
         
        return center;
    }


    Vector2 FindDestination()
    {
        Vector2 destination = Vector2.zero;
        if (seeker)
        {
            Vector3 seek = target.transform.position;
            Vector3 seekDirection = (seek - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.forward * hazardDistance, seekDirection);
            if(hit && hit.collider.gameObject.tag == "Unit")
            {
                int sentry = 0;
                do
                {
                    destination = (Vector2)target.position + new Vector2(Random.Range(-hazardDistance, hazardDistance), Random.Range(-hazardDistance, hazardDistance));
                    sentry++;
                } while (!Pathfinding.instance.grid.NodeFromWorldPoint(destination).walkable && sentry < 5);

            }
            else
            {
                
                destination = seek;
                float distanceFromIT = 999;
                seekDirection *= hazardDistance;              
                bool pickedDestination = false;
                var destinations = new List<Vector3>();

                //CHeck if position exists in graph -> is walkable
                var landMarks = GameObject.FindGameObjectsWithTag("Landmark");
                foreach (var l in landMarks)
                {
                    if (Pathfinding.instance.grid.NodeFromWorldPoint(l.transform.position + seekDirection).walkable)
                    {
                        destinations.Add(l.transform.position + seekDirection);

                        if (Vector3.Distance((l.transform.position + seekDirection), seek) < distanceFromIT)                        
                        {
                            distanceFromIT = Vector3.Distance((l.transform.position + seekDirection), seek);
                            destination = (l.transform.position + seekDirection);
                            pickedDestination = true;
                        }
                    }
                }

                if (!pickedDestination && destinations.Count > 0)
                {
                    destination = destinations[Random.Range(0, destinations.Count)];
                }

            }           
        }
        else
        {                  

            Vector3 avoid = FindCenterOfMass();
            destination = avoid;
            float distanceFromMass = 0;
            Vector3 fleeDirection = (transform.position - avoid).normalized * hazardDistance;
            float distanceFromPlayer = 999;
            bool pickedDestination = false;
            var destinations = new List<Vector3>();

            //CHeck if position exists in graph -> is walkable
            var landMarks = GameObject.FindGameObjectsWithTag("Landmark");
            foreach (var l in landMarks)
            {
                if (Pathfinding.instance.grid.NodeFromWorldPoint(l.transform.position + fleeDirection).walkable)
                {
                    destinations.Add(l.transform.position + fleeDirection);

                    if (Vector3.Distance((l.transform.position + fleeDirection), avoid) > distanceFromMass &&
                    Vector3.Distance((l.transform.position + fleeDirection), transform.position) < distanceFromPlayer)
                    {
                        distanceFromMass = Vector3.Distance((l.transform.position + fleeDirection), avoid);
                        distanceFromPlayer = Vector3.Distance((l.transform.position + fleeDirection), transform.position);
                        destination = (l.transform.position + fleeDirection);
                        pickedDestination = true;
                    }                    
                }                
            } 
            
            if(!pickedDestination && destinations.Count > 0)
            {
                destination = destinations[Random.Range(0, destinations.Count)];
            }            
                     
        }
        return destination;
    }
 
    // Update is called once per frame
    void Update()
    {
        if(game.started)
        {
            
            Vector2 accleration = Vector2.zero;       

            FindTarget();

            if(gameObject.tag == "Unit" && nearEnemies)
            {
                acceleration = (-control.seek(FindCenterOfMass())*0.25f + control.seek(FindNextPosition(FindDestination()))*0.75f);
            }
            else
            {
                acceleration = control.seek(FindNextPosition(FindDestination()));
            }

            control.steer(acceleration);
            control.lookWhereYoureGoing();
        }       
    }    

    Vector2 FindNextPosition(Vector2 targetPos)
    {       
        
        if(path.Length <= 0 || (Vector3.Distance(lastCalculatedTarget, (Vector3)targetPos) > 2))
        {           
            path = Pathfinding.RequestPath(transform.position, targetPos);
            pathIndex = 0;
            if (path.Length > 0)
            {
                currentWaypoint = path[pathIndex];
            }
            else
            {
                currentWaypoint = transform.position;
            }
        }

        if (Vector3.Distance((Vector3)currentWaypoint, transform.position) < 0.05f)
        {
            pathIndex++;
            if (pathIndex >= path.Length)
            {
                path = Pathfinding.RequestPath(transform.position, lastCalculatedTarget);
                pathIndex = 0;
            }

            if (path.Length > 0)
            {
                currentWaypoint = path[pathIndex];
            }
            else
            {
                currentWaypoint = transform.position;
            }          
        }        
        return currentWaypoint;
    }



   

    void OnCollisionEnter2D(Collision2D coll)
    {   
        if(gameObject.tag == "Unit" && coll.gameObject.tag == "Seeker")
        {
            Application.LoadLevel(0);
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = pathIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;              

                if (i == pathIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}

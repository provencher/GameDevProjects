using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

[RequireComponent(typeof(SteeringController))]
public class UnitContoller : NetworkBehaviour
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

    bool initialized = false;

    public void initialize()
    {
        initialized = true;
        game = FindObjectOfType<TagGameLogic>();
        acceleration = Vector3.zero;
        targetCoord = Vector3.zero;
        seekers = new List<Transform>();
        control = GetComponent<SteeringController>();
        path = new Vector2[0];
        hazardDistance = 2 * transform.localScale.x * GetComponent<CircleCollider2D>().radius;
    }

    // Use this for initialization
    public override void OnStartLocalPlayer()
    {
        //initialize();
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
        int proximityEnemies = 0;
        
        var surroundingEnemies = Physics2D.OverlapCircleAll(transform.position, hazardDistance*4);
       
        foreach (var s in surroundingEnemies)
        {          
            if (s.gameObject.tag == "Seeker" && Physics2D.Raycast(transform.position, s.transform.position - transform.position))
            {               
                center += s.transform.position;
                proximityEnemies++; 
            }             
        }

    
        if (proximityEnemies > 0)
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
        var landMarks = GameObject.FindGameObjectsWithTag("Landmark");
        var destinations = new List<Vector3>();
        bool pickedDestination = false;
        Vector3 focus = Vector3.zero;

        if (seeker)
        {
            focus = target.transform.position;
            Vector3 seekDirection = (focus - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.forward * hazardDistance, seekDirection);

            if(hit && hit.collider.gameObject.tag == "Unit")
            {
                return hit.transform.position;                
            }
            else
            {   
                destination = focus;
                float distanceFromIT = 999;
                seekDirection *= hazardDistance;            

                foreach (var l in landMarks)
                {
                    if (Pathfinding.instance.grid.NodeFromWorldPoint(l.transform.position + seekDirection).walkable)
                    {
                        destinations.Add(l.transform.position + seekDirection);

                        if (Vector3.Distance((l.transform.position + seekDirection), focus) < distanceFromIT)                        
                        {
                            distanceFromIT = Vector3.Distance((l.transform.position + seekDirection), focus);
                            destination = (l.transform.position + seekDirection);
                            pickedDestination = true;
                        }
                    }
                }  
            }      
                 
        }
        else
        {

            focus = FindCenterOfMass();
            /*
            destination = focus;
            float distanceFromMass = 0;            
            Vector3 fleeDirection = (transform.position - focus).normalized * hazardDistance;                    

            foreach (var l in landMarks)
            {
                //Check if position exists in graph -> is walkable            
                if (Pathfinding.instance.grid.NodeFromWorldPoint(l.transform.position + fleeDirection).walkable)
                {
                    destinations.Add(l.transform.position + fleeDirection);

                    if (Vector3.Distance((l.transform.position + fleeDirection), focus) > distanceFromMass)
                    {
                        distanceFromMass = Vector3.Distance((l.transform.position + fleeDirection), focus);                        
                        destination = (l.transform.position + fleeDirection);
                        pickedDestination = true;                                            
                    }                    
                }                
            } 
            */                  
        }

        if (!pickedDestination || destinations.Count == 0)
        {
            //Shuffle landmarks
            for (int i = 0; i < landMarks.Length; i++)
            {
                var temp = landMarks[i];
                int randomIndex = Random.Range(i, destinations.Count);
                landMarks[i] = landMarks[randomIndex];
                landMarks[randomIndex] = temp;
            }


            int sentry = 0;
            bool fleeSeek = (gameObject.tag == "Unit" ? false : true);
            float distance = (fleeSeek ? 999 : 0);
            float bestDistance = distance;

            do
            {
                distance = Vector3.Distance(landMarks[sentry].transform.position, focus);
                if ((fleeSeek && bestDistance > distance)
                || (!fleeSeek && bestDistance < distance))
                {
                    //int modifier = Random.Range(-1, 1) < 0? -1: 1;
                    Vector2 direction = (focus - transform.position);
                    destination = (Vector2)landMarks[sentry].transform.position - direction;                   
                }                
            } while ( ++sentry < landMarks.Length);           
          
        }
        return destination;
    }
 
    // Update is called once per frame
    void Update()
    {
        if (initialized && game.started)
        {            
            Vector2 accleration = Vector2.zero;       

            FindTarget();
            if (nearEnemies && gameObject.tag == "Unit")
            {
                Vector3 centerOfMass = FindCenterOfMass();
                acceleration = control.arrive((Vector3)FindNextPosition(FindDestination()) - (centerOfMass - transform.position) * hazardDistance);
            }
            else
            {
                acceleration = control.arrive((Vector3)FindNextPosition(FindDestination()));

            }

            control.steer(acceleration);
            control.lookWhereYoureGoing();
        }       
    }

    float timeOfLastPath = 0;
    Vector2 lastTarget = Vector2.zero;
    Vector2 FindNextPosition(Vector2 targetPos)
    {       
        
        if(path.Length <= 0 || ((Vector3.Distance(lastTarget, targetPos) > hazardDistance) && (Time.time - timeOfLastPath) > 0.4f))
        {           
            path = Pathfinding.RequestPath(transform.position, targetPos);
            timeOfLastPath = Time.time;
            lastTarget = targetPos;
            pathIndex = 0;
            if (path.Length > 0)
            {
                currentWaypoint = path[pathIndex];
            }
            else
            {
                currentWaypoint = targetPos;
            }
        }

        if (Vector3.Distance((Vector3)currentWaypoint, transform.position) < hazardDistance)
        {
            pathIndex++;
            if (pathIndex >= path.Length)
            {
                path = Pathfinding.RequestPath(transform.position, targetPos);
                timeOfLastPath = Time.time;
                lastTarget = targetPos;
                pathIndex = 0;
            }

            if (path.Length > 0)
            {
                currentWaypoint = path[pathIndex];
            }
            else
            {
                currentWaypoint = targetPos;
            }          
        }        
        return currentWaypoint;
    }



   

    void OnCollisionEnter2D(Collision2D coll)
    {   
        if(gameObject.tag == "Unit" && coll.gameObject.tag == "Seeker")
        {
            //Application.LoadLevel(0);
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

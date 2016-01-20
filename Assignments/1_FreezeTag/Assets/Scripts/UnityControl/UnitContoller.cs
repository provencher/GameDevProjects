using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteeringController))]
public class UnitContoller : MonoBehaviour
{   
    public bool seeker = false;
    public bool frozen = false;
    public bool lastFrozen = false;

    Vector3 acceleration;    

    // Use this for initialization
    void Start()
    {
        acceleration = Vector3.zero;

    }
 
    // Update is called once per frame
    void Update()
    {       


        if (frozen)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<MeshRenderer>().material.color = new Color(0.0f, 0.0f, 1.0f);
        }
        else
        {
            if (seeker)
            {
                GetComponent<MeshRenderer>().material.color = new Color(1.0f, 0.0f, 0.0f);
                //Seek non-frozen characters
                acceleration = GetComponent<SteeringController>().seek(FindNearestUnit(false));

            }
            else
            {
                GetComponent<MeshRenderer>().material.color = new Color(0.0f, 1.0f, 0.0f);

                
                //Flee and Seek frozen characters
                Vector3 seekerPosition = GameObject.FindGameObjectWithTag("Seeker").transform.position;

                //Check within flee range
                if(Vector3.Distance(transform.position, seekerPosition) < 3)
                {
                    acceleration = (-0.3f) * GetComponent<SteeringController>().seek(seekerPosition);
                }
                else
                {        
                    acceleration = (0.3f) * GetComponent<SteeringController>().seek(FindNearestUnit(true));
                }
                //acceleration = (-0.75f) *GetComponent<SteeringController>().seek(seekerPosition) + (0.25f)* GetComponent<SteeringController>().seek(FindNearestUnit(true));
            }
                        
            GetComponent<SteeringController>().steer(acceleration);
            GetComponent<SteeringController>().lookWhereYoureGoing();
        }
    }

    //If !Seeking then wander + flee if close
    //else seek nearest opponent

    //Find closest unit
    Vector3 FindNearestUnit(bool findFrozen)
    {
        UnitContoller[] units = FindObjectsOfType<UnitContoller>();

        Vector3 nearestPosition = new Vector3(999, 999, 999);
        foreach (var unit in units)
        {
            if ((unit.transform != this.transform) 
                && ((!findFrozen && !unit.frozen) || (findFrozen && unit.frozen))
                && (Vector3.Distance(unit.gameObject.transform.position, transform.position) < Vector3.Distance(nearestPosition, transform.position)))
            {
                if(findFrozen && Vector3.Distance(transform.position, unit.transform.position) < 4 || !findFrozen)
                {
                    nearestPosition = unit.gameObject.transform.position;
                }                
            }
        }

        //If no target found, wander
        if (nearestPosition.x == 999)
        {
            nearestPosition = GetComponent<SteeringController>().wanderPosition();
        }

        return nearestPosition;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Unit" || collision.gameObject.tag == "Seeker")
        {
            if (collision.gameObject.GetComponent<UnitContoller>().seeker && !seeker && !frozen)
            {
                GameObject.FindObjectOfType<TagGameLogic>().frozenPlayers++;
                frozen = true;

                if (GameObject.FindObjectOfType<TagGameLogic>().frozenPlayers == GameObject.FindObjectOfType<TagGameLogic>().numberOfPlayers - 1)
                {
                    lastFrozen = true;
                }              
                
            }

            if (collision.gameObject.GetComponent<UnitContoller>().frozen && !seeker)
            {
                GameObject.FindObjectOfType<TagGameLogic>().frozenPlayers--;
                collision.gameObject.GetComponent<UnitContoller>().lastFrozen = false;
                collision.gameObject.GetComponent<UnitContoller>().frozen = false;
            }
        }   
    }
}

using UnityEngine;
using System.Collections;

public class ScreenEdge : MonoBehaviour
{

    public Vector3 bottomLeft;
    public Vector3 topRight;
    public Vector3 widthHeight;

    // Use this for initialization
    void Start()
    {
        float z = -1 * Camera.main.transform.position.z;

        bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, z));
        topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, z));
        widthHeight = topRight - bottomLeft;

        transform.localScale = new Vector3(widthHeight.x, widthHeight.y, transform.localScale.z);

        //bottomLeft *= 0.89f;
        //topRight *= 0.89f;
        //widthHeight = topRight - bottomLeft;
    }

    void FixedUpdate()
    {
        foreach(var unit in FindObjectsOfType<DroneBehavior>())
        {
            if (unit.gameObject.tag == "Seeker" || unit.gameObject.tag == "Unit")
            {
                Transform t = unit.transform;
                SphereCollider s = unit.gameObject.GetComponent<SphereCollider>();


                if ((t.position.x - s.radius * t.localScale.x) < bottomLeft.x)
                {
                    t.position = new Vector3(t.position.x + widthHeight.x - s.radius * t.localScale.x, t.position.y, t.position.z);
                }

                if ((t.position.x + s.radius * t.localScale.x) > topRight.x)
                {
                    t.position = new Vector3(t.position.x - widthHeight.x + s.radius * t.localScale.x, t.position.y, t.position.z);
                }

                if ((t.position.y - s.radius * t.localScale.y) < bottomLeft.y)
                {
                    t.position = new Vector3(t.position.x, t.position.y + widthHeight.y - s.radius * t.localScale.y, t.position.z);
                }

                if ((t.position.y + s.radius * t.localScale.y) > topRight.y)
                {
                    t.position = new Vector3(t.position.x, t.position.y - widthHeight.y + s.radius * t.localScale.y, t.position.z);
                }
            }
        }
    }
}

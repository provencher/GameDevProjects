using UnityEngine;
using System.Collections;

public class GrabberController : MonoBehaviour {
	public Vector3 dragVector;
	public GameObject cursorObject;
	public Vector3 cursorPosition;
	public Vector3 prevCursorPosition;
    public Vector3 clickPosition;

	private static GrabberController instance;

	// Use this for initialization
	void Start () {
	
	}	
	// Update is called once per frame
	void Update () {
		// track cursor position
		prevCursorPosition = cursorPosition;
		cursorPosition = GetMousePositionXYero();

		// reset drag vector
		//dragVector = Vector3.zero;

		if (Input.GetMouseButtonDown(0))
		{
			// attempt to find cursor object
			cursorObject = GetCursorObject();
            clickPosition = GetMousePositionXYero();
        }
		// listen for mouse down events
		else if (Input.GetMouseButton(0))
		{
			// holding
			dragVector = cursorPosition - clickPosition;
		}                

        if(dragVector.magnitude > 0.01f)
        {
            dragVector -= dragVector * Time.deltaTime;
        }
        else
        {
            dragVector = Vector3.zero;
        }
	}

	/// <summary>
	/// Singleton instance
	/// </summary>
	public static GrabberController Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject container = new GameObject();
				container.name = "GrabberController";
				instance = container.AddComponent<GrabberController>();			
			}
			
			return instance;
		}
	}

	private GameObject GetCursorObject()
	{
		// get mouse position data
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		// get the object (if any) immediately under the cursor
		RaycastHit hit;
		Physics.Raycast(ray, out hit);
		
		return hit.collider != null ? hit.collider.gameObject : null;
	}

	public static Vector3 GetMousePositionXYero()
	{
		if (Camera.main != null)
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(
																		Input.mousePosition.x,
																		Input.mousePosition.y, 
																		Camera.main.transform.position.y
																	));
			return new Vector3(pos.x, pos.y, 0);
		}
		else {
			return Vector3.zero;
		}
	}
}

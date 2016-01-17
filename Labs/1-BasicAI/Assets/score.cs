using UnityEngine;
using System.Collections;

public class score : MonoBehaviour 
{
	
	static public string score1 = "This is a test";
	
	
	void OnGUI() 
	{   
		// Make a background box
		//GUI.Box(new Rect(10,10,100,90), "Score Menu");
		GUI.Label(new Rect(10, 10, 100, 20),score1);   

	}
	
	
	
}
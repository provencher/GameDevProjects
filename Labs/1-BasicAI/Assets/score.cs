using UnityEngine;
using System.Collections;

public class score : MonoBehaviour 
{
    public int value = 0;
    public string score1 = "Score: ";
	
    void Update()
    {
        score1 = "Score: " +value.ToString();
    }
	
	void OnGUI() 
	{   
		// Make a background box
		//GUI.Box(new Rect(10,10,100,90), "Score Menu");
		GUI.Label(new Rect(10, 10, 100, 20),score1);   
	}
	
	
	
}
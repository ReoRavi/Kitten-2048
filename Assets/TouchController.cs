using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour {

    public 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("ee");

        if (Input.touchCount > 0)
        {
            Debug.Log(Input.touchCount);
        }
	}

    private void OnMouseDown()
    {
        
    }
}

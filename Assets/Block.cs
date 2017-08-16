using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour {

    // Block Number
    public int number;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Create(int number)
    {
        this.number = number;
        GetComponent<SpriteRenderer>().color = new Color(40 * ((number / 2) - 1), 0, 0);
    }
}

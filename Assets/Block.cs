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

    public void Create(int number, Sprite sprite)
    {
        this.number = number;

        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void SetImage(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}

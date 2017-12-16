using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

    private Transform currentRoom;

    void Awake ()
    {
        currentRoom = this.transform.parent;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Transform GetCurrentRoom()
    {
        return currentRoom;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusOnRoom : MonoBehaviour {
    
    private Transform mRoomToFocus;

	// Use this for initialization
	void Start () {
        mRoomToFocus = GameObject.FindWithTag("Player").GetComponent<PlayerBehaviour>().GetCurrentRoom();
        ChangeRoom();
    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log(mRoomToFocus);
    }

    void ChangeRoom()
    {
        this.transform.position = new Vector3(mRoomToFocus.position.x, 15, mRoomToFocus.position.z);
    }
}

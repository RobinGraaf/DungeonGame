using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private GameObject roomPrefab, playerPrefab, spawnPointPrefab;
    private GameObject spawnPoint;

    private List<GameObject> rooms;

	// Use this for initialization
	void Start () {
        rooms = new List<GameObject>();

        CreateNewRoom(0, 0, 0, 1, true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void CreateNewRoom(float x, float y, float z, int amountOfRooms, bool spawnroom)
    {
        for (int i = 0; i < amountOfRooms; i++) {
            GameObject room = Instantiate(roomPrefab, new Vector3(x, y, z), Quaternion.identity);
            rooms.Add(room);

            if (spawnroom)
            {
                spawnPoint = Instantiate(spawnPointPrefab, Vector3.zero, Quaternion.identity, room.transform);
            }
        }
        CreatePlayer();
    }

    void CreatePlayer()
    {
        Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform.parent);
    }

    public List<GameObject> GetRooms()
    {
        return rooms;
    }
}

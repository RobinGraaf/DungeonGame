using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusOnRoom : MonoBehaviour {

    public void ChangeRoom(TDMap.Room room, float tileSize)
    {
        this.transform.position = new Vector3((room.center.x + 0.5f) * tileSize, 25, (room.center.y + 0.5f) * tileSize);
    }
}

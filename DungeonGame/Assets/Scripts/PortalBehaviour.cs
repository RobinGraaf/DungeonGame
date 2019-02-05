using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortalBehaviour : MonoBehaviour
{

    TDMap.Room connectedToRoom;
    int connectedToPortal;

    Vector3 teleportPosition;
    float tileSize;

    enum PORTAL { TOP, RIGHT, BOTTOM, LEFT }

    // Use this for initialization
    void Start()
    {
        tileSize = GameManager.GetTileSize();

        switch (connectedToPortal)
        {
            case (int)PORTAL.TOP:
                teleportPosition = new Vector3((connectedToRoom.portalPositionTop.x + 0.5f) * tileSize, 0.5f * tileSize, (connectedToRoom.portalPositionTop.y + 0.5f) * tileSize - 3.0f);
                break;

            case (int)PORTAL.BOTTOM:
                teleportPosition = new Vector3((connectedToRoom.portalPositionBottom.x + 0.5f) * tileSize, 0.5f * tileSize, (connectedToRoom.portalPositionBottom.y + 0.5f) * tileSize + 3.0f);
                break;

            case (int)PORTAL.LEFT:
                teleportPosition = new Vector3((connectedToRoom.portalPositionLeft.x + 0.5f) * tileSize + 3.0f, 0.5f * tileSize, (connectedToRoom.portalPositionLeft.y + 0.5f) * tileSize);
                break;

            case (int)PORTAL.RIGHT:
                teleportPosition = new Vector3((connectedToRoom.portalPositionRight.x + 0.5f) * tileSize - 3.0f, 0.5f * tileSize, (connectedToRoom.portalPositionRight.y + 0.5f) * tileSize);
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isActiveAndEnabled)
        {
            other.transform.position = teleportPosition;
            Camera.main.GetComponent<FocusOnRoom>().ChangeRoom(connectedToRoom, tileSize);
        }
    }

    public void SetRoomConnection(TDMap.Room room)
    {
        connectedToRoom = room;
    }

    public void SetPortalConnection(int portal)
    {
        connectedToPortal = portal;
    }

    public void PrintTarget()
    {
        Debug.Log("Room: " + connectedToRoom.index + "\t | Portal: " + connectedToPortal);
    }
}

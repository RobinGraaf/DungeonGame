using UnityEngine;
using System.Collections.Generic;

public class TDMap
{
    int index;
    const int AMOUNT_OF_ROOMS = 20;
    int sizeX, sizeY;
    int[,] mapData;
    List<Room> rooms;

    public int minConnections;
    public int maxConnections;

    bool noCollisions;

    enum TYPE { UNKNOWN, FLOOR, WALL, STONE, PORTAL }

    // Room class
    public class Room
    {
        public int index;
        // Position related
        public int left, bottom, width, height;
        public int right { get { return left + (width + 1); } }
        public int top { get { return bottom + (height - 1); } }
        public Vector2 center { get { return new Vector2(left + (width / 2), bottom + (height / 2)); } }

        // Collision related
        public bool isColliding;
        public bool CollidesWith(Room other)
        {
            if (left > other.right + 1) return false;
            if (right < other.left - 1) return false;
            if (top < other.bottom - 1) return false;
            if (bottom > other.top + 1) return false;

            return true;
        }

        // Connection related
        public int maxRoomConnections;
        public int connections = 0;
        public List<Room> connectedRooms = new List<Room>();
        public enum PORTAL { TOP, RIGHT, BOTTOM, LEFT }
        public List<Portal> portals = new List<Portal>();

        public class Portal
        {
            bool isActive = true;
            public int connectedWithPortal;
            public Room connectedWithRoom = null;
            public bool isConnected = false;
        }

        public Vector2 portalPositionTop { get { return new Vector2(center.x, top - 1); } }
        public Vector2 portalPositionBottom { get { return new Vector2(center.x, bottom + 1); } }
        public Vector2 portalPositionLeft { get { return new Vector2(left + 1, center.y); } }
        public Vector2 portalPositionRight { get { return new Vector2(right - 3, center.y); } }

    }

    // Map constructor
    public TDMap(int width, int height)
    {
        index = 1;
        // Initialize variables
        sizeX = width;
        sizeY = height;

        minConnections = 1;
        maxConnections = 5;

        rooms = new List<Room>();

        // Initialize rooms and save them in the list
        while (rooms.Count < AMOUNT_OF_ROOMS)
        {
            Room currentRoom = new Room();
            int roomSizeX = Random.Range(7, 18);
            int roomSizeY = Random.Range(7, 18);
            currentRoom.left = Random.Range(((sizeX - roomSizeX) / 2) - roomSizeX, ((sizeX - roomSizeX) / 2) + roomSizeX * 2);
            currentRoom.bottom = Random.Range(((sizeY - roomSizeY) / 2) - roomSizeY, ((sizeY - roomSizeY) / 2) + roomSizeY * 2);
            currentRoom.width = roomSizeX;
            currentRoom.height = roomSizeY;

            for (int i = 0; i < 4; i++)
            {
                currentRoom.portals.Add(new Room.Portal());
            }

            if (!IsRoomColliding(currentRoom))
            {
                currentRoom.isColliding = false;
                rooms.Add(currentRoom);
            }
            else if (IsRoomColliding(currentRoom))
            {

                int failsAllowed = 5;

                currentRoom.isColliding = true;
                do
                {
                    ResolveCollisions(currentRoom);
                    if (!IsRoomColliding(currentRoom))
                    {
                        currentRoom.isColliding = false;
                        currentRoom.index = index++;
                        rooms.Add(currentRoom);
                    }
                    else
                    {
                        failsAllowed--;
                        if (failsAllowed <= 0)
                        {
                            break;
                        }
                    }
                } while (currentRoom.isColliding);
            }
        }

        int smallestX = sizeX, smallestY = sizeY;

        foreach (Room room in rooms)
        {
            if (room.left < smallestX)
                smallestX = room.left;
            if (room.bottom < smallestY)
                smallestY = room.bottom;
        }

        foreach (Room room in rooms)
        {
            room.left -= (smallestX - 1);
            room.bottom -= (smallestY - 1);
        }

        int biggestX = 0, biggestY = 0;

        foreach (Room room in rooms)
        {
            if (room.right > biggestX)
                biggestX = room.right + 1;
            if (room.top > biggestY)
                biggestY = room.top + 1;
        }

        sizeX = biggestX + 1;
        sizeY = biggestY + 1;

        mapData = new int[sizeX, sizeY];

        // Set all tiles to black
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                mapData[x, y] = (int)TYPE.UNKNOWN;
            }
        }

        int soloConnectionRooms = 0, hubRooms = 0;
        int maxSoloRooms = (int)Mathf.Ceil(AMOUNT_OF_ROOMS / 7), maxHubRooms = (int)Mathf.Ceil(AMOUNT_OF_ROOMS / 7);
        // Visually create the rooms
        foreach (Room room in rooms)
        {
            room.maxRoomConnections = Random.Range(minConnections, maxConnections);
            if (room.maxRoomConnections == 1)
            {
                soloConnectionRooms++;
                if (soloConnectionRooms >= maxSoloRooms)
                    minConnections += 1;
            }

            if (room.maxRoomConnections == 5)
            {
                hubRooms++;
                if (hubRooms >= maxHubRooms)
                    maxConnections -= 1;
            }
            MakeRoom(room);
        }

        foreach (Room room in rooms)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                MakePortal(room, rooms[i]);
            }
        }
    }

    void ResolveCollisions(Room room)
    {
        foreach (Room room2 in rooms)
        {
            switch (Random.Range(0, 4))
            {
                case 0: // Top left quadrant
                    room.left -= 1;
                    room.bottom += 1;
                    break;
                case 1: // Top right quadrant
                    room.left += 1;
                    room.bottom += 1;
                    break;
                case 2: // Bottom left quadrant
                    room.left -= 1;
                    room.bottom -= 1;
                    break;
                case 3: // Bottom right quadrant
                    room.left += 1;
                    room.bottom -= 1;
                    break;
                default:
                    room.left += 1;
                    break;
            }
        }
    }

    bool IsRoomColliding(Room room)
    {
        foreach (Room room2 in rooms)
        {
            if (room.CollidesWith(room2))
            {
                return true;
            }
        }
        return false;
    }

    public int GetTileAt(int x, int y)
    {
        return mapData[x, y];
    }

    public int GetSizeX()
    {
        return sizeX;
    }

    public int GetSizeY()
    {
        return sizeY;
    }

    public List<Room> GetRooms()
    {
        return rooms;
    }

    public Room GetRoom(int index)
    {
        return rooms[index];
    }

    void MakeRoom(Room room)
    {
        for (int x = 0; x < room.width; x++)
        {
            for (int y = 0; y < room.height; y++)
            {
                if (x == 0 || x == room.width - 1 || y == 0 || y == room.height - 1)
                {
                    mapData[room.left + x, room.bottom + y] = (int)TYPE.WALL;
                }
                else
                {
                    mapData[room.left + x, room.bottom + y] = (int)TYPE.FLOOR;
                }
            }
        }
    }

    void MakePortal(Room r1, Room r2)
    {
        if (r1.connections < r1.maxRoomConnections && r2.connections < r2.maxRoomConnections)
        {
            // Beneath
            if (r1.top < r2.bottom && !r1.portals[(int)Room.PORTAL.TOP].isConnected && !r2.portals[(int)Room.PORTAL.BOTTOM].isConnected)
            {
                if ((r1.left > r2.left && r1.left < r2.right) || (r1.right < r2.right && r1.right > r2.left))
                {
                    mapData[(int)r1.portalPositionTop.x, (int)r1.portalPositionTop.y] = (int)TYPE.PORTAL;
                    mapData[(int)r2.portalPositionBottom.x, (int)r2.portalPositionBottom.y] = (int)TYPE.PORTAL;

                    ConnectPortals(r1, r2, (int)Room.PORTAL.TOP, (int)Room.PORTAL.BOTTOM);
                }
            }

            // Above
            if (r1.bottom > r2.top && !r1.portals[(int)Room.PORTAL.BOTTOM].isConnected && !r2.portals[(int)Room.PORTAL.TOP].isConnected)
            {
                if ((r1.left > r2.left && r1.left < r2.right) || (r1.right < r2.right && r1.right > r2.left))
                {
                    mapData[(int)r1.portalPositionBottom.x, (int)r1.portalPositionBottom.y] = (int)TYPE.PORTAL;
                    mapData[(int)r2.portalPositionTop.x, (int)r2.portalPositionTop.y] = (int)TYPE.PORTAL;

                    ConnectPortals(r1, r2, (int)Room.PORTAL.BOTTOM, (int)Room.PORTAL.TOP);
                }
            }

            // Left
            if (r1.right < r2.left && !r1.portals[(int)Room.PORTAL.RIGHT].isConnected && !r2.portals[(int)Room.PORTAL.LEFT].isConnected)
            {
                if ((r1.top < r2.top && r1.top > r2.bottom) || (r1.bottom > r2.bottom && r1.bottom < r2.top))
                {
                    mapData[(int)r1.portalPositionRight.x, (int)r1.portalPositionRight.y] = (int)TYPE.PORTAL;
                    mapData[(int)r2.portalPositionLeft.x, (int)r2.portalPositionLeft.y] = (int)TYPE.PORTAL;

                    ConnectPortals(r1, r2, (int)Room.PORTAL.RIGHT, (int)Room.PORTAL.LEFT);
                }
            }

            // Right
            if (r1.left > r2.right && !r1.portals[(int)Room.PORTAL.LEFT].isConnected && !r2.portals[(int)Room.PORTAL.RIGHT].isConnected)
            {
                if ((r1.top < r2.top && r1.top > r2.bottom) || (r1.bottom > r2.bottom && r1.bottom < r2.top))
                {
                    mapData[(int)r1.portalPositionLeft.x, (int)r1.portalPositionLeft.y] = (int)TYPE.PORTAL;
                    mapData[(int)r2.portalPositionRight.x, (int)r2.portalPositionRight.y] = (int)TYPE.PORTAL;

                    ConnectPortals(r1, r2, (int)Room.PORTAL.LEFT, (int)Room.PORTAL.RIGHT);
                }
            }
        }
    }

    void ConnectPortals(Room r1, Room r2, int portal1, int portal2)
    {
        r1.connections++;
        r2.connections++;

        r1.connectedRooms.Add(r2);
        r2.connectedRooms.Add(r1);

        r1.portals[portal1].isConnected = true;
        r1.portals[portal1].connectedWithRoom = r2;
        r1.portals[portal1].connectedWithPortal = portal2;

        r2.portals[portal2].isConnected = true;
        r2.portals[portal2].connectedWithRoom = r1;
        r2.portals[portal2].connectedWithPortal = portal1;
    }
}

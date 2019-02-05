using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TGMap : MonoBehaviour
{
    int sizeX = 500;
    int sizeZ = 500;
    [SerializeField] float tileSize = 1.0f;

    [SerializeField] Texture2D terrainTiles;
    [SerializeField] int tileResolution;

    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject portalPrefab;
    [SerializeField] GameObject playerSpawnPoint;

    List<GameObject> levelObjects = new List<GameObject>();

    TDMap map;
    public enum PORTAL { TOP, RIGHT, BOTTOM, LEFT }

    private float wallCenterX, wallCenterY;

    // Use this for initialization
    void Start()
    {
        BuildMesh();
    }

    public void BuildMesh()
    {
        map = new TDMap(sizeX, sizeZ);
        sizeX = map.GetSizeX();
        sizeZ = map.GetSizeY();

        int numTiles = sizeX * sizeZ;
        int numTriangles = numTiles * 2;

        int vSizeX = sizeX + 1;
        int vSizeZ = sizeZ + 1;
        int numVerts = vSizeX * vSizeZ;

        Vector3[] vertices = new Vector3[numVerts];
        Vector3[] normals = new Vector3[numVerts];
        Vector2[] uv = new Vector2[numVerts];

        int[] triangles = new int[numTriangles * 3];

        int x, z;
        for (z = 0; z < vSizeZ; z++)
        {
            for (x = 0; x < vSizeX; x++)
            {
                vertices[z * vSizeX + x] = new Vector3(x * tileSize, 0, z * tileSize);
                normals[z * vSizeX + x] = Vector3.up;
                uv[z * vSizeX + x] = new Vector2((float)x / sizeX, (float)z / sizeZ);
            }
        }

        for (z = 0; z < sizeZ; z++)
        {
            for (x = 0; x < sizeX; x++)
            {
                int squareIndex = z * sizeX + x;
                int triangleOffset = squareIndex * 6;

                triangles[triangleOffset + 0] = z * vSizeX + x + 0;
                triangles[triangleOffset + 1] = z * vSizeX + x + vSizeX + 0;
                triangles[triangleOffset + 2] = z * vSizeX + x + vSizeX + 1;

                triangles[triangleOffset + 3] = z * vSizeX + x + 0;
                triangles[triangleOffset + 4] = z * vSizeX + x + vSizeX + 1;
                triangles[triangleOffset + 5] = z * vSizeX + x + 1;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        MeshFilter filter = GetComponent<MeshFilter>();
        MeshCollider collider = GetComponent<MeshCollider>();

        filter.mesh = mesh;
        collider.sharedMesh = mesh;

        BuildTexture();
    }

    public void BuildTexture()
    {
        int texWidth = sizeX * tileResolution;
        int texHeight = sizeZ * tileResolution;
        Texture2D texture = new Texture2D(texWidth, texHeight);

        Color[][] tiles = ChopUpTiles();

        for (int y = 0; y < sizeZ; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                Color[] color = tiles[map.GetTileAt(x, y)];
                texture.SetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution, color);
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.sharedMaterials[0].mainTexture = texture;

        BuildModels();
    }

    public Color[][] ChopUpTiles()
    {
        int numTilesPerRow = terrainTiles.width / tileResolution;
        int numRows = terrainTiles.height / tileResolution;

        Color[][] tiles = new Color[numTilesPerRow * numRows][];

        for (int y = 0; y < numRows; y++)
        {
            for (int x = 0; x < numTilesPerRow; x++)
            {
                tiles[y * numTilesPerRow + x] = terrainTiles.GetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution);
            }
        }

        return tiles;

    }

    public void BuildModels()
    {
        foreach (GameObject obj in levelObjects)
        {
            DestroyImmediate(obj);
        }
        levelObjects.Clear();

        int roomNumber = 1;

        foreach (var room in map.GetRooms())
        {
            BuildWalls(room, roomNumber);
            BuildPortals(room, roomNumber);
            roomNumber++;
        }

        StartGame();
    }

    void StartGame()
    {
        // Spawn the player on the selected starting point
        TDMap.Room startRoom = map.GetRoom(Random.Range(0, map.GetRooms().Count));
        levelObjects.Add(Instantiate(playerSpawnPoint, new Vector3((startRoom.center.x + 0.5f) * tileSize, (0.5f * tileSize) + 3.0f, (startRoom.center.y + 0.5f) * tileSize), Quaternion.identity));
        GameManager.CreatePlayer(tileSize);
    }

    void BuildWalls(TDMap.Room room, int roomNumber)
    {

        wallCenterX = room.width % 2 == 0 ? room.center.x * tileSize : (room.center.x + 0.5f) * tileSize;
        wallCenterY = room.height % 2 == 0 ? room.center.y * tileSize : (room.center.y + 0.5f) * tileSize;

        // Horizontal walls
        CreateHorizontalWall(room, room.top + 0.5f, roomNumber);
        CreateHorizontalWall(room, room.bottom + 0.5f, roomNumber);

        // Vertical walls
        CreateVerticalWall(room, room.left + 0.5f, roomNumber);
        CreateVerticalWall(room, room.right - 1.5f, roomNumber);
    }

    private void CreateHorizontalWall(TDMap.Room room, float roomLocation, int roomNumber)
    {
        GameObject newWall = Instantiate(wallPrefab, new Vector3(wallCenterX, 1.5f * tileSize, (roomLocation) * tileSize), Quaternion.identity, this.transform);
        newWall.transform.localScale = new Vector3(room.width * tileSize, tileSize * tileSize, tileSize);
        newWall.name = "Room " + roomNumber.ToString() + "\t | Horizontal wall";
        levelObjects.Add(newWall);
    }

    private void CreateVerticalWall(TDMap.Room room, float roomLocation, int roomNumber)
    {
        GameObject newWall = Instantiate(wallPrefab, new Vector3((roomLocation) * tileSize, 1.5f * tileSize, wallCenterY), Quaternion.identity, this.transform);
        newWall.transform.localScale = new Vector3(tileSize, tileSize * tileSize, room.height * tileSize);
        newWall.name = "Room " + roomNumber.ToString() + "\t | Vertical wall";
        levelObjects.Add(newWall);
    }

    void BuildPortals(TDMap.Room room, int roomNumber)
    {
        if (room.portals[(int)PORTAL.TOP].isConnected)
        {
            CreateNewPortal(room.portals[(int)PORTAL.TOP], room.portalPositionTop, roomNumber);
        }

        if (room.portals[(int)PORTAL.BOTTOM].isConnected)
        {
            CreateNewPortal(room.portals[(int)PORTAL.BOTTOM], room.portalPositionBottom, roomNumber);
        }

        if (room.portals[(int)PORTAL.LEFT].isConnected)
        {
            CreateNewPortal(room.portals[(int)PORTAL.LEFT], room.portalPositionLeft, roomNumber);
        }

        if (room.portals[(int)PORTAL.RIGHT].isConnected)
        {
            CreateNewPortal(room.portals[(int)PORTAL.RIGHT], room.portalPositionRight, roomNumber);
        }
    }

    private void CreateNewPortal(TDMap.Room.Portal portal, Vector2 portalPosition, int roomNumber)
    {
        GameObject newPortal = Instantiate(
                portalPrefab,
                new Vector3((portalPosition.x + 0.5f) * tileSize, 0.5f * tileSize, (portalPosition.y + 0.5f) * tileSize),
                Quaternion.identity, this.transform);
        newPortal.GetComponent<BoxCollider>().size *= tileSize;
        newPortal.GetComponent<PortalBehaviour>().SetRoomConnection(portal.connectedWithRoom);
        newPortal.GetComponent<PortalBehaviour>().SetPortalConnection(portal.connectedWithPortal);
        newPortal.name = "Room " + roomNumber.ToString() + "\t | Portal";
        levelObjects.Add(newPortal);
    }
}

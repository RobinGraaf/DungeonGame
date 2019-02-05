using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    GameObject playerPrefab;

    static GameObject s_playerPrefab;
    static float tileSize;

    // Use this for initialization
    void Awake()
    {
        SetPlayer(playerPrefab);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void CreatePlayer(float tSize)
    {
        SetTileSize(tSize);
        GameObject player = Instantiate(s_playerPrefab, GameObject.FindWithTag("PlayerSpawnPoint").transform.position, Quaternion.identity);
        Vector3 currentScale = player.transform.localScale;
        player.transform.localScale = new Vector3(currentScale.x * tileSize, currentScale.y * tileSize, currentScale.z * tileSize);
    }

    public static float GetTileSize()
    {
        return tileSize;
    }

    private static void SetTileSize(float value)
    {
        tileSize = value;
    }

    private static void SetPlayer(GameObject prefab)
    {
        s_playerPrefab = prefab;
    }
}

using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class Line : NetworkBehaviour
{
    public enum LineType
    {
        Vinland,
        Japan,
        Kazakhstan,
        ChoGalia
    }
    public LineType lineType;

    bool canSpawn = true;

    public GameObject tilemap;
    public GameObject[] spawnPlaces;
    public BuildPlace buildPlaceOrange;
    public BuildPlace buildPlaceBrown;
    public LineObjective lineObjectivePrefab;

    public PlayerManager playerManager;
    public PlayerManager enemyManager;
    public LineObjective lineObjective;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tilemap.SetActive(true);
        foreach(GameObject go in spawnPlaces)
        {
            go.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsSpawned&&IsHost && canSpawn)
        {
            SpawnBuildPlacesServerRpc();
        }
    }
    [ServerRpc]void SpawnBuildPlacesServerRpc()
    {
        SpawnBuildPlacesClientRpc();
    }
    [ClientRpc]void SpawnBuildPlacesClientRpc()
    {
        if(IsHost)
        {
            SpawnBuildPlaces();
        }
    }
    void SpawnBuildPlaces()
    {
        canSpawn = false;
        for(byte i =0 ;i < spawnPlaces.Length ;i++)
        {
            Entity entityForSpawn = null;
            switch(spawnPlaces[i].tag)
            {
                case "Brown":
                    if(IsHost)
                    {
                        entityForSpawn = Instantiate(buildPlaceBrown, spawnPlaces[i].transform.position, spawnPlaces[i].transform.rotation);
                        entityForSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(enemyManager.OwnerClientId);
                    }
                    break;
                case "Orange":
                    if (IsHost)
                    {
                        entityForSpawn = Instantiate(buildPlaceOrange, spawnPlaces[i].transform.position, spawnPlaces[i].transform.rotation);
                        entityForSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(playerManager.OwnerClientId);
                    }
                    break;
                case "LineObjective":
                    if (IsHost && lineObjectivePrefab != null)
                    {
                        entityForSpawn = Instantiate(lineObjectivePrefab, spawnPlaces[i].transform.position, spawnPlaces[i].transform.rotation);
                        entityForSpawn.GetComponent<NetworkObject>().Spawn();
                    }
                    break;
                default:
                    Debug.LogError(spawnPlaces[i]+" doesn't have a tag!");
                    break;
            }
        }
    }
}

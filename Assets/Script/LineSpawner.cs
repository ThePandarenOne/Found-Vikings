using Unity.Netcode;
using UnityEngine;

public class LineSpawner : NetworkBehaviour
{
    public GameObject[] lineSpawners;
    public GameObject[] lines;
    int i = 0;
    bool canSpawn = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(canSpawn && IsHost)
        {
            Debug.Log("Check");
            SpawnLine();
        }
    }
    void SpawnLine()
    {
        Debug.Log("SpawnLine");
        canSpawn = false;
        int saverCount = 0;
        if(lines.Length < 3)
        {
            Debug.LogError("Less than 4 lines");
            return;
        }
        if(lineSpawners.Length < 3)
        {
            Debug.LogError("Less than 3 lineSpanwers");
            return;
        }
        int firstLoop = -1;
        int secondLoop = -1;
        for (byte b = 0; b < lineSpawners.Length; b++)
        {
            
            i = Random.Range(0, lines.Length);
            while (i == firstLoop || i == secondLoop)
            {
                saverCount++;
                if(saverCount>= 100)
                {
                    Debug.LogError("ToMuchRandom");
                    return;
                }
                i = Random.Range(0, lines.Length);
            }
            saverCount = 0;
            
            if (b == 0)
            {
                firstLoop = i;
            }
            else if (b == 1)
            {
                secondLoop = i;
            }
            SpawnServerRpc(b,i);
        }
    }
    void Spawn(byte b,int i)
    {
        Debug.Log("MoveLine");
        //lines[i].transform.parent = lineSpawners[b].transform;
        lines[i].GetComponent<Line>().tilemap.transform.position = lineSpawners[b].transform.position;
        lines[i].gameObject.SetActive(true);
        if(IsHost && lines[i].GetComponent<NetworkObject>().IsSpawned == false)
        {
            Debug.Log("Isn't spawned");
            lines[i].GetComponent<NetworkObject>().Spawn();
        }

    }
    [ServerRpc(RequireOwnership = false)]
    void SpawnServerRpc(byte b, int i)
    {
        Debug.Log("Server");
        SpawnClientRpc(b,i);
    }
    [ClientRpc]
    void SpawnClientRpc(byte b, int i)
    {
        Debug.Log("Client");
        Spawn(b, i);
    }
    [ServerRpc(RequireOwnership = false)] void SpawnLineServerRpc()
    {
        Debug.Log("Server");
        SpawnLineClientRpc();
    }
    [ClientRpc] void SpawnLineClientRpc()
    {
        Debug.Log("Client");
        SpawnLine();
    }
}

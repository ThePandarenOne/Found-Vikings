using UnityEngine;
using System.Collections;
using static UnityEngine.Rendering.DebugUI;
using System.Linq;
using Unity.Netcode;
using static Building;

public class BuildPlace : Entity
{
    [Header("BUILDPLACE")]

    public int timer;
    public bool isBuilding = false;
    public Building[] buildings;
    public Building building;
    public BuildPlace buildPlaceBrown;
    public BuildPlace buildPlaceOrange;

    void Start()
    {
        StartObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0)
        {
            if (side == Side.Player)
            {
                AskForBuildingDestroy(Side.Enemy);
            }
            else if (side == Side.Enemy)
            {
                AskForBuildingDestroy(Side.Player);
            }
        }
        UpdateObject();
        if (playerManager == null || playerManager != null&& playerManager.sidePlayer != side)
        {
            playerManager = FindObjectsByType<PlayerManager>(FindObjectsSortMode.None).FirstOrDefault(pm => pm.sidePlayer == side);
            Debug.Log(playerManager);
        }
    }

    // BUILD

    public void BuildSpawner()
    {
        if (isBuilding == false)
        {
            building = buildings[0];
            StartCoroutine(Building());
        }
    }
    public void BuildMine()
    {
        if (isBuilding == false)
        {
            building = buildings[1];
            StartCoroutine(Building());
        }
    }
    public void BuildTower()
    {
        if (isBuilding == false)
        {
            building = buildings[2];
            StartCoroutine(Building());
        }
    }
    IEnumerator Building()
    {
        isBuilding = true;
        for (int b = building.respawnSpeed; b >= 0; b--)
        {
            timer = b;
            yield return new WaitForSeconds(1f);
            if (b <= 0)
            {

                isBuilding = false;
                panel.UpdateUnitsIconsWhileBuilding(this);
                //Building spawnedBuilding = Instantiate(building, transform.position, transform.rotation);

                if(IsHost)
                {
                    Build(AskForBuildingInBuildings());
                }
                else
                {
                    BuildServerRpc(AskForBuildingInBuildings());
                }
            }
        }
    }
    byte AskForBuildingInBuildings()
    {
        for(byte i = 0; i < buildings.Length;i++)
        {
            if (buildings[i] != null && buildings[i] == building)
            {
                return i;
            }
        }
        return 0;
    }
    [ServerRpc(RequireOwnership = false)] void BuildServerRpc(byte i)
    {
        Build(i);
    }
    void Build(byte i)
    {
        if(building == null)
        {
            building = buildings[i];
        }
        Building spawnedBuilding = Instantiate(building, transform.position, Quaternion.identity);
        spawnedBuilding.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);//Строит здание
        if(IsOwner)
        {
            panel.objectUnit = spawnedBuilding;
        }
        BuildClientRpc(spawnedBuilding.GetComponent<NetworkObject>());
    }
    [ClientRpc(RequireOwnership = false)]
    void BuildClientRpc(NetworkObjectReference buildingReference)
    {
        if(buildingReference.TryGet(out NetworkObject networkObject))
        {
            Building buildingSpawned = networkObject.GetComponent<Building>();
            buildingSpawned.playerManager = playerManager;
            building = null;
            panel.ChangePanel();
            Destroy(gameObject);
        }
    }

    // SIDE CHANGE
    /*
    public void SideChange(Side sidee)
    {
        Debug.Log("SideChange");
        hp = maxhp;
        side = sidee;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if(sidee == Side.Player)
        {
            sr.sprite = spriteOrange;
        }
        else if(sidee == Side.Enemy)
        {
            sr.sprite = spriteBrown;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void AskForSideChange(Side sidee)
    {
        Debug.Log("AskForSideChange");
        if(IsHost)
        {
            Debug.Log("IsHost");
            SideChangeClientRpc(sidee);
        }
        else
        {
            Debug.Log("IsClient");
            SideChangeServerRpc(sidee);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void SideChangeServerRpc(Side sidee)
    {
        Debug.Log("SideChangeServerRpc");
        SideChangeClientRpc(sidee);
    }
    [ClientRpc]
    void SideChangeClientRpc(Side sidee)
    {
        Debug.Log("SideChangeClientRpc");
        SideChange(sidee);
    }
    */
    // BUILDING DESTROY

    public void AskForBuildingDestroy(Side side)
    {
        if (IsHost)
        {
            BuildingDestroyClientRpc(side);
        }
        else
        {
            BuildingDestroyServerRpc(side);
        }
    }
    [ServerRpc]
    void BuildingDestroyServerRpc(Side side)
    {
        BuildingDestroyClientRpc(side);
    }
    [ClientRpc]
    void BuildingDestroyClientRpc(Side side)
    {
        BuildingDestroy(side);
    }
    void BuildingDestroy(Side side)
    {
        SpawnBuildingPlacementServerRpc(side);
        Destroy(gameObject);
        //playerManager.UpdateUnitOwnerServerRpc(buildPlace.GetComponent<NetworkObject>());
        //buildPlace.gameObject.SetActive(true);
    }
    [ServerRpc(RequireOwnership = false)]
    public void SpawnBuildingPlacementServerRpc(Side side)
    {
        SpawnBuildingPlacement(side);
    }
    void SpawnBuildingPlacement(Side side_)//Призыв после разрушения buildplace
    {
        {
            BuildPlace spawnedBuilding = null;

            if (side_ == Side.Player)
            {
                spawnedBuilding = Instantiate(buildPlaceOrange, transform.position, Quaternion.identity);
                spawnedBuilding.GetComponent<NetworkObject>().SpawnWithOwnership(0);
            }
            else if (side_ == Side.Enemy)
            {
                spawnedBuilding = Instantiate(buildPlaceBrown, transform.position, Quaternion.identity);
                spawnedBuilding.GetComponent<NetworkObject>().SpawnWithOwnership(1);
            }
            if(side_ == side)
            {
                spawnedBuilding.playerManager = playerManager;
            }
            else
            {
                spawnedBuilding.playerManager = playerManager.enemyManager;
            }
            //spawnedBuilding.GetComponent<NetworkObject>().SpawnWithOwnership(NetworkObject.OwnerClientId);
            panel.objectUnit = spawnedBuilding;
            SpawnBuildingPlacementClientRpc(spawnedBuilding.GetComponent<NetworkObject>());
        }
    }
    [ClientRpc(RequireOwnership = false)]
    void SpawnBuildingPlacementClientRpc(NetworkObjectReference buildingReference)
    {
        if (buildingReference.TryGet(out NetworkObject networkObject))
        {
            BuildPlace buildingSpawned = networkObject.GetComponent<BuildPlace>();
            buildingSpawned.playerManager = playerManager;
            panel.ChangePanel();
        }
    }
}

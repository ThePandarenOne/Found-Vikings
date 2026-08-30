using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UIElements;
using Unity.Netcode;

public class Building : Entity
{
    public enum TypeOfBuilding
    { 
        Spawner,
        Mine,
        Tower,
        MainBuilding
    }

    [Header("BUILDING")]

    public BuildPlace buildPlaceBrown;
    public BuildPlace buildPlaceOrange;
    //public UnitState unitState = UnitState.Idle;
    public TypeOfBuilding typeOfBuilding;
    public List<byte> unitQueue = new List<byte>();
    public bool passiveSpawn;
    public Unit[] unitSpawn;
    protected bool canSpawn = true;
    public GameObject spawnplace;
    void Start()
    {
        StartObject();
        panel = FindAnyObjectByType<Panel>();
        if (passiveSpawn)
        {
            //Spawn(unitSpawn[unitIndex]);
        }
    }
    void Update()
    {
        if(IsSpawned == false)
        {
            return;
        }
        if(spawnplace == null)
        {
            spawnplace = gameObject;
        }
        if(hp <= 0 && typeOfBuilding != TypeOfBuilding.MainBuilding)
        {
            if(side == Side.Player)
            {
                AskForBuildingDestroy(Side.Enemy);
            }
            else if(side == Side.Enemy)
            {
                AskForBuildingDestroy(Side.Player);
            }
        }
        UpdateObject();
        switch (typeOfBuilding)
        {
            case TypeOfBuilding.Spawner:
                break;
            case TypeOfBuilding.Mine:
                if (IsOwner && readyAttack && NetworkManager.Singleton.ServerTime.Time > timerCooldown)
                {
                    readyAttack = false;
                    ReadyGiveMoneyCheckServerRpc(2);
                }
                break;
            case TypeOfBuilding.Tower:
                break;
            case TypeOfBuilding.MainBuilding:
                break;
        }
        switch(currentState.Value)
        {
            case UnitState.Idle:
                break;
            case UnitState.Defend:
                if (targetUnit == null)
                {
                    Collider2D[] touchableObjects = Physics2D.OverlapCircleAll(transform.position, range);
                    foreach (Collider2D touchableObject in touchableObjects)
                    {
                        if (touchableObject.TryGetComponent(out Entity unit) && unit.side != side)
                        {
                            targetUnit = unit;
                        }
                    }
                }
                if (targetUnit != null && Mathf.Abs(targetUnit.transform.position.x - transform.position.x) < range && readyAttack)
                {
                    AskForAttack();
                }
                break;
            case UnitState.Attack:
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                    if (hit == true)
                    {
                        hit.transform.gameObject.TryGetComponent(out targetUnit);
                    }
                }
                if (targetUnit != null && Mathf.Abs(targetUnit.transform.position.x - transform.position.x) < range && readyAttack)
                {
                    AskForAttack();
                }
                break;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    protected virtual void ReadyGiveMoneyCheckServerRpc(int count)
    {
        ReadyGiveMoneyCheckClientRpc(count);
    }
    [ClientRpc(RequireOwnership = false)]
    protected virtual void ReadyGiveMoneyCheckClientRpc(int count)
    {
        if(IsOwner)
        {
            timerCooldown = NetworkManager.Singleton.ServerTime.Time + attackTime;
            panel.playerManager.money += count;
            readyAttack = true;
        }
    }

    // ACTIONS

    public void Attack()
    {
        AskForChangeUnitState(UnitState.Attack);
    }
    public void AttackPosition()
    {
        AskForChangeUnitState(UnitState.Defend);
    }
    public void SelfDestroy()
    {
        StartCoroutine(WaitForDestroy());
    }
    IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(2f);
        AskForBuildingDestroy(side);
    }

    // QUEUE
    public void AskForAddUnitToQueue(byte NameOfUnit)
    {
        if(IsHost)
        {
            AddUnitToQueueClientRpc(NameOfUnit);
        }
        else
        {
            AddUnitToQueueServerRpc(NameOfUnit);
        }
    }
    [ServerRpc]void AddUnitToQueueServerRpc(byte NameOfUnit)
    {
        AddUnitToQueueClientRpc(NameOfUnit);
    }

    [ClientRpc]void AddUnitToQueueClientRpc(byte NameOfUnit)
    {
        AddUnitToQueue(NameOfUnit);
    }

    public virtual void AddUnitToQueue(byte NameOfUnit)//Добавляет юнита в очередь
    {
        //Debug.Log("AddUnitToQueue");
        if(unitQueue.Count < 5)
        {
            unitQueue.Add(NameOfUnit);
            if(canSpawn)
            {
                canSpawn = false;
                StartCoroutine(WaitForSpawn(unitSpawn[NameOfUnit]));
            }
        }
    }
    void QueueUpdate()//Обновляет в очередь
    {
        //Debug.Log("QueueUpdate");
        if (unitQueue.Count > 0)
        {
            panel.UpdateUnitsIconsInQueue(this);
            unitQueue.RemoveAt(0);
        }
        if (unitQueue.Count > 0)
        {
            StartCoroutine(WaitForSpawn(unitSpawn[unitQueue[0]]));
        }
    }
    public int timer;

    protected IEnumerator WaitForSpawn(Unit unit)
    {
        for (int i = unit.respawnSpeed; i >= 0; i--)
        {
            timer = i;
            yield return new WaitForSeconds(1f);
            if (i <= 0)
            {
                Unit un = null;
                if(IsHost)
                {
                    un = Instantiate(unit, new Vector3(spawnplace.transform.position.x, spawnplace.transform.position.y, 0), spawnplace.transform.rotation);
                }
                un.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
                un.playerManager = playerManager;
                canSpawn = true;
                QueueUpdate();
            }
        }
    }

    // BUILDING DESTROY

    public void AskForBuildingDestroy(Side side)
    {
        if(IsHost)
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
    void SpawnBuildingPlacement(Side side)
    {
        {
            BuildPlace spawnedBuilding = null;

            if (side == Side.Player)
            {
                spawnedBuilding = Instantiate(buildPlaceOrange, transform.position, Quaternion.identity);
                spawnedBuilding.GetComponent<NetworkObject>().SpawnWithOwnership(0);//Призывает BuildPlace
            }
            else if (side == Side.Enemy)
            {
                spawnedBuilding = Instantiate(buildPlaceBrown, transform.position, Quaternion.identity);
                spawnedBuilding.GetComponent<NetworkObject>().SpawnWithOwnership(1);//Призывает BuildPlace
            }
            if(side == Side.Enemy)
            {

            }
            else if(side == Side.Player)
            {

            }
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

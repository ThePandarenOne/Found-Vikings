using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UIElements;
using Unity.Netcode;

public class Building : Object
{
    public enum TypeOfBuilding
    { 
        Spawner,
        Mine,
        Tower,
        MainBuilding
    }
    public BuildPlace buildPlace;
    //public UnitState unitState = UnitState.Idle;
    public TypeOfBuilding typeOfBuilding;
    public List<byte> unitQueue = new List<byte>();
    public bool passiveSpawn;
    public Unit[] unitSpawn;
    private bool canSpawn = true;
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
        if(hp <= 0 && typeOfBuilding != TypeOfBuilding.MainBuilding)
        {
            AskForBuildingDestroy();
        }
        UpdateObject();
        switch (typeOfBuilding)
        {
            case TypeOfBuilding.Spawner:
                break;
            case TypeOfBuilding.Mine:
                if(readyAttack)
                {
                    StartCoroutine(GiveMoney(2));
                    readyAttack = false;
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
                        if (touchableObject.TryGetComponent(out Unit unit) && unit.side != side)
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
    IEnumerator GiveMoney(int count)
    {
        yield return new WaitForSeconds(1f);
        panel.playerManager.money += count;
        readyAttack = true;
    }
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
        hp = 0;
    }
    public void AddUnitToQueue(byte NameOfUnit)//Добавляет юнита в очередь
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
    public void AskForBuildingDestroy()
    {
        if(IsHost)
        {
            BuildingDestroyClientRpc();
        }
        else
        {
            BuildingDestroyServerRpc();
        }
    }
    [ServerRpc]
    void BuildingDestroyServerRpc()
    {
        BuildingDestroyClientRpc();
    }
    [ClientRpc]
    void BuildingDestroyClientRpc()
    {
        BuildingDestroy();
    }
    void BuildingDestroy()
    {
        Destroy(gameObject);
        playerManager.UpdateUnitOwnerServerRpc(buildPlace.GetComponent<NetworkObject>());
        buildPlace.gameObject.SetActive(true);
    }

    IEnumerator WaitForSpawn(Unit unit)
    {
        for (int i = unit.respawnSpeed; i >= 0; i--)
        {
            timer = i;
            yield return new WaitForSeconds(1f);
            if (i <= 0)
            {
                Unit un = Instantiate(unit, new Vector3(spawnplace.transform.position.x, spawnplace.transform.position.y, 0), spawnplace.transform.rotation);
                un.playerManager = playerManager;
                canSpawn = true;
                QueueUpdate();
            }
        }
    }
}

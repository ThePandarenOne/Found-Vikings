using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UIElements;

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
    public UnitState unitState = UnitState.Idle;
    public TypeOfBuilding typeOfBuilding;
    public List<byte> unitQueue = new List<byte>();
    public bool passiveSpawn;
    public Unit[] unitSpawn;
    private bool canSpawn = true;
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
        if(hp <= 0)
        {
            buildPlace.gameObject.SetActive(true);
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
                if (readyAttack)
                {
                    StartCoroutine(GiveMoney(1));
                    readyAttack = false;
                }
                break;
        }
        switch(unitState)
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
                    AttackTarget();
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
                    AttackTarget();
                }
                break;
        }
    }
    IEnumerator GiveMoney(int count)
    {
        yield return new WaitForSeconds(1f);
        panel.money += count;
        readyAttack = true;
    }
    public void Attack()
    {
        unitState = UnitState.Attack;
    }
    public void AttackPosition()
    {
        unitState = UnitState.Defend;
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
        Debug.Log("AddUnitToQueue");
        if(unitQueue.Count < 5)
        {
            unitQueue.Add(NameOfUnit);
            if(canSpawn)
            {
                canSpawn = false;
                StartCoroutine(WaitForSpawn(unitSpawn[NameOfUnit]));
                //FindUnit(NameOfUnit);
            }
        }
    }
    void QueueUpdate()//Обновляет в очередь
    {
        Debug.Log("QueueUpdate");
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
    IEnumerator WaitForSpawn(Unit unit)
    {
        for (int i = unit.respawnSpeed; i >= 0; i--)
        {
            timer = i;
            yield return new WaitForSeconds(1f);
            if (i <= 0)
            {
                Instantiate(unit, new Vector3(transform.position.x + 1, transform.position.y, 0), transform.rotation);
                canSpawn = true;
                QueueUpdate();
            }
        }
    }
}

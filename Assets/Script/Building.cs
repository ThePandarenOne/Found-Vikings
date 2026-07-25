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
                GiveMoney();
                readyAttack = false;
                break;
            case TypeOfBuilding.Tower:
                break;
        }
    }
    IEnumerator GiveMoney()
    {
        yield return new WaitForSeconds(1f);
        panel.money += 4;
        readyAttack = true;
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
            //FindUnit(unitQueue[0]);///
        }
    }
    /*
    void FindUnit(string nameUnit)//Переводит имя юнита в номер для спавна
    {
        Debug.Log("FindUnit");
        for (byte i = 0; i<unitSpawn.Length;i++)
        {
            if (unitSpawn[i] != null && unitSpawn[i].name == nameUnit)
            {
                StartCoroutine(WaitForSpawn(unitSpawn[i]));
                break;
            }
        }
        
    }
    */
    public int timer;
    IEnumerator WaitForSpawn(Unit unit)
    {
        Debug.Log("WaitForSpawn");
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

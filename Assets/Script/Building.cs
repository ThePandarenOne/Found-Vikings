using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

public class Building : Object
{
    List<string> unitQueue = new List<string>();
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
        UpdateObject();
    }
    public void AddUnitToQueue(string NameOfUnit)//Добавляет юнита в очередь
    {
        Debug.Log("AddUnitToQueue");
        if(unitQueue.Count < 5)
        {
            unitQueue.Add(NameOfUnit);
            if(canSpawn)
            {
                canSpawn = false;
                FindUnit(NameOfUnit);
            }
        }
    }
    void QueueUpdate()//Обновляет в очередь
    {
        Debug.Log("QueueUpdate");
        if (unitQueue.Count > 0)
        {
            unitQueue.RemoveAt(0);
        }
        if (unitQueue.Count > 0)
        {
            FindUnit(unitQueue[0]);
        }
    }
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
    
    IEnumerator WaitForSpawn(Unit unit)
    {
        Debug.Log("WaitForSpawn");
        for (int i = unit.respawnSpeed; i >= 0; i--)
        {
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

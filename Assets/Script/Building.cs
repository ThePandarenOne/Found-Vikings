using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

public class Building : Object
{
    //Dictionary<int,string> unitOnQueue = new Dictionary<int,string>();
    List<string> unitQueue = new List<string>();
    public bool passiveSpawn;
    public Unit[] unitSpawn;
    //private byte unitIndex;
    //byte freeNumber;

    //private bool readyRespawn;
    private bool canSpawn = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panel = FindAnyObjectByType<Panel>();
        if (passiveSpawn)
        {
            //Spawn(unitSpawn[unitIndex]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateObject();
        /*
        if (readyRespawn)
        {
            readyRespawn = false;
            if(passiveSpawn)
            {
                //Instantiate(unitSpawn, new Vector3(transform.position.x - 1, transform.position.y, 0), transform.rotation);
                //Spawn(unitSpawn);
            }
            else
            {
                Instantiate(unitSpawn[unitIndex], new Vector3(transform.position.x + 1, transform.position.y, 0), transform.rotation);
            }
        }
        /*
        if(unitOnQueue.Count != 0 && canSpawn)
        {
            FindUnit(unitOnQueue[1]);
        }
        */
    }
    
    public void Spawn(Unit unit)//Спавнит юнита
    {
        Debug.Log("Spawn");
        //this.unitIndex = queue[0];
        StartCoroutine(WaitForSpawn(unit));
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
        unitQueue.RemoveAt(0);
        if (unitQueue.Count > 0)
        {
            FindUnit(unitQueue[0]);
        }
        /*
        for (byte i = 0; i <= unitQueue.Count; i++)
        {
            if (i == unitQueue.Count)
            {
                i = 0;
            }
            else
            {
                unitQueue.RemoveAt(0);
                if (unitQueue.Count > 0)
                {
                    FindUnit(unitQueue[0]);
                }
            }
        }
        /*
        Debug.Log(unitOnQueue);
        for(byte i = 0; i <= unitOnQueue.Count; i++)
        {
            if(i == unitOnQueue.Count)
            {
                i = 0;
            }
            else
            {
                unitOnQueue.Add(i, unitOnQueue[i + 1]);
            }
        }
        */
    }
    void FindUnit(string nameUnit)
    {
        Debug.Log("FindUnit");
        Debug.Log(canSpawn);
        for (byte i = 0; i<unitSpawn.Length;i++)
        {
            if (unitSpawn[i] != null && unitSpawn[i].name == nameUnit)
            {
                Spawn(unitSpawn[i]);
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
                //readyRespawn = true;
                canSpawn = true;
                QueueUpdate();
            }
        }
    }
}

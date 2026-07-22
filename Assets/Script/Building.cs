using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

public class Building : Object
{
    Dictionary<int,string> unitOnQueue = new Dictionary<int,string>();
    public bool passiveSpawn;
    public Unit[] unitSpawn;
    private byte unitIndex;
    byte freeNumber;

    private bool readyRespawn;
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
        if(unitOnQueue.Count != 0 && canSpawn)
        {
            FindUnit(unitOnQueue[1]);
        }
    }
    
    public void Spawn(Unit unit)//Спавнит юнита
    {
        //this.unitIndex = queue[0];
        if(canSpawn)
        {
            StartCoroutine(WaitForSpawn(unit));
            canSpawn = false;
        }
    }
    public void AddUnitToQueue(string NameOfUnit)//Добавляет юнита в очередь
    {
        if(unitOnQueue.Count < 5)
        {
            unitOnQueue.Add(unitOnQueue.Count + 1, NameOfUnit);
        }
    }
    void QueueUpdate()//Обновляет в очередь
    {
        
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
        
        //Хочу сделать очередь до 5 юнитов. Чтобы когда новый юнит появлялся, первый элемент массива/словаря исчезал и прочие двигались в верх.
    }
    void FindUnit(string nameUnit)
    {
        for(byte i = 0; i<unitSpawn.Length;i++)
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
        for(int i = unit.respawnSpeed; i >= 0; i--)
        {
            yield return new WaitForSeconds(1f);
            if (i <= 0)
            {
                readyRespawn = true;
                canSpawn = true;
                QueueUpdate();
            }
        }
    }
}

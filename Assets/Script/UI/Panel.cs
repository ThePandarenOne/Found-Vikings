using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Panel : MonoBehaviour
{
    public PlayerManager playerManager;
    public UnitIcon[] unitIcons;
    public PanelButton[] buttons;
    public Text nameOfSelectUnit;
    public Object objectUnit;
    public Text[] characteristics;
    public Text moneyCounter;
    public Slider sliderHP;
    public Group group;
    public Group groupPrefab;
    bool cannew = true;
    List<Unit> units = new List<Unit>();

    public SelectionSquare selectionSquare;
    SelectionSquare sl;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void ChangePanel()
    {
        for (byte i = 0; i < buttons.Length; i++)
        {
            buttons[i].action = null;
            buttons[i].acsessButton.onClick.RemoveAllListeners();
            buttons[i].gameObject.SetActive(false);
            if (objectUnit != null)
            {
                if (objectUnit.TryGetComponent(out Unit unit) && group == null && unit.action[i] != null)
                {
                    buttons[i].action = unit.action[i];
                    buttons[i].GetAction();
                    if (objectUnit.side == playerManager.sidePlayer)
                    {
                        buttons[i].gameObject.SetActive(true);
                        buttons[i].acsessButton.onClick.AddListener(buttons[i].GetCost);
                        GiveActionUnit(i);
                    }
                }
                else if (objectUnit.TryGetComponent(out Building build) && build.action[i] != null)
                {
                    buttons[i].action = build.action[i];
                    buttons[i].GetAction();
                    if (objectUnit.side == playerManager.sidePlayer)
                    {
                        buttons[i].gameObject.SetActive(true);
                        buttons[i].acsessButton.onClick.AddListener(buttons[i].GetCost);
                        GiveActionBuilding(i);
                    }
                }
                else if (objectUnit.TryGetComponent(out BuildPlace buildPlace) && buildPlace.action[i])
                {
                    buttons[i].action = buildPlace.action[i];
                    buttons[i].GetAction();
                    if (objectUnit.side == playerManager.sidePlayer)
                    {
                        buttons[i].gameObject.SetActive(true);
                        buttons[i].acsessButton.onClick.AddListener(buttons[i].GetCost);
                        GiveActionBuildingPlacement(i);
                    }
                }

                else
                {
                    buttons[i].gameObject.SetActive(false);
                }
            }
        }
        if (objectUnit != null)
        {
            nameOfSelectUnit.text = objectUnit.name;
            characteristics[0].text = "Damage:" + objectUnit.dmg.ToString();
            characteristics[1].text = "AttackSpeed:" + objectUnit.attackTime.ToString();
            characteristics[2].text = "Speed:" + objectUnit.speed.ToString();
            characteristics[4].text = "Range:" + objectUnit.range.ToString();
        }
        else
        {
            foreach (Text c in characteristics)
            {
                c.text = "";
            }
        }
    }
    public void GiveActionUnit(byte i)
    {
        Unit unit = objectUnit.GetComponent<Unit>();
        if (buttons[i].action.namE == "ClickMove")
        {
            buttons[i].acsessButton.onClick.AddListener(unit.ClickMovement);
        }
        if (buttons[i].action.namE == "Attack")
        {
            buttons[i].acsessButton.onClick.AddListener(unit.Attack);
        }
        if (buttons[i].action.namE == "Defend")
        {
            buttons[i].acsessButton.onClick.AddListener(unit.AttackPosition);
        }
        if (buttons[i].action.namE == "JumpUp")
        {
            buttons[i].acsessButton.onClick.AddListener(unit.JumpUp);
        }
        if (buttons[i].action.namE == "JumpDown")
        {
            buttons[i].acsessButton.onClick.AddListener(unit.JumpDown);
        }
    }
    public void GiveActionUnitsInGroup(byte i)
    {
        units = group.units;
        foreach (Unit unit in units)
        {
            if (buttons[i].action != null)
            {
                if (buttons[i].action.namE == "ClickMove")
                {
                    buttons[i].acsessButton.onClick.AddListener(unit.ClickMovement);
                }
                if (buttons[i].action.namE == "Attack")
                {
                    buttons[i].acsessButton.onClick.AddListener(unit.Attack);
                }
                if (buttons[i].action.namE == "Defend")
                {
                    buttons[i].acsessButton.onClick.AddListener(unit.AttackPosition);
                }
                if (buttons[i].action.namE == "JumpUp")
                {
                    buttons[i].acsessButton.onClick.AddListener(unit.JumpUp);
                }
                if (buttons[i].action.namE == "JumpDown")
                {
                    buttons[i].acsessButton.onClick.AddListener(unit.JumpDown);
                }
            }
        }
    }
    public void GiveActionBuilding(byte i)
    {
        Building building = objectUnit.GetComponent<Building>();
        if (buttons[i].action.namE == "Spawn Baleog")
        {
            buttons[i].acsessButton.onClick.AddListener(() => building.AddUnitToQueue(1));
        }
        if (buttons[i].action.namE == "Spawn Olaf")
        {
            buttons[i].acsessButton.onClick.AddListener(() => building.AddUnitToQueue(2));
        }
        if (buttons[i].action.namE == "Spawn Erik")
        {
            buttons[i].acsessButton.onClick.AddListener(() => building.AddUnitToQueue(3));
        }
        if (buttons[i].action.namE == "Attack")
        {
            buttons[i].acsessButton.onClick.AddListener(building.Attack);
        }
        if (buttons[i].action.namE == "Defend")
        {
            buttons[i].acsessButton.onClick.AddListener(building.AttackPosition);
        }
        if (buttons[i].action.namE == "DestroyBuilding")
        {
            buttons[i].acsessButton.onClick.AddListener(building.SelfDestroy);
        }
    }
    public void GiveActionBuildingPlacement(byte i)
    {
        BuildPlace building = objectUnit.GetComponent<BuildPlace>();
        if (buttons[i].action.namE == "Build mine")
        {
            buttons[i].acsessButton.onClick.AddListener(building.BuildMine);
        }
        if (buttons[i].action.namE == "Build spawner")
        {
            buttons[i].acsessButton.onClick.AddListener(building.BuildSpawner);
        }
        if (buttons[i].action.namE == "Build tower")
        {
            buttons[i].acsessButton.onClick.AddListener(building.BuildTower);
        }
    }
    // Update is called once per frame
    void UpdateUnitsIconsInGroup()
    {
        //Debug.Log("UpdateUnitsIconsInGroup");
        for(byte i = 0; i < 12;i++)
        {
            //Debug.Log(i);
            //Debug.Log(group.units.Count);
            if (i >= group.units.Count)
            {
                unitIcons[i].typeOfIcon = UnitIcon.TypeOfIcon.Disabled;
            }
            else if (group.units[i] != null)
            {
                unitIcons[i].typeOfIcon = UnitIcon.TypeOfIcon.UnitGroupIcon;
                unitIcons[i].gameObject.SetActive(true);  
                unitIcons[i].unit = group.units[i];
            }
        }
    }
    public void UpdateUnitsIconsInQueue(Building build)
    {
        for (byte i = 0; i < unitIcons.Length; i++)
        {
            if (build.unitQueue.Count > 0)
            {
                if (i < build.unitQueue.Count)
                {
                    unitIcons[i].typeOfIcon = UnitIcon.TypeOfIcon.UnitQueueIcon;
                    unitIcons[i].gameObject.SetActive(true);
                    unitIcons[i].unit = build.unitSpawn[build.unitQueue[i]];
                }
                else
                {
                    unitIcons[i].typeOfIcon = UnitIcon.TypeOfIcon.Disabled;
                }
            }
        }
    }
    public void UpdateUnitsIconsWhileBuilding(BuildPlace build)
    {
        if (build.isBuilding)
        {
            unitIcons[0].unit = build;
            unitIcons[0].typeOfIcon = UnitIcon.TypeOfIcon.Building;
            unitIcons[0].gameObject.SetActive(true);
        }
        else
        {
            unitIcons[0].typeOfIcon = UnitIcon.TypeOfIcon.Disabled;
        }
    }
    IEnumerator WaitForSquare()
    {
        Vector2 a = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        yield return new WaitForSeconds(0.1f);
        Vector2 b = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(0) && sl == null)
        {
            if((a.x - b.x > 2f) && (a.y - b.y > 2f) || (a.x - b.x < 2f) && (a.y - b.y < 2f))
            {
                sl = Instantiate(selectionSquare, b, transform.rotation);
                sl.panel = this;
                sl.playerManager = playerManager;
                sl.startPoint = a;
            }
        }
    }
    void Update()
    {
        if(Input.GetMouseButton(0) && sl == null)//Создаёт квадрат выделения
        {
            StartCoroutine(WaitForSquare());
        }
        moneyCounter.text = "Money:"+playerManager.money;
        if (group != null && group.units.Count > 0)//Обновляет иконки юнитов в группе
        {
            UpdateUnitsIconsInGroup();
        }
        else if (objectUnit == null && group == null ||  objectUnit != null &&objectUnit.gameObject.activeSelf == false && group == null)//
        {
            objectUnit = null;
            foreach(UnitIcon u in unitIcons)
            {
                u.typeOfIcon = UnitIcon.TypeOfIcon.Disabled;
            }
            ChangePanel();
        }
        else if (group != null && group.units.Count == 0)//Если группа создана, но пуста.
        {
            foreach (PanelButton panelButton in buttons)
            {
                panelButton.gameObject.SetActive(false);
            }
            foreach (UnitIcon unit in unitIcons)
            {
                unit.typeOfIcon = UnitIcon.TypeOfIcon.Disabled;
            }
        }
        else if (objectUnit != null && objectUnit.TryGetComponent(out Building build) && build.unitQueue.Count > 0)//Обновляет иконки во время очереди
        {
            UpdateUnitsIconsInQueue(build);
        }
        else if (objectUnit != null && objectUnit.TryGetComponent(out BuildPlace buildPlace))//Обновляет иконки во время строительства
        {
            foreach (UnitIcon u in unitIcons)
            {
                u.typeOfIcon = UnitIcon.TypeOfIcon.Disabled;
            }
            UpdateUnitsIconsWhileBuilding(buildPlace);
        }
        else
        {
            foreach (UnitIcon unit in unitIcons)
            {
                unit.typeOfIcon = UnitIcon.TypeOfIcon.Disabled;
            }
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (group == null || cannew)
            {
                SpawnGroup();
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                group.SearchForUnit(false);
            }
        }
        else
        {
            cannew = true;
        }
        if (objectUnit != null)
        {
            if (sliderHP.gameObject.activeSelf == false)
            {
                sliderHP.gameObject.SetActive(true);
            }
            sliderHP.maxValue = objectUnit.maxhp;
            sliderHP.value = objectUnit.hp;
            characteristics[3].text = "HP:" + objectUnit.hp.ToString() + "/" + objectUnit.maxhp.ToString();
        }
        else
        {
            sliderHP.gameObject.SetActive(false);
        }
    }
    public void SpawnGroup()
    {
        objectUnit = null;
        group = Instantiate(groupPrefab);
        group.panel = this;
        group.playerManager = playerManager;
        cannew = false;
    }
}
   
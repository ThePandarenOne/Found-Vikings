using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Panel : MonoBehaviour
{
    public int money;
    public UnitIcon[] unitIcons;
    public PanelButton[] buttons;
    public Text nameOfSelectUnit;
    public Object objectUnit;
    public Text[] characteristics;
    public Slider sliderHP;
    public Group group;
    public Group groupPrefab;
    bool cannew = true;
    List<Unit> units = new List<Unit>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void ChangePanel()
    {
        for(byte i = 0; i < buttons.Length; i++)
        {
            buttons[i].action = null;
            buttons[i].acsessButton.onClick.RemoveAllListeners();
            buttons[i].gameObject.SetActive(false);
            if (objectUnit != null)
            {
                if (objectUnit.TryGetComponent(out Unit unit) && group == null && unit.action[i])
                {
                    buttons[i].action = unit.action[i];
                    buttons[i].GetAction();
                    if (objectUnit.side != Object.Side.Enemy)
                    {
                        buttons[i].gameObject.SetActive(true);
                        GiveActionUnit(i);
                    }
                }
                else if (objectUnit.TryGetComponent(out Building build) && build.action[i])
                {
                    buttons[i].action = build.action[i];
                    buttons[i].GetAction();
                    if (objectUnit.side != Object.Side.Enemy)
                    {
                        buttons[i].gameObject.SetActive(true);
                        GiveActionBuilding(i);
                    }
                }
                else if (objectUnit.TryGetComponent(out BuildPlace buildPlace) && buildPlace.action[i])
                {
                    buttons[i].action = buildPlace.action[i];
                    buttons[i].GetAction();
                    if (objectUnit.side == Object.Side.Neutral)
                    {
                        buttons[i].gameObject.SetActive(true);
                        GiveActionBuildingPlacement(i);
                    }
                }

                else
                {
                    buttons[i].gameObject.SetActive(false);
                }
            }
        }
        
        if(objectUnit != null)
        {
            characteristics[0].text = "Damage:" + objectUnit.dmg.ToString();
            characteristics[1].text = "AttackSpeed:" + objectUnit.attackTime.ToString();
            characteristics[2].text = "Speed:" + objectUnit.speed.ToString();
            characteristics[4].text = "Range:" + objectUnit.range.ToString();
            nameOfSelectUnit.text = objectUnit.name;
        }
        else
        {
            sliderHP.gameObject.SetActive(false);
            foreach(Text c in characteristics)
            {
                c.text = "";
            }
        }
        
    }
    public void GiveActionUnit(byte i)
    {
        Unit unit = objectUnit.GetComponent<Unit>();
        if (buttons[i].action.namE == "A/D")
        {
            buttons[i].acsessButton.onClick.AddListener(unit.AnD);
        }
        if (buttons[i].action.namE == "FollowCursor")
        {
            buttons[i].acsessButton.onClick.AddListener(unit.CursorFollow);
        }
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
    }
    public void GiveActionUnitsInGroup(byte i)
    {
        units = group.units;
        foreach (Unit unit in units)
        {
            if (buttons[i].action.namE == "A/D")
            {
                buttons[i].acsessButton.onClick.AddListener(unit.AnD);
            }
            if (buttons[i].action.namE == "FollowCursor")
            {
                buttons[i].acsessButton.onClick.AddListener(unit.CursorFollow);
            }
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
    bool startLoop = true;
    void UpdateUnitsIconsInGroup()
    {
        for(byte i = 0; i < group.units.Count;i++)
        {
            if(group.units[i] != null)
            {
                unitIcons[i].typeOfIcon = UnitIcon.TypeOfIcon.UnitGroupIcon;
                unitIcons[i].gameObject.SetActive(true);  
                unitIcons[i].unit = group.units[i];
            }
        }
        startLoop = true;
    }
    public void UpdateUnitsIconsInQueue(Building build)
    {
        startLoop = false;
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
        startLoop = true;
    }
    void UpdateUnitsIconsWhileBuilding(BuildPlace build)
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
    void Update()
    {
        if(objectUnit == null || objectUnit.gameObject.activeSelf == false)
        {
            objectUnit = null;
            ChangePanel();
        }
        if(group != null && group.units.Count > 0 && startLoop)
        {
            UpdateUnitsIconsInGroup();
        }
        else if(group != null && group.units.Count == 0)
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
        else if(objectUnit != null && objectUnit.TryGetComponent(out Building build) && build.unitQueue.Count > 0 && startLoop)
        {
            UpdateUnitsIconsInQueue(build);
        }
        else if(objectUnit != null && objectUnit.TryGetComponent(out BuildPlace buildPlace))
        {
            UpdateUnitsIconsWhileBuilding(buildPlace);
        }
        else
        {
            foreach(UnitIcon unit in unitIcons)
            {
                unit.typeOfIcon = UnitIcon.TypeOfIcon.Disabled;
            }
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            objectUnit = null;
            if (group == null || cannew)
            {
                group = Instantiate(groupPrefab);
                group.panel = this;
                cannew = false;
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                group.SearchForUnit();
            }
        }
        else
        {
            cannew = true;
        }
        if(objectUnit != null)
        {
            if(sliderHP.gameObject.activeSelf == false)
            {
                sliderHP.gameObject.SetActive(true);
            }
            sliderHP.maxValue = objectUnit.maxhp;
            sliderHP.value = objectUnit.hp;
            characteristics[3].text = "HP:" + objectUnit.hp.ToString() + "/" + objectUnit.maxhp.ToString();
        }
    }
}
   
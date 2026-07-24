using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Panel : MonoBehaviour
{
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
            if (objectUnit.GetComponent<Unit>() && group == null)
            {
                if (objectUnit.GetComponent<Unit>().action[i])
                {
                    buttons[i].gameObject.SetActive(true);
                    buttons[i].action = objectUnit.GetComponent<Unit>().action[i];
                    buttons[i].GetAction();
                    if (objectUnit.enemy == false)
                    {
                        GiveActionUnit(i);
                    }
                }
            }
            else if (objectUnit.GetComponent<Building>())
            {
                if (objectUnit.GetComponent<Building>().action[i])
                {
                    buttons[i].gameObject.SetActive(true);
                    buttons[i].action = objectUnit.GetComponent<Building>().action[i];
                    buttons[i].GetAction();
                    if (objectUnit.enemy == false)
                    {
                        GiveActionBuilding(i);
                    }
                }
            }

            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
        characteristics[0].text = "Damage:" + objectUnit.dmg.ToString();
        characteristics[1].text = "AttackSpeed:" + objectUnit.attackspeed.ToString();
        characteristics[2].text = "Speed:" + objectUnit.speed.ToString();
        characteristics[4].text = "Range:" + objectUnit.range.ToString();
        nameOfSelectUnit.text = objectUnit.name;
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
            buttons[i].acsessButton.onClick.AddListener(() => building.AddUnitToQueue("Baleog"));
        }
        if (buttons[i].action.namE == "Spawn Olaf")
        {
            buttons[i].acsessButton.onClick.AddListener(() => building.AddUnitToQueue("Olaf"));
        }
        if (buttons[i].action.namE == "Spawn Erik")
        {
            buttons[i].acsessButton.onClick.AddListener(() => building.AddUnitToQueue("Erick"));
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
        for (byte i = 0; i < build.unitQueue.Count; i++)
        {
            unitIcons[i].typeOfIcon = UnitIcon.TypeOfIcon.UnitQueueIcon;
            if (i == group.units.Count - 1)
            {
                startLoop = true;
            }
        }
    }
    void Update()
    {
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
            sliderHP.maxValue = objectUnit.maxhp;
            sliderHP.value = objectUnit.hp;
            characteristics[3].text = "HP:" + objectUnit.hp.ToString() + "/" + objectUnit.maxhp.ToString();
        }
    }
}
   
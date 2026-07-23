using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Panel : MonoBehaviour
{
    public PanelButton[] buttons;
    public Text nameOfSelectUnit;
    public Object objectUnit;
    public Text[] characteristics;
    public Slider sliderHP;
    public Group group;
    public Group groupPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void ChangePanel()
    {
        for(byte i = 0; i < buttons.Length; i++)
        {
            buttons[i].acsessButton.onClick.RemoveAllListeners();
            if (objectUnit.GetComponent<Unit>() && group == null)
            {
                if (objectUnit.GetComponent<Unit>().action[i] != null)
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
                if (objectUnit.GetComponent<Building>().action[i] != null)
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
        characteristics[3].text = "Range:" + objectUnit.range.ToString();

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
    List<Unit> units = new List<Unit>();
    public void GiveActionUnitsInGroup(byte i)
    {
        units = group.units;
        foreach (Unit unit in units)
        {
            if (buttons[i].action.namE == "A/D")
            {
                Debug.Log("GetListener");
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
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            objectUnit = null;
            if(group != null)
            {
                Destroy(group);
            }
            group = Instantiate(groupPrefab);
            group.panel = this;
        }
        if(objectUnit != null)
        {
            sliderHP.maxValue = objectUnit.maxhp;
            sliderHP.value = objectUnit.hp;
            characteristics[3].text = "HP:" + objectUnit.hp.ToString() + "/" + objectUnit.maxhp.ToString();
        }
    }
}

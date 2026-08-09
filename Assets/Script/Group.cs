using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Object;

public class Group : MonoBehaviour
{
    public PlayerManager playerManager;
    public List<Unit> units = new List<Unit>();
    public Panel panel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Unit unit in units)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                unit.unitState = UnitState.Move;
                if (panel.objectUnit == this || unit.SearchForUnitInGroup() == true)
                {
                    unit.clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if (hit == true && hit.transform.gameObject.TryGetComponent(out unit.targetUnit))
                {
                    unit.unitState = UnitState.Hunt;
                }
            }
        }
        if (panel.group != this)
        {
            Destroy(gameObject);
        }
        panel.nameOfSelectUnit.text = "Group: " + units.Count.ToString();
    }
    public void SearchForUnit()
    {
        RaycastHit2D rayhit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (rayhit && rayhit.collider.gameObject.TryGetComponent(out Unit unit) && units.Count < 12 && askForUnitInGroup(unit) == false && unit.side == playerManager.sidePlayer)
        {
            units.Add(unit);
            UpdateActions();
        }
    }
    private void UpdateActions()
    {
        for (byte i = 0; i < panel.buttons.Length; i++)
        {
            panel.buttons[i].acsessButton.onClick.RemoveAllListeners();
            panel.buttons[i].gameObject.SetActive(true);
            for (byte u = 0; u < units.Count; u++)
            {
                panel.buttons[i].action = units[u].action[i];
                panel.buttons[i].GetAction();
                panel.GiveActionUnitsInGroup(i);
            }
        }
    }
    private bool askForUnitInGroup(Unit unit)
    {
        for (byte i = 0; i < units.Count; i++)
        {
            if (units[i] != null &&units[i] == unit)
            {
                return true;
            }
        }
        return false;
    }
}

using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SelectionSquare : MonoBehaviour
{
    public Panel panel;
    public Vector2 startPoint;
    public PlayerManager playerManager;
    Vector2 mousePosition;
    bool canAdd;
    bool hasDone = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = startPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.localScale = new Vector3(startPoint.x - mousePosition.x,startPoint.y - mousePosition.y);
            transform.position = new Vector3(startPoint.x/2 + mousePosition.x/2, startPoint.y/2 + mousePosition.y/2);
        }
        if (!Input.GetMouseButton(0))
        {
            if (hasDone)
            {
                panel.SpawnGroup();
                hasDone = false;
                canAdd = true;
                StartCoroutine(WaitForDestroy());
            }
        }
    }
    IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(0.1f);
        if (panel.group.units.Count == 0)
        {
            Destroy(panel.group.gameObject);
            panel.ChangePanel();
        }
        Destroy(gameObject);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Unit unit) && AskForUnitInGroup(collision) == false && canAdd && panel.group.units.Count < 12)
        {
            panel.group.units.Add(unit);
            panel.group.SearchForUnit(true);
        }
    }
    bool AskForUnitInGroup(Collider2D collision)
    {
        foreach (Unit u in panel.group.units)
        {
            if (u.gameObject == collision.gameObject)
            {
                Debug.Log("Return true");
                return true;
            }
        }
        Debug.Log("Return false");
        return false;
    }
}

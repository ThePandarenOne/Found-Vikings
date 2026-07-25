using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public enum MovementType
{
    AnDMove,
    cursorFollow,
    clickMove
}

public class Unit : Object
{
    public enum UnitState
    {
        Idle,
        Move,
        Defend,
        Attack,
        Hunt
    }
    public MovementType movementType;
    public UnitState unitState = UnitState.Idle;

    void Start()
    {
        StartObject();
        panel = FindAnyObjectByType<Panel>();
    }
    Vector2 clickPosition = new Vector2(- 270, 270);
    void Update()
    {
        UpdateObject();
        switch(unitState)
        {
            case UnitState.Attack:
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                    if (hit == true)
                    {
                        hit.transform.gameObject.TryGetComponent(out targetUnit);
                        unitState = UnitState.Hunt;
                    }
                }
                break;
            case UnitState.Hunt:
                if (targetUnit != null && Mathf.Abs(targetUnit.transform.position.x - transform.position.x) > range)
                {
                    if (targetUnit.transform.position.x > transform.position.x)
                    {
                        transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
                    }
                    else
                    {
                        transform.position += new Vector3(-speed, 0, 0) * Time.deltaTime;
                    }
                }
                if (targetUnit != null && Mathf.Abs(targetUnit.transform.position.x - transform.position.x) < range && readyAttack)
                {
                    AttackTarget();
                }
                if(targetUnit == null)
                {
                    unitState = UnitState.Idle;
                }
                break;
            case UnitState.Move:
                switch (movementType)
                {
                    case MovementType.AnDMove:
                        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                        {
                            transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
                        }
                        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                        {
                            transform.position += new Vector3(-speed, 0, 0) * Time.deltaTime;
                        }
                        if (panel.objectUnit != this && SearchForUnitInGroup() == false)
                        {
                            unitState = UnitState.Idle;
                        }
                        break;
                    case MovementType.clickMove:
                        if (Input.GetKeyDown(KeyCode.Mouse1))
                        {
                            if (panel.objectUnit == this || SearchForUnitInGroup() == true)
                            {
                                clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            }
                        }
                        Move();
                        break;
                    case MovementType.cursorFollow:
                        clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        if (panel.objectUnit != this && SearchForUnitInGroup() == false)
                        {
                            unitState = UnitState.Idle;
                        }
                        Move();
                        break;
                }
                break;
            case UnitState.Defend:
                if(targetUnit == null)
                {
                    Collider2D[] touchableObjects = Physics2D.OverlapCircleAll(transform.position, range);
                    foreach (Collider2D touchableObject in touchableObjects)
                    {
                        if (touchableObject.TryGetComponent(out Unit unit) && unit.side != side)
                        {
                            targetUnit = unit;
                        }
                    }
                }
                if (targetUnit != null && Mathf.Abs(targetUnit.transform.position.x - transform.position.x) < range && readyAttack)
                {
                    AttackTarget();
                }
                break;
        }
    }
    public bool SearchForUnitInGroup()
    {
        if(panel.group != null)
        {
            for (byte i = 0; i < panel.group.units.Count; i++)
            {
                if (panel.group.units[i] == this)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void Move()
    {
        if (transform.position.x != clickPosition.x && clickPosition != new Vector2(-270, 270))
        {
            if (Mathf.Abs(clickPosition.x - transform.position.x) < 0.1f && movementType == MovementType.clickMove)
            {
                transform.position = new Vector3(clickPosition.x, transform.position.y);
                unitState = UnitState.Idle;
            }
            if (clickPosition.x > transform.position.x)
            {
                transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
            }
            else if (clickPosition.x < transform.position.x)
            {
                transform.position += new Vector3(-speed, 0, 0) * Time.deltaTime;
            }
        }
    }
        
    public void Attack()
    {
        unitState = UnitState.Attack;
    }
    public void AttackPosition()
    {
        unitState = UnitState.Defend;
    }
    public void ChangeMovementType(MovementType t)
    {
        movementType = t;
    }
    public void AnD()
    {
        unitState = UnitState.Move;
        movementType = MovementType.AnDMove; 
    }
    public void CursorFollow()
    {
        unitState = UnitState.Move;
        movementType = MovementType.cursorFollow;
    }
    public void ClickMovement()
    {
        clickPosition = new Vector2(-270, 270);
        unitState = UnitState.Move;
        movementType = MovementType.clickMove;
    }
}
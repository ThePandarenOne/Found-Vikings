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
        CircleCollider2D cir = GetComponent<CircleCollider2D>();
        cir.radius = range;
        panel = FindAnyObjectByType<Panel>();
    }
    Vector2 clickPosition = new Vector2(- 270, 270);
    void Update()
    {
        UpdateObject();
        if (panel.objectUnit != null && panel.objectUnit != this && unitState == UnitState.Move)
        {
            unitState = UnitState.Idle;
        }
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
                if (targetUnit != null && targetUnit.transform.position.x - transform.position.x > range)
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
                if (targetUnit.transform.position.x - transform.position.x < range && readyAttack)
                {
                    AttackTarget();
                }
                if(targetUnit == null)
                {
                    unitState = UnitState.Idle;
                }
                break;
            case UnitState.Move:
                switch(movementType)
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
                        break;
                    case MovementType.clickMove:
                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }
                        Move();
                        break;
                    case MovementType.cursorFollow:
                        clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);    
                        Move();
                        break;
                }
                break;
            case UnitState.Defend:
                if (targetUnit != null && targetUnit.transform.position.x - transform.position.x < range && readyAttack)
                {
                    AttackTarget();
                }
                break;
        }
    }
    public void Move()
    {
        if (transform.position.x != clickPosition.x && clickPosition != new Vector2(-270, 270))
        {
            if (clickPosition.x > transform.position.x)
            {
                transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
            }
            if (clickPosition.x < transform.position.x)
            {
                transform.position += new Vector3(-speed, 0, 0) * Time.deltaTime;
            }
        }
    }
        
    private void OnTriggerStay2D(Collider2D collision)
    { 
        if(unitState == UnitState.Defend && collision.TryGetComponent(out Unit unit) && unit.enemy != enemy && targetUnit == null)
        {
            targetUnit = unit;
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
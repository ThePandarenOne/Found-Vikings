using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Unit : Object
{
    public UnitState unitState = UnitState.Idle;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartObject();
        panel = FindAnyObjectByType<Panel>();
    }
    public Vector2 clickPosition = new Vector2(- 270, 270);
    void Update()
    {
        if(panel.objectUnit == this)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                unitState = UnitState.Move;
                if (panel.objectUnit == this || SearchForUnitInGroup() == true)
                {
                    clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if (hit == true && hit.transform.gameObject.TryGetComponent(out targetUnit))
                {
                    unitState = UnitState.Hunt;
                }
            }
        }
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
                        spriteRenderer.flipX = false;
                        transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
                    }
                    else
                    {
                        spriteRenderer.flipX = true;
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
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    if (panel.objectUnit == this || SearchForUnitInGroup() == true)
                    {
                        clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    }
                }
                Move();
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
            if (Mathf.Abs(clickPosition.x - transform.position.x) < 0.1f)
            {
                transform.position = new Vector3(clickPosition.x, transform.position.y);
                unitState = UnitState.Idle;
            }
            if (clickPosition.x > transform.position.x)
            {
                spriteRenderer.flipX = false;
                transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
            }
            else if (clickPosition.x < transform.position.x)
            {
                spriteRenderer.flipX = true;
                transform.position += new Vector3(-speed, 0, 0) * Time.deltaTime;
            }
        }
    }
        
    public void JumpUp()
    {
        if(transform.position.y < 7.5)
        {
            StartCoroutine(WaitForJump(+10));
        }
    }
    public void JumpDown()
    {
        if (transform.position.y > -12.5)
        {
            StartCoroutine(WaitForJump(-10));
        }
    }
    IEnumerator WaitForJump(int jumpHeigth)
    {
        yield return new WaitForSeconds(1f);
        transform.position += new Vector3(0,jumpHeigth);
    }
    public void Attack()
    {
        unitState = UnitState.Attack;
    }
    public void AttackPosition()
    {
        unitState = UnitState.Defend;
    }
    public void ClickMovement()
    {
        clickPosition = new Vector2(-270, 270);
        unitState = UnitState.Move;
    }
}
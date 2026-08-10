using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using static UnityEngine.UI.CanvasScaler;

public class Unit : Object
{
    public UnitState unitState = UnitState.Idle;
    SpriteRenderer spriteRenderer;
    bool canGoThrough = false;
    Collider2D collider;
    void Start()
    {
        collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartObject();
        panel = FindAnyObjectByType<Panel>();
    }
    public Vector2 clickPosition = new Vector2(- 270, 270);
    IEnumerator WaitForRush()
    {
        yield return new WaitForSeconds(0.1f);
        unitState = UnitState.Rush;
    }
    void Update()
    {
        //if(unitState != UnitState.Hunt)
        {
            //rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        }
        collider.isTrigger = canGoThrough;
        if (panel.objectUnit == this)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1) && unitState != UnitState.Attack)
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
                    else
                    {
                        Debug.Log("Rush");
                        clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        StartCoroutine(WaitForRush());
                    }
                }
                break;
            case UnitState.Rush:
                if (targetUnit == null)
                {
                    Collider2D[] touchableObjects = Physics2D.OverlapCircleAll(transform.position, range);
                    foreach (Collider2D touchableObject in touchableObjects)
                    {
                        if (touchableObject.TryGetComponent(out Object unit) && unit.side != side && unit.transform.position.x - transform.position.x < range)
                        {
                            targetUnit = unit;
                        }
                    }
                    Move();
                }
                if (targetUnit != null && targetUnit.transform.position.x - transform.position.x > range)
                {
                    targetUnit = null;
                }
                if (targetUnit != null && Mathf.Abs(targetUnit.transform.position.x - transform.position.x) <= range && readyAttack)
                {
                    Debug.Log(1);
                    canGoThrough = false;
                    AttackTarget();
                }
                break;
            case UnitState.Hunt:
                if (targetUnit != null && Mathf.Abs(targetUnit.transform.position.x - transform.position.x) > range)
                {
                    if (targetUnit.transform.position.x > transform.position.x)
                    {
                        canGoThrough = true;
                        spriteRenderer.flipX = false;
                        transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
                    }
                    else
                    {
                        canGoThrough = true;
                        spriteRenderer.flipX = true;
                        transform.position += new Vector3(-speed, 0, 0) * Time.deltaTime;
                    }
                }
                if (targetUnit != null && Mathf.Abs(targetUnit.transform.position.x - transform.position.x) < range && readyAttack)
                {
                    canGoThrough = false;
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
                    canGoThrough = true;
                    if (panel.objectUnit == this || SearchForUnitInGroup() == true)
                    {
                        clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    }
                }
                Move();
                break;
            case UnitState.Defend:
                canGoThrough = false;
                if (targetUnit == null)
                {
                    Collider2D[] touchableObjects = Physics2D.OverlapCircleAll(transform.position, range);
                    foreach (Collider2D touchableObject in touchableObjects)
                    {
                        if (touchableObject.TryGetComponent(out Object unit) && unit.side != side)
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
            case UnitState.Idle:
                canGoThrough = false;
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
                canGoThrough = true;
                spriteRenderer.flipX = false;
                transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
            }
            else if (clickPosition.x < transform.position.x)
            {
                canGoThrough = true;
                spriteRenderer.flipX = true;
                transform.position += new Vector3(-speed, 0, 0) * Time.deltaTime;
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Object objectt) && objectt.side != side)
        {
            canGoThrough = false;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Object objectt) && objectt.side != side)
        {
            canGoThrough = false;
        }
    }
    public void JumpUp()
    {
        if(transform.position.y < 7.5 && AskForAction("JumpUp"))
        {
            StartCoroutine(WaitForJump(+10));
        }
    }
    public void JumpDown()
    {
        if (transform.position.y > -12.5 && AskForAction("JumpDown"))
        {
            StartCoroutine(WaitForJump(-10));
        }
    }
    bool AskForAction(string name)
    {
        for (byte b = 0; b < action.Length; b++)
        {
            if (action[b] != null &&action[b].name == name)
            {
                return true;
            }
        }
        return false;
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
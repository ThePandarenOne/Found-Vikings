using UnityEngine;
using System;
using System.Collections;
using Unity.VisualScripting;
using static UnityEngine.UI.CanvasScaler;
using Unity.Netcode;

public class Unit : Object
{
    public enum TypeOfUnit
    {
        Olaf,
        Baleog,
        Eric,
        ChoGall,
        Moonshiner,
        Dragon
    }

    [Header("UNIT")]

    public TypeOfUnit typeOfUnit;
    SpriteRenderer spriteRenderer;
    bool canGoThrough = false;
    Collider2D collider_;
    void Start()
    {
        collider_ = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartObject();
        panel = FindAnyObjectByType<Panel>();
    }
    public Vector2 clickPosition = new Vector2(- 270, 270);
    IEnumerator WaitForRush()
    {
        yield return new WaitForSeconds(0.1f);
        AskForChangeUnitState(UnitState.Rush);
    }
    void ClickCommands()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && unitState != UnitState.Attack)
        {
            AskForChangeUnitState(UnitState.Move);
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
                AskForChangeUnitState(UnitState.Hunt);
            }
        }
    }
    void Update()
    {
        //if(unitState != UnitState.Hunt)
        {
            //rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        }
        collider_.isTrigger = canGoThrough;
        if (panel.objectUnit == this && playerManager.IsOwner)
        {
            ClickCommands();
        }
        UpdateObject();
        if(targetUnit != null && targetUnit.side == side)
        {
            Debug.Log("TargetUnit = null 2");
            targetUnit = null;
        }
        switch(currentState.Value)
        {
            case UnitState.Attack:
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                    if (hit == true)
                    {
                        if(hit.transform.gameObject.TryGetComponent(out Object obj) && obj.side != side)
                        {
                            targetUnit = obj;
                            AskForChangeUnitState(UnitState.Hunt);
                        }
                    }
                    else
                    {
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
                    MoveResearch();
                }
                if (targetUnit != null && targetUnit.transform.position.x - transform.position.x > range)
                {
                    Debug.Log("TargetUnit = null 3");
                    targetUnit = null;
                }
                if (targetUnit != null && Mathf.Abs(targetUnit.transform.position.x - transform.position.x) <= range)
                {
                    canGoThrough = false;
                    if (readyAttack)
                    {
                        AskForAttack();
                    }
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
                    AskForAttack();
                }
                if(targetUnit == null ||targetUnit != null&& targetUnit.hp <= 0)
                {
                    Debug.Log("No target (");
                    AskForChangeUnitState(UnitState.Idle);
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
                MoveResearch();
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
                if (targetUnit != null && Mathf.Abs(targetUnit.transform.position.x - transform.position.x) < range)
                {
                    if(readyAttack)
                    {
                        AskForAttack();
                    }
                }
                break;
            case UnitState.Idle:
                canGoThrough = false;
                break;
        }
    }
    //private NetworkVariable<Vector2> serverTargetPosition = new NetworkVariable<Vector2>(new Vector2(-270, 270));
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
    public void MoveResearch()
    {
        if(IsOwner)
        {
            //Debug.Log(gameObject.name + ": MoveResearch");
            if (playerManager.IsHost)
            {
                MoveClientRpc();
            }
            else
            {
                MoveServerRpc();
            }
        }
        else
        {
            //Debug.Log(gameObject.name +": "+ IsOwner);
        }
    }
    [ServerRpc]
    public void MoveServerRpc()
    {
        //Debug.Log(gameObject.name + ": MoveServerRpc");
        MoveClientRpc();
    }
    [ClientRpc]
    public void MoveClientRpc()
    {
        //Debug.Log(gameObject.name + ": MoveClientRpc");
        Move();
    }
    public void Move()
    {
        if (transform.position.x != clickPosition.x && clickPosition != new Vector2(-270, 270))
        {
            //Debug.Log(gameObject.name + ": Move");
            if (Mathf.Abs(clickPosition.x - transform.position.x) < 0.1f)
            {
                transform.position = new Vector3(clickPosition.x, transform.position.y);
                AskForChangeUnitState(UnitState.Idle);
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
        if(transform.position.y < 8 && AskForAction("JumpUp"))
        {
            StartCoroutine(WaitForJump(+10));
        }
    }
    public void JumpDown()
    {
        if (transform.position.y > -13 && AskForAction("JumpDown"))
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
        if(transform.position.y > -13 && transform.position.y <8 && Math.Abs(transform.position.x) < 40.5f)
        {
            if (transform.position.y + jumpHeigth < 9 && jumpHeigth + transform.position.y > -13)
            {
                transform.position += new Vector3(0, jumpHeigth);
            }
        }
    }
    public void Attack()
    {
        AskForChangeUnitState(UnitState.Attack);
    }
    public void AttackPosition()
    {
        AskForChangeUnitState(UnitState.Defend);
    }
    public void ClickMovement()
    {
        clickPosition = new Vector2(-270, 270);
        AskForChangeUnitState(UnitState.Move);
    }
}
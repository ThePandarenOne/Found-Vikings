using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;
using System;
using Unity.Netcode;

public class Object : NetworkBehaviour
{
    public enum UnitState
    {
        Idle,
        Move,
        Defend,
        Attack,
        Hunt,
        Rush
    }

    [Header("OBJECT")]

    public NetworkVariable<UnitState> currentState = new NetworkVariable<UnitState>(
    UnitState.Idle,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner
);
    public UnitState unitState = UnitState.Idle;
    public enum Side
    {
        Player,
        Enemy,
        Neutral
    }
    public PlayerManager playerManager;
    public Sprite spriteIcon;
    public Side side;
    public string objectName;
    public bool readyAttack = true;

    public int hp;
    public int maxhp;
    public NetworkVariable<int> currentHp = new NetworkVariable<int>(
    558,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner
);
    public int dmg;
    public byte range;
    public byte respawnSpeed;
    public float attackTime;
    public float speed;

    public ActionData[] action;

    //protected Rigidbody2D rb;
    public GameObject selectArrow;
    public Slider hpBar;
    public Panel panel;
    public Object targetUnit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(IsOwner)
        {
            currentHp.Value = hp;
        }
        if (playerManager.IsServer == false)
        {
            StartObject();
        }
        panel = FindAnyObjectByType<Panel>();
    }
    public void StartObject()
    {
        if (playerManager != null && playerManager.IsServer == false)
        {
            //playerManager.UpdateUnitOwnerServerRpc(GetComponent<NetworkObject>());
        }
        gameObject.name = objectName;
    }
    public void OnMouseDown()
    {
        ChooseUnit(false);
    }

    public void ChooseUnit(bool d)
    {
        //Debug.Log("IsOwner"+IsOwner);
        //Debug.Log("OwnerId"+OwnerClientId);
        //Debug.Log("ChooseUnit");
        panel = FindAnyObjectByType<Panel>();
        if (panel.group != null && !Input.GetKey(KeyCode.LeftControl) || d == true)
        {
            Destroy(panel.group.gameObject);
        }
        panel.objectUnit = this;
        if(GetComponent<Unit>() && Input.GetKey(KeyCode.LeftControl) && panel.group != null)
        {
            panel.ChangePanel();
        }
        else if(!Input.GetKey(KeyCode.LeftControl))
        {
            panel.ChangePanel();
        }
    }

    GameObject gm;
    // Update is called once per frame
    [ServerRpc(RequireOwnership = false)]
    void UpdateHPServerRpc()
    {
        UpdateHPClientRpc();
    }
    [ClientRpc]
    void UpdateHPClientRpc()
    {
        if(IsOwner)
        {
            currentHp.Value = hp;
        }
    }
    public void UpdateObject()
    {
        if(IsSpawned == false)
        {
            return;
        }
        if(IsOwner&&currentHp.Value != hp)
        {
            UpdateHPServerRpc();
        }
        if(currentState.Value != unitState && IsOwner)
        {
            currentState.Value = unitState;
        }
        if (playerManager.sidePlayer == side)
        {
            playerManager.UpdateUnitOwnerServerRpc(GetComponent<NetworkObject>());
        }
        if(panel == null)
        {
            panel = FindFirstObjectByType<Panel>();
        }
        if (playerManager.IsServer == false)
        {
            playerManager.UpdateUnitOwnerServerRpc(GetComponent<NetworkObject>());
        }
        if (gm == null && selectArrow != null)
        {
            if (panel.objectUnit == this || TryGetComponent(out Unit unit) && unit.SearchForUnitInGroup() == true)
            {
                gm = Instantiate(selectArrow, new Vector2(transform.position.x, transform.position.y + 2f), transform.rotation, transform);
                selectArrow.gameObject.SetActive(true);
            }
        }
        else
        {
            if (panel.objectUnit != this && TryGetComponent(out Unit unit) && unit.SearchForUnitInGroup() == false)
            {
                Destroy(gm);
            }
            if (panel.objectUnit != this && !GetComponent<Unit>())
            {
                Destroy(gm);
            }
        }
        if (hpBar != null)
        {
            hpBar.value = currentHp.Value;
            hpBar.maxValue = maxhp;
        }
        if (hp <= 0 && GetComponent<Unit>())
        {
            GetComponent<NetworkObject>().Despawn();
            if(IsHost)
            {
                DestroyClientRpc();
            }
            else
            {
                DestroyServerRpc();
            }
        }
    }
    public void AskForAttack()
    {
        if(readyAttack)
        {
            //Debug.Log("ReadyAttack " + readyAttack);
            readyAttack = false;
            //Debug.Log("AskforAttack");
            AttackServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)] void AttackServerRpc()
    {
        readyAttack = false;
        //Debug.Log("AttackServerRpc");
        AttackClientRpc();
    }
    [ClientRpc] void AttackClientRpc()
    {
        readyAttack = false;
        //Debug.Log("AttackClientRpc");
        AttackTarget();
    }
    void AttackTarget()
    {
        readyAttack = false;
        //Debug.Log("AttackTarget");
        StartCoroutine(WaitForAttack());
        //Debug.Log(1);
        //rb.constraints = RigidbodyConstraints2D.FreezePosition;
        if (targetUnit.side == side && targetUnit.IsOwner != IsOwner)
        {
            //Debug.Log("TargetUnit = null 1");
            targetUnit = null;
        }
        if (targetUnit != null && transform.position.y - targetUnit.transform.position.y < range)
        {
            //Debug.Log(2);
            if(IsHost)
            {
                targetUnit.GetDamageServerRpc(dmg);
            }
            readyAttack = false;
            if(GetComponent<Unit>() && GetComponent<Unit>().typeOfUnit == Unit.TypeOfUnit.Olaf)
            {
                targetUnit.targetUnit = this;
            }
            if (targetUnit.hp <= 0)
            {
                //if (targetUnit.TryGetComponent(out BuildPlace buildPlace))
                {
                    //buildPlace.playerManager = playerManager;
                    //Debug.Log("1 Check");
                    //buildPlace.AskForSideChange(side);
                }
                //else if (targetUnit.TryGetComponent(out Building building))
                {
                    //building.SpawnBuildingPlacementServerRpc(side);
                    //building.buildPlace.playerManager = playerManager;
                    //Debug.Log("2 Check");
                    //building.buildPlace.AskForSideChange(side);
                }
                if(targetUnit.TryGetComponent(out LineObjective lineObjective))
                {
                    lineObjective.playerManager = playerManager;
                    lineObjective.SideChange(side);
                }
                //panel.ChangePanel();
            }
        }
    }
    IEnumerator WaitForAttack()
    {
        yield return new WaitForSeconds(attackTime);
        readyAttack = true;
    }
    public void GetDamage(int damage)
    {
        //Debug.Log(5);
        hp -= damage;
        //currentHp.Value -= damage;
    }
    [ServerRpc(RequireOwnership = false)]
    void GetDamageServerRpc(int dmg)
    {
        //Debug.Log(3);
        GetDamageClientRpc(dmg);
    }
    [ClientRpc]
    void GetDamageClientRpc(int dmg)
    {
        //Debug.Log(4);
        GetDamage(dmg);
    }

    //Destroy

    [ClientRpc]
    protected void DestroyClientRpc()
    {
        Destroy(gameObject);
    }
    [ServerRpc(RequireOwnership = false)]
    protected void DestroyServerRpc()
    {
        DestroyClientRpc();
    }

    //Unit state

    public void AskForChangeUnitState(UnitState unitStatee)
    {
        //Debug.Log(unitStatee);
        ChangeUnitStateServerRpc(unitStatee);
    }
    [ServerRpc(RequireOwnership = false)] void ChangeUnitStateServerRpc(UnitState unitStatee)
    {
        ChangeUnitStateClientRpc(unitStatee);
    }
    [ClientRpc]
    void ChangeUnitStateClientRpc(UnitState unitStatee)
    {
        ChangeUnitState(unitStatee);
    }
    void ChangeUnitState(UnitState unitStatee)
    {
        unitState = unitStatee;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;
using System;
using Unity.Netcode;
using static LineObjective;
using Unity.VisualScripting;

public class Entity : NetworkBehaviour
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
    NetworkVariableWritePermission.Server
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
    NetworkVariableWritePermission.Server
);
    public int dmg;
    public byte range;
    public byte respawnSpeed;
    public float attackTime;
    public float speed;
    public bool isHealer;
    public ActionData[] action;

    //protected Rigidbody2D rb;
    public GameObject selectArrow;
    public Slider hpBar;
    public Panel panel;
    public Entity targetUnit;

    protected double timerCooldown;

    void Start()
    {
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
        if (IsSpawned == false)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                GetComponent<NetworkObject>().Spawn();
            }
        }
        gameObject.name = objectName;
    }
    public void OnMouseDown()
    {
        ChooseUnit(false);
    }

    public void ChooseUnit(bool d)
    {
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
    [ServerRpc(RequireOwnership = false)]
    void UpdateHPServerRpc()
    {
        UpdateHPClientRpc();
    }
    [ClientRpc]
    void UpdateHPClientRpc()
    {
        if(IsHost)
        {
            currentHp.Value = hp;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdatecurrentStateServerRpc()
    {
        UpdatecurrentStateClientRpc();
    }
    [ClientRpc]
    void UpdatecurrentStateClientRpc()
    {
        if (IsHost)
        {
            currentState.Value = unitState;
        }
    }

    public void UpdateObject()
    {
        if(hp > maxhp)
        {
            hp = maxhp;
        }
        if (IsSpawned == false)
        {
            return;
        }
        if (NetworkManager.Singleton.ServerTime.Time > timerCooldown)
        {
            if(!GetComponent<Building>()||TryGetComponent(out Building building) && building.typeOfBuilding != Building.TypeOfBuilding.Mine)
            {
                //Debug.Log("Time reach cooldown");
                timerCooldown = NetworkManager.Singleton.ServerTime.Time + attackTime;
                if (readyAttack == false)
                {
                    //Debug.Log("Ready attack true again");
                    readyAttack = true;
                }
            }
        }
        if (playerManager == null)
        {
            playerManager = FindObjectsByType<PlayerManager>(FindObjectsSortMode.None).FirstOrDefault(m => m.sidePlayer == side);
        }
        else
        {
            playerManager.UpdateUnitOwnerServerRpc(GetComponent<NetworkObject>());
        }

        if (panel == null)
        {
            panel = FindFirstObjectByType<Panel>();
        }

        if (IsOwner&&currentHp.Value != hp)
        {
            UpdateHPServerRpc();
        }
        if(currentState.Value != unitState && IsOwner)
        {
            UpdatecurrentStateServerRpc();
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
            if(panel.objectUnit != this)
            {
                if (TryGetComponent(out Unit unit) && unit.SearchForUnitInGroup() == false)
                {
                    Destroy(gm);
                }
                if (!GetComponent<Unit>())
                {
                    Destroy(gm);
                }
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

    // ATTACK

    public void AskForAttack()
    {
        //Debug.Log("AskForAttack1");
        if (readyAttack == false)
        {
            return;
        }
        if(readyAttack)
        {
            //Debug.Log("AskForAttack2");
            readyAttack = false;
            if (IsHost)
            {
                AttackClientRpc();
            }
            else
            {
                AttackServerRpc();
            }
        }
    }
    [ServerRpc(RequireOwnership = false)] void AttackServerRpc()
    {
        AttackClientRpc();
    }
    [ClientRpc] void AttackClientRpc()
    {
        AttackTarget();
    }
    void AttackTarget()
    {
        readyAttack = false;
        timerCooldown = NetworkManager.Singleton.ServerTime.Time + attackTime;
        if (targetUnit != null)
        {
            if (isHealer && targetUnit.side != side && targetUnit.GetComponent<Unit>() || !isHealer && targetUnit.side == side)
            {
                targetUnit = null;
                return;
            }
        }
        if (targetUnit != null && Math.Abs(targetUnit.transform.position.y - transform.position.y) < range)
        {
            if(IsHost)
            {
                targetUnit.GetDamageClientRpc(dmg,isHealer);
            }
            else
            {
                targetUnit.GetDamageServerRpc(dmg,isHealer);
            }
            readyAttack = false;
            if(GetComponent<Unit>() && GetComponent<Unit>().typeOfUnit == Unit.TypeOfUnit.Olaf)
            {
                //targetUnit.targetUnit = this;
            }
            if (targetUnit.hp <= 0)
            {
                if(targetUnit.TryGetComponent(out LineObjective lineObjective))
                {
                    lineObjective.playerManager = playerManager;
                    lineObjective.AskForSideChange(side);
                }
            }
        }
    }
    public void GetDamage(int damage)
    {
        hp -= damage;
    }
    public void GetHeal(int damage)
    {
        hp += damage;
    }
    [ServerRpc(RequireOwnership = false)]
    void GetDamageServerRpc(int dmg, bool isHeal)
    {
        GetDamageClientRpc(dmg, isHeal);
    }
    [ClientRpc]
    void GetDamageClientRpc(int dmg, bool isHeal)
    {
        if(isHeal)
        {
            GetHeal(dmg);
        }
        else
        {
            GetDamage(dmg);
        }

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

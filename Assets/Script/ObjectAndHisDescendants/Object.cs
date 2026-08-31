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

    public ActionData[] action;

    //protected Rigidbody2D rb;
    public GameObject selectArrow;
    public Slider hpBar;
    public Panel panel;
    public Entity targetUnit;

    protected double timerCooldown;


    [ServerRpc(RequireOwnership = false)]protected void ReadyAttackCheckServerRpc()
    {
        if (readyAttack == false)
        {
            Debug.Log("Ready attack true again");
            ReadyAttackCheckClientRpc();
        }
    }
    [ClientRpc]void ReadyAttackCheckClientRpc()
    {
        Debug.Log("Ready attack for client");
        readyAttack = true;
    }
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
            //Debug.LogError(gameObject.name + ": Object isn't spawned!");
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
        if (IsSpawned == false)
        {
            return;
        }
        if (NetworkManager.Singleton.ServerTime.Time > timerCooldown && !GetComponent<Building>())
        {
            //Debug.Log("Time reach cooldown");
            timerCooldown = NetworkManager.Singleton.ServerTime.Time + attackTime;
            if (readyAttack == false)
            {
                //Debug.Log("Ready attack true again");
                ReadyAttackCheckServerRpc();
            }
        }
        //ReadyAttackCheckServerRpc();
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
        Debug.Log("AttackTarget");
        readyAttack = false;
        timerCooldown = NetworkManager.Singleton.ServerTime.Time + attackTime;
        //rb.constraints = RigidbodyConstraints2D.FreezePosition;
        if (targetUnit != null&&targetUnit.side == side)
        {
            Debug.Log("TargetUnit = null 1");
            targetUnit = null;
        }
        if (targetUnit != null && Math.Abs(targetUnit.transform.position.y - transform.position.y) < range)
        {
            if(IsHost)
            {
                Debug.Log("IsHostAttack");
                targetUnit.GetDamageClientRpc(dmg);
            }
            else
            {
                Debug.Log("IsClientAttack");
                targetUnit.GetDamageServerRpc(dmg);
            }
            readyAttack = false;
            if(GetComponent<Unit>() && GetComponent<Unit>().typeOfUnit == Unit.TypeOfUnit.Olaf)
            {
                //targetUnit.targetUnit = this;
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
                    lineObjective.AskForSideChange(side);
                }
                //panel.ChangePanel();
            }
        }
    }
    public void GetDamage(int damage)
    {
        hp -= damage;
    }
    [ServerRpc(RequireOwnership = false)]
    void GetDamageServerRpc(int dmg)
    {
        GetDamageClientRpc(dmg);
    }
    [ClientRpc]
    void GetDamageClientRpc(int dmg)
    {
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

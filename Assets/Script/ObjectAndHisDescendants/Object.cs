using UnityEngine;
using System.Collections;
using static UnityEngine.Rendering.DebugUI;

public class Object : MonoBehaviour
{
    public enum UnitState
    {
        Idle,
        Move,
        Defend,
        Attack,
        Hunt
    }
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
    public int dmg;
    public byte range;
    public byte respawnSpeed;
    public float attackTime;
    public float speed;

    public ActionData[] action;

    public Panel panel;
    public Object targetUnit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panel = FindAnyObjectByType<Panel>();
    }
    public void StartObject()
    {
        gameObject.name = objectName;
    }
    public void OnMouseDown()
    {
        ChooseUnit(false);
    }
    public void ChooseUnit(bool d)
    {
        panel = FindAnyObjectByType<Panel>();
        if (panel.group != null || d == true)
        {
            Destroy(panel.group.gameObject);
            Destroy(panel.group);
            panel.group = null;
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

    // Update is called once per frame
    public void UpdateObject()
    {
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void GetDamage(int damage)
    {
        hp -= damage;
    }
    public void AttackTarget()
    {
        if (targetUnit.side == side)
        {
            targetUnit = null;
        }
        if (targetUnit != null)
        {
            targetUnit.GetDamage(dmg);
            readyAttack = false;
            if (targetUnit.hp <= 0)
            {
                if (targetUnit.TryGetComponent(out BuildPlace buildPlace))
                {
                    buildPlace.playerManager = playerManager;
                    buildPlace.SideChange(side);
                }
                else if (targetUnit.TryGetComponent(out Building building))
                {
                    building.buildPlace.playerManager = playerManager;
                    building.buildPlace.SideChange(side);
                }
                else if(targetUnit.TryGetComponent(out LineObjective lineObjective))
                {
                    lineObjective.playerManager = playerManager;
                    lineObjective.SideChange(side);
                }
                //panel.ChangePanel();
            }
        }
        StartCoroutine(WaitForAttack());
    }
    IEnumerator WaitForAttack()
    {
        yield return new WaitForSeconds(attackTime);
        readyAttack = true;
    }
}

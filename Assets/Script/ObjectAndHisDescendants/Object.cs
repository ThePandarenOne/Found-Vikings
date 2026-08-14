using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;

public class Object : MonoBehaviour
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

    //protected Rigidbody2D rb;
    public GameObject selectArrow;
    public Slider hpBar;
    public Panel panel;
    public Object targetUnit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
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
    public void UpdateObject()
    {
        if (gm == null)
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
            hpBar.value = hp;
            hpBar.maxValue = maxhp;
        }
        if (hp <= 0 && GetComponent<Unit>())
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
        //rb.constraints = RigidbodyConstraints2D.FreezePosition;
        if (targetUnit.side == side)
        {
            targetUnit = null;
        }
        if (targetUnit != null && transform.position.y - targetUnit.transform.position.y < range)
        {
            targetUnit.GetDamage(dmg);
            readyAttack = false;
            if(GetComponent<Unit>().typeOfUnit == Unit.TypeOfUnit.Olaf)
            {
                targetUnit.targetUnit = this;
            }
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

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
    public Unit targetUnit;
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
        if (panel.group != null && !Input.GetKey(KeyCode.LeftControl) || d == true)
        {
            Destroy(panel.group.gameObject);
            Destroy(panel.group);
            panel.group = null;
        }
        panel.objectUnit = this;
        panel.ChangePanel();
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
        targetUnit.GetDamage(dmg);
        readyAttack = false;
        StartCoroutine(WaitForAttack());
    }
    IEnumerator WaitForAttack()
    {
        yield return new WaitForSeconds(attackTime);
        readyAttack = true;
    }
}

using UnityEngine;
using System.Collections;
using static UnityEngine.Rendering.DebugUI;

public class Object : MonoBehaviour
{
    public bool enemy;
    public bool readyAttack = true;

    public int hp;
    public int maxhp;
    public int dmg;
    public byte range;
    public byte respawnSpeed;
    public float attackspeed;
    public float speed;

    public ActionData[] action;

    public Panel panel;
    public Unit targetUnit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void OnMouseDown()
    {
        panel = FindAnyObjectByType<Panel>();
        Destroy(panel.group);
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
        yield return new WaitForSeconds(attackspeed);
        readyAttack = true;
    }
}

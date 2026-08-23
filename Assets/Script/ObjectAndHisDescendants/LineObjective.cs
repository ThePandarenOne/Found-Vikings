using UnityEngine;
using System.Collections;
public class LineObjective : Object
{
    public enum TypeOfObjective
    {
        ChoGallCave,
        DragonAltar,
        Mine,
        Grill
    }

    [Header("LINEOBJECTIVE")]

    public Sprite spriteBrown;
    public Sprite spriteOrange;
    public TypeOfObjective typeOfObjective;
    bool can = true;
    bool canSpawnChoGall = false;
    public Line line;
    public Unit unitBrown;
    public Unit unitOrange;
    SpriteRenderer sr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartObject();
        sr = GetComponent<SpriteRenderer>();
        if(typeOfObjective == TypeOfObjective.DragonAltar)
        {
            StartCoroutine(WaitForDragonReload());
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateObject();
        switch (typeOfObjective)
        {
            case TypeOfObjective.DragonAltar:
                if (canSpawnChoGall && side != Side.Neutral)
                {
                    GiveDragon(side);
                }
                else if(canSpawnChoGall == false && can)
                {
                    can = false;
                    StartCoroutine(WaitForDragonReload());
                }
                break;
            case TypeOfObjective.Grill:
                break;
            case TypeOfObjective.Mine:
                if (can && playerManager != null)
                {
                    StartCoroutine(GiveMoney(3));
                }
                break;
        }
    }
    IEnumerator GiveMoney(int count)
    {
        can = false;
        yield return new WaitForSeconds(1f);
        playerManager.money += count;
        can = true;
    }
    public void SideChange(Side sidee)
    {
        hp = maxhp;
        side = sidee;
        if (sidee == Side.Player)
        {
            sr.sprite = spriteOrange;
        }
        else if (sidee == Side.Enemy)
        {
            sr.sprite = spriteBrown;
        }
        if (typeOfObjective == TypeOfObjective.DragonAltar)
        {
            if (canSpawnChoGall)
            {
                GiveDragon(sidee);
            }
        }
    }
    public void SpawnUnit()
    {
        if(side == Side.Player)
        {

        }
        else if (side == Side.Enemy)
        {

        }
    }
    IEnumerator WaitForSpawn(Unit unit)
    {
        yield return new WaitForSeconds(unit.respawnSpeed);
    }
    void GiveDragon(Side side)
    {
        if (side == Side.Player)
        {
            Unit unit = Instantiate(unitOrange, new Vector2(transform.position.x, transform.position.y), transform.rotation);
            unit.playerManager = line.playerManager;
        }
        else if (side == Side.Enemy)
        {
            Unit unit = Instantiate(unitBrown, new Vector2(transform.position.x, transform.position.y), transform.rotation);
            unit.playerManager = line.playerManager;
        }
        canSpawnChoGall = false;
    }
    IEnumerator WaitForDragonReload()
    {
        yield return new WaitForSeconds(60f);
        can = true;
        canSpawnChoGall = true;
    }
}

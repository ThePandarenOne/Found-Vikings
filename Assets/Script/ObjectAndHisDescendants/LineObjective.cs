using UnityEngine;
using System.Collections;
public class LineObjective : Object
{
    public enum TypeOfObjective
    {
        ChoGall,
        Mine,
        Grill
    }
    public Sprite spriteBrown;
    public Sprite spriteOrange;
    public TypeOfObjective typeOfObjective;
    bool can = true;
    bool canSpawnChoGall = false;
    public Line line;
    public Unit choGallBrown;
    public Unit choGallOrange;
    SpriteRenderer sr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if(typeOfObjective == TypeOfObjective.ChoGall)
        {
            StartCoroutine(WaitForChoGallReload());
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (typeOfObjective)
        {
            case TypeOfObjective.ChoGall:
                if (canSpawnChoGall && side != Side.Neutral)
                {
                    GiveChoGall(side);
                }
                else if(canSpawnChoGall == false && can)
                {
                    can = false;
                    StartCoroutine(WaitForChoGallReload());
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
        if (typeOfObjective == TypeOfObjective.ChoGall)
        {
            if (canSpawnChoGall)
            {
                GiveChoGall(sidee);
            }
        }
    }
    void GiveChoGall(Side side)
    {
        if (side == Side.Player)
        {
            Unit unit = Instantiate(choGallOrange, new Vector2(transform.position.x, transform.position.y), transform.rotation);
            unit.playerManager = line.playerManager;
        }
        else if (side == Side.Enemy)
        {
            Unit unit = Instantiate(choGallBrown, new Vector2(transform.position.x, transform.position.y), transform.rotation);
            unit.playerManager = line.playerManager;
        }
        canSpawnChoGall = false;
    }
    IEnumerator WaitForChoGallReload()
    {
        yield return new WaitForSeconds(60f);
        can = true;
        canSpawnChoGall = true;
    }
}

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (can && playerManager != null && typeOfObjective == TypeOfObjective.Mine)
        {
            StartCoroutine(GiveMoney(3));
        }
        switch (typeOfObjective)
        {
            case TypeOfObjective.Grill:
                break;
            case TypeOfObjective.Mine:
                if(can && playerManager != null)
                {
                   // StartCoroutine(GiveMoney(3));
                }
                break;
        }
    }
    IEnumerator GiveMoney(int count)
    {
        Debug.Log("S");
        can = false;
        yield return new WaitForSeconds(1f);
        playerManager.money += count;
        can = true;
    }
    public void SideChange(Side sidee)
    {
        hp = maxhp;
        side = sidee;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sidee == Side.Player)
        {
            sr.sprite = spriteOrange;
        }
        else if (sidee == Side.Enemy)
        {
            sr.sprite = spriteBrown;
        }
    }
}

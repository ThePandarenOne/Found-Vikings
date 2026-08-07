using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour
{
    public enum LineType
    {
        Vinland,
        Japan,
        Kazakhstan,
        ChoGalia
    }
    public Unit choGall;
    public LineType lineType;
    bool canSpawnChoGall;

    public int playerScore;
    public int enemyScore;

    public Unit[] unitsBrown;
    public Unit[] unitsOrange;

    public PlayerManager playerManager;
    public PlayerManager enemyManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(lineType == LineType.ChoGalia)
        {
            StartCoroutine(WaitForChoGallReload());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lineType == LineType.ChoGalia)
        {
            if (canSpawnChoGall)
            {
                if (playerScore == 10)
                {
                    GiveChoGall(Object.Side.Player);
                }
                else if (enemyScore == 10)
                {
                    GiveChoGall(Object.Side.Enemy);
                }
            }
        }
    }
    IEnumerator WaitForChoGallReload()
    {
        yield return new WaitForSeconds(60f);
        canSpawnChoGall = true;
    }
    void GiveChoGall(Object.Side side)
    {
        canSpawnChoGall = false;
        Unit unit = Instantiate(choGall,transform.position,transform.rotation);
        unit.side = side;
        if(side == Object.Side.Player)
        {
            unit.playerManager = playerManager;
        }
        else if(side == Object.Side.Enemy)
        {
            unit.playerManager = enemyManager;
        }
        StartCoroutine(WaitForChoGallReload());
    }
}

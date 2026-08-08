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
    public LineType lineType;

    public Unit[] unitsBrown;
    public Unit[] unitsOrange;

    public PlayerManager playerManager;
    public PlayerManager enemyManager;
    public LineObjective lineObjective;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(lineType == LineType.ChoGalia)
        {
            //StartCoroutine(WaitForChoGallReload());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lineType == LineType.ChoGalia)
        {
        }
    }
}

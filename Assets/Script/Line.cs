using UnityEngine;

public class Line : MonoBehaviour
{
    public enum LineType
    {
        Vinland,
        Japanese,
        Kazakhstan
    }
    public Building objective;
    public LineType lineType;
    public BuildPlace[] bp;
    public Unit[] unitsBrown;
    public Unit[] unitsOrange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(lineType)
        {
            case LineType.Vinland:
                break;
            case LineType.Japanese:
                break;
            case LineType.Kazakhstan:
                break;
        }
    }
    void VinlandObjective()
    {

    }
}

using UnityEngine;

public class LineSpawner : MonoBehaviour
{
    public GameObject[] lineSpawners;
    public GameObject[] lines;
    int i = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnLine();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void SpawnLine()
    {
        int saverCount = 0;
        if(lines.Length < 3)
        {
            Debug.LogError("Less than 4 lines");
            return;
        }
        if(lineSpawners.Length < 3)
        {
            Debug.LogError("Less than 3 lineSpanwers");
            return;
        }
        int firstLoop = -1;
        int secondLoop = -1;
        for (byte b = 0; b < lineSpawners.Length; b++)
        {
            
            i = Random.Range(0, lines.Length);
            while (i == firstLoop || i == secondLoop)
            {
                saverCount++;
                if(saverCount>= 100)
                {
                    Debug.LogError("ToMuchRandom");
                    return;
                }
                i = Random.Range(0, lines.Length);
            }
            saverCount = 0;
            lines[i].transform.parent = lineSpawners[b].transform;
            lines[i].transform.position = lineSpawners[b].transform.position;
            lines[i].gameObject.SetActive(true);
            
            if (b == 0)
            {
                firstLoop = i;
            }
            else if (b == 1)
            {
                secondLoop = i;
            }
        }
    }
}

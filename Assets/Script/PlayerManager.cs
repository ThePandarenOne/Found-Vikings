using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Object.Side sidePlayer;
    public PlayerManager enemyManager;
    public int money;
    public Building main;
    public bool yellowBeard;
    public Panel panel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(main.hp <=0)
        {
            Defeat();
        }
        else if(enemyManager.main.hp <= 0)
        {
            Victory();
        }
    }
    public void Defeat()
    {
        Debug.Log("Defeat");
    }
    public void Victory()
    {
        Debug.Log("Victory");
    }
}

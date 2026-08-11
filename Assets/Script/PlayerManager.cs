using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public string nameOfFaction;
    public Object.Side sidePlayer;
    public PlayerManager enemyManager;
    public int money;
    public Building main;
    public Panel panel;

    UIManager manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = FindAnyObjectByType<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(money < 0)
        {
            money = 0;
        }
        if(main.hp <=0)
        {
            manager.VictoryMenu(enemyManager.nameOfFaction);
        }
        else if(enemyManager.main.hp <= 0)
        {
            manager.VictoryMenu(nameOfFaction);
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

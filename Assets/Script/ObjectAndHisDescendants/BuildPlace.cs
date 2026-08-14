using UnityEngine;
using System.Collections;
using static UnityEngine.Rendering.DebugUI;
using System.Linq;

public class BuildPlace : Object
{
    public int timer;
    public bool isBuilding = false;
    public Building[] buildings;
    public Building building;
    public Sprite spriteBrown;
    public Sprite spriteOrange;
    public void BuildSpawner()
    {
        if (isBuilding == false)
        {
            building = buildings[0];
            StartCoroutine(Building());
        }
    }
    public void BuildMine()
    {
        if (isBuilding == false)
        {
            building = buildings[1];
            StartCoroutine(Building());
        }
    }
    public void BuildTower()
    {
        if (isBuilding == false)
        {
            building = buildings[2];
            StartCoroutine(Building());
        }
    }
    IEnumerator Building()
    {
        isBuilding = true;
        for (int b = building.respawnSpeed; b >= 0; b--)
        {
            timer = b;
            yield return new WaitForSeconds(1f);
            if (b <= 0)
            {
                isBuilding = false;
                panel.UpdateUnitsIconsWhileBuilding(this);
                Building spawnedBuilding = Instantiate(building, transform.position, transform.rotation);
                spawnedBuilding.buildPlace = this;
                spawnedBuilding.playerManager = playerManager;
                panel.objectUnit = spawnedBuilding;
                panel.ChangePanel();
                building = null;
                gameObject.SetActive(false);
            }
        }
    }
    public void SideChange(Side sidee)
    {
        hp = maxhp;
        side = sidee;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if(sidee == Side.Player)
        {
            sr.sprite = spriteOrange;
        }
        else if(sidee == Side.Enemy)
        {
            sr.sprite = spriteBrown;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerManager == null)
        {
            playerManager = FindObjectsByType<PlayerManager>(FindObjectsSortMode.None).FirstOrDefault(pm => pm.sidePlayer == side);
        }
    }
}

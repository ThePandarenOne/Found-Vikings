using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class LineObjective : Building
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
        if (typeOfObjective == TypeOfObjective.DragonAltar)
        {
            timerCooldown = respawnSpeed;
        }
        StartObject();
        sr = GetComponent<SpriteRenderer>();
        if(typeOfObjective == TypeOfObjective.DragonAltar)
        {
            DragonTimeCheckServerRpc(respawnSpeed);
        }
    }
    [ServerRpc]
    void DragonTimeCheckServerRpc(float cooldown)
    {
        if (NetworkManager.Singleton.ServerTime.Time > timerCooldown)
        {
            if(typeOfObjective == TypeOfObjective.DragonAltar)
            {
                timerCooldown = Mathf.Ceil(((float)NetworkManager.Singleton.ServerTime.Time) / respawnSpeed);
                timerCooldown *= respawnSpeed;
                can = false;
                canSpawnChoGall = true;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (spawnplace == null)
        {
            spawnplace = gameObject;
        }
        UpdateObject();
        switch (typeOfObjective)
        {
            case TypeOfObjective.DragonAltar:
                DragonTimeCheckServerRpc(respawnSpeed);
                if (canSpawnChoGall && side != Side.Neutral && IsHost)
                {
                    canSpawnChoGall = false;
                    AskForGiveDragon();
                }
                else if(canSpawnChoGall == false && can)
                {
                    can = false;

                }
                break;
            case TypeOfObjective.Mine:
                if (can && side != Side.Neutral)
                {
                    EarnMoney(3);
                }
                break;
        }
    }
    /*
    [ServerRpc]protected override void ReadyGiveMoneyCheckServerRpc(int count)
    {
        if (NetworkManager.Singleton.ServerTime.Time > timerCooldown)
        {
            if (readyAttack == false)
            {
                playerManager.money += count;
                readyAttack = true;
            }
        }
    }
    
    IEnumerator GiveMoney(int count)
    {
        can = false;
        yield return new WaitForSeconds(1f);
        playerManager.money += count;
        can = true;
    }
    */

    //SIDE CHANGE

    public void AskForSideChange(Side sidee)
    {
        if(IsHost)
        {
            SideChangeClientRpc(sidee);
        }
        else
        {
            SideChangeServerRpc(sidee);
        }
    }
    [ServerRpc(RequireOwnership = false)] void SideChangeServerRpc(Side sidee)
    {
        SideChangeClientRpc(sidee);
    }
    [ClientRpc]
    void SideChangeClientRpc(Side sidee)
    {
        SideChange(sidee);
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
    }

    // UNIT SPAWN
    
    public override void AddUnitToQueue(byte NameOfUnit)
    {
        if (unitQueue.Count < 5)
        {
            unitQueue.Add(NameOfUnit);
            if (canSpawn)
            {
                canSpawn = false;
                if(side == Side.Player)
                {
                    StartCoroutine(WaitForSpawn(unitOrange));
                }
                else if(side == Side.Enemy)
                {
                    StartCoroutine(WaitForSpawn(unitBrown));
                }
            }
        }
    }
    
    public void SpawnUnit()
    {
        if (side == Side.Player)
        {
            Unit unit = Instantiate(unitOrange, new Vector2(transform.position.x, transform.position.y), transform.rotation);
            unit.GetComponent<NetworkObject>().SpawnWithOwnership(0);
            unit.playerManager = line.playerManager;
        }
        else if (side == Side.Enemy)
        {
            Unit unit = Instantiate(unitBrown, new Vector2(transform.position.x, transform.position.y), transform.rotation);
            unit.GetComponent<NetworkObject>().SpawnWithOwnership(1);
            unit.playerManager = line.playerManager;
        }
    }

    // DRAGON OBJECTIVE

    public void AskForGiveDragon()
    {
        if (IsHost)
        {
            GiveDragonClientRpc();
        }
        else
        {
            GiveDragonServerRpc();
        }
    }

    [ServerRpc]
    void GiveDragonServerRpc()
    {
        GiveDragonClientRpc();
    }
    [ClientRpc]
    void GiveDragonClientRpc()
    {
        GiveDragon(side);
    }

    void GiveDragon(Side side)
    {
        if(IsHost)
        {
            if (side == Side.Player)
            {
                Unit unit = Instantiate(unitOrange, new Vector2(transform.position.x, transform.position.y), transform.rotation);
                if (IsHost)
                {
                    unit.GetComponent<NetworkObject>().SpawnWithOwnership(0);
                }
                unit.playerManager = line.playerManager;
                if (line.playerManager == null)
                {
                    unit.playerManager = playerManager;
                }
            }
            else if (side == Side.Enemy)
            {
                Unit unit = Instantiate(unitBrown, new Vector2(transform.position.x, transform.position.y), transform.rotation);
                if (IsHost)
                {
                    unit.GetComponent<NetworkObject>().SpawnWithOwnership(1);
                }
                unit.playerManager = line.playerManager;
                if (line.playerManager == null)
                {
                    unit.playerManager = playerManager;
                }
            }
            canSpawnChoGall = false;
        }
    }
}

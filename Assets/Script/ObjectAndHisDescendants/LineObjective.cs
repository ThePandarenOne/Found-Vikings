using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class LineObjective : Object
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
        StartObject();
        sr = GetComponent<SpriteRenderer>();
        if(typeOfObjective == TypeOfObjective.DragonAltar)
        {
            StartCoroutine(WaitForDragonReload());
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateObject();
        switch (typeOfObjective)
        {
            case TypeOfObjective.DragonAltar:
                if (canSpawnChoGall && side != Side.Neutral)
                {
                    AskForGiveDragon();
                }
                else if(canSpawnChoGall == false && can)
                {
                    can = false;
                    StartCoroutine(WaitForDragonReload());
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
    [ServerRpc] void SideChangeServerRpc(Side sidee)
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
        if (typeOfObjective == TypeOfObjective.DragonAltar)
        {
            if (canSpawnChoGall)
            {
                GiveDragon(sidee);
            }
        }
    }

    // UNIT SPAWN

    public void AskForUnitSpawn()
    {
        if(IsHost)
        {
            SpawnUnitClientRpc();
        }
        else
        {
            SpawnUnitServerRpc();
        }
    }

    [ServerRpc] void SpawnUnitServerRpc()
    {
        SpawnUnitClientRpc();
    }
    [ClientRpc]
    void SpawnUnitClientRpc()
    {
        SpawnUnit();
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
    IEnumerator WaitForSpawn(Unit unit)
    {
        yield return new WaitForSeconds(unit.respawnSpeed);
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
        SpawnUnit();
    }

    void GiveDragon(Side side)
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
        canSpawnChoGall = false;
    }
    IEnumerator WaitForDragonReload()
    {
        yield return new WaitForSeconds(60f);
        can = true;
        canSpawnChoGall = true;
    }
}

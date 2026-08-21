using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public ulong d;
    public string nameOfFaction;
    public Object.Side sidePlayer;
    public PlayerManager enemyManager;
    public int money;
    public Building main;
    Panel panel;
    UIManager manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = FindAnyObjectByType<UIManager>();
        if (IsHost && sidePlayer == Object.Side.Player)
        {
            GetComponent<NetworkObject>().ChangeOwnership(0);
        }
        else if(IsClient && sidePlayer == Object.Side.Enemy)
        {
            AskHostForOwnerServerRpc();
        }
    }

    // Update is called once per frame
    void Update()
    {
        ulong ownerId = OwnerClientId;
        d = ownerId;
        //Debug.Log(gameObject.name + OwnerClientId);
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
    [ServerRpc(RequireOwnership = false)]
    public void UpdateUnitOwnerServerRpc(NetworkObjectReference netObj)
    {
        if (netObj.TryGet(out NetworkObject ownerObj))
        {
            ownerObj.ChangeOwnership(OwnerClientId);
        }
    }
    [ServerRpc(RequireOwnership = false)] void AskHostForOwnerServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong callingClientId = rpcParams.Receive.SenderClientId;
        GetComponent<NetworkObject>().ChangeOwnership(callingClientId);
    }
}

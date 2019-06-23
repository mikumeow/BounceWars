using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using PlayerManagerSpc;
using PlayerUnitManagerSpc;

#pragma warning disable 0618 //ignore UNET warnings

public class PlayerConnectionObject : NetworkMessageHandler
{
    //the visible playerUnit object class
    public GameObject PlayerUnitPrefab;
    [Header("Player Properties")]
    public string playerID;
    //list of playerUnits


    // Start is called before the first frame update
    void Start()
    {
        //register player to PlayerManager
        playerID = "player" + GetComponent<NetworkIdentity>().netId.ToString();
        transform.name = playerID;
        PlayerManager.Instance.AddPlayerToConnectedPlayers(playerID, gameObject);
        //init playerUnit dictionary
        //PlayerUnits = new Dictionary<int, GameObject>();
        

        if (isLocalPlayer) //isLocalPlayer
        {
            PlayerManager.Instance.SetLocalPlayerID(playerID);

            Camera.main.transform.position = transform.position + new Vector3(0, 10, -20);
            Camera.main.transform.rotation = Quaternion.Euler(0, -15, 0);

            //spawn the playerUnit
            CmdSpawnMyUnit();

            RegisterNetworkMessages();
            
        }
        else
        {
        }
    }

    private void RegisterNetworkMessages()
    {
        NetworkManager.singleton.client.RegisterHandler(movement_msg, OnReceiveMovementMessage);
    }

    private void OnReceiveMovementMessage(NetworkMessage _message)
    {
        PlayerMovementMessage _msg = _message.ReadMessage<PlayerMovementMessage>();

        GameObject targetUnit = PlayerUnitManager.Instance.GetUnit(_msg.playerUnitID);
        if (targetUnit.GetComponent<PlayerUnit>().getPlayerID() != transform.name)
        //if (_msg.playerUnitID != transform.name)
        {
            //Debug.Log("User ID" + transform.name + "  message");
            targetUnit.GetComponent<PlayerUnit>()
                .ReceiveMovementMessage(_msg.objectPosition, _msg.objectRotation, _msg.time);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)//hasAuthority
        {
            return;
        }
        /*if (Input.GetKeyDown(KeyCode.C))
        {
            CmdSpawnMyUnit();
        }*/

    }


    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
           //do the lerp for each player object
            /*foreach (KeyValuePair<int, GameObject> entry in PlayerUnits)
            {
                entry.Value.GetComponent<PlayerUnit>().NetworkLerp();
            }*/
        }
    }

    private void OnDestroy()
    {
        //NetworkManager.singleton.client.UnregisterHandler(movement_msg);
        PlayerManager.Instance.RemovePlayerFromConnectedPlayers(playerID);
        PlayerUnitManager.Instance.RemoveUnitByPlayerID(transform.name);

    }

    //commands
    [Command]
    void CmdSpawnMyUnit()
    {
        GameObject pUnit = Instantiate(PlayerUnitPrefab);
        pUnit.GetComponent<PlayerUnit>().setPlayerID(playerID);
        //go.transform.parent = gameObject.transform;
        //PlayerUnits.Add(uID, go);
        //spread the object to all in the network
        //MyPlayerUnit = go;
        //go.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
        NetworkServer.SpawnWithClientAuthority(pUnit, connectionToClient);//connectionToClient

    }
    //network transform sync server to client
    /*
    [Command]
    void CmdMoveUnitUp()
    {
        if(MyPlayerUnit == null)
        {
            return;
        }

        MyPlayerUnit.transform.Translate(0, 1, 0);
    }

    [ClientRpc]
    void RPCChangePlayerName(string n)
    {
        Debug.Log("RPC change player name");
        PlayerName = n;
    }*/
}

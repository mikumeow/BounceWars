using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using PlayerManager;

#pragma warning disable 0618 //disable UNET warnings

public class PlayerConnectionObject : NetworkMessageHandler
{
    //the visible playerUnit object class
    public GameObject PlayerUnitPrefab;
    [Header("Player Properties")]
    public string playerID;
    //list of playerUnits
    public Dictionary<int, GameObject> PlayerUnits { get; set; }
    //incrementa playerUnit ID
    public int playerUnitIDIncrem;

    


    // Start is called before the first frame update
    void Start()
    {
        //register player to PlayerManager
        playerID = "player" + GetComponent<NetworkIdentity>().netId.ToString();
        transform.name = playerID;
        Manager.Instance.AddPlayerToConnectedPlayers(playerID, gameObject);
        //init playerUnit dictionary
        PlayerUnits = new Dictionary<int, GameObject>();
        playerUnitIDIncrem = 0;

        if (isLocalPlayer) //isLocalPlayer
        {
            Manager.Instance.SetLocalPlayerID(playerID);
            Camera.main.transform.position = transform.position + new Vector3(0, 0, -20);
            Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);

            RegisterNetworkMessages();

            //spawn the playerUnit?
            CmdSpawnMyUnit();
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

        //only local player Connection has this handler. so for transformations on other objs,
        //pass the message to corresponding object.
        if (_msg.targetPlayerID != transform.name)
        {
            Manager.Instance.ConnectedPlayers[_msg.targetPlayerID].GetComponent<PlayerConnectionObject>()
                .PlayerUnits[_msg.playerUnitID].GetComponent<PlayerUnit>()
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
        if (Input.GetKeyDown(KeyCode.C))
        {
            CmdSpawnMyUnit();
        }

    }


    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
           //do the lerp for each player object
            foreach (KeyValuePair<int, GameObject> entry in PlayerUnits)
            {
                entry.Value.GetComponent<PlayerUnit>().NetworkLerp();
            }
        }
    }
    //commands
    [Command]
    void CmdSpawnMyUnit()
    {
        GameObject go = Instantiate(PlayerUnitPrefab);
        go.GetComponent<PlayerUnit>().playerID = playerID;
        go.GetComponent<PlayerUnit>().playerUnitID = ++playerUnitIDIncrem;
        PlayerUnits.Add(playerUnitIDIncrem, go);
        //spread the object to all in the network
        //MyPlayerUnit = go;
        //go.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
        NetworkServer.SpawnWithClientAuthority(go,connectionToClient);
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

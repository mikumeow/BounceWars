using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
#pragma warning disable 0618 //ignore UNET warnings

public class NetworkServerRelay : NetworkMessageHandler
{
    private void Start()
    {
        if (isServer)
        {
            RegisterNetworkMessages();
        }
    }

    public void RegisterNetworkMessages()
    {
        Debug.Log("RegisterNetworkMessages");
        NetworkServer.RegisterHandler(movement_msg, OnReceivePlayerMovementMessage);
    }

    private void OnReceivePlayerMovementMessage(NetworkMessage _message)
    {
        //Debug.Log("Server message");
        //dispatch this message to all players.
        PlayerMovementMessage _msg = _message.ReadMessage<PlayerMovementMessage>();
        //Debug.Log("Server receive  message,  Target  " + _msg.playerUnitID);
        NetworkServer.SendToAll(movement_msg, _msg);
    }
}

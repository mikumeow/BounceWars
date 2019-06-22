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

    }

    public void RegisterNetworkMessages()
    {
        Debug.Log("RegisterNetworkMessages");
        NetworkServer.RegisterHandler(movement_msg, OnReceivePlayerMovementMessage);
    }

    private void OnReceivePlayerMovementMessage(NetworkMessage _message)
    {
        //dispatch this message to all players.
        PlayerMovementMessage _msg = _message.ReadMessage<PlayerMovementMessage>();
        NetworkServer.SendToAll(movement_msg, _msg);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerAdv : NetworkManager
{
    public GameObject networkServerRelay;
    public void Start()
    {
    }
    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("OnServerConnect");
        networkServerRelay.GetComponent<NetworkServerRelay>().RegisterNetworkMessages();
        base.OnServerConnect(conn);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("OnClientConnect");
        networkServerRelay.GetComponent<NetworkServerRelay>().RegisterNetworkMessages();
        base.OnClientConnect(conn);

    }
}

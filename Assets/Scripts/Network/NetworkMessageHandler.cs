using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
#pragma warning disable 0618 //ignore UNET warnings

public class NetworkMessageHandler : NetworkBehaviour
{
    public const short movement_msg = 1337;

    public class PlayerMovementMessage : MessageBase
    {
        public string playerUnitID;
        public Vector3 objectPosition;
        public Quaternion objectRotation;
        public float time;
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
#pragma warning disable 0618 //disable UNET warnings

public class NetworkMessageHandler : NetworkBehaviour
{
    public const short movement_msg = 601;

    public class PlayerMovementMessage : MessageBase
    {
        public string targetPlayerID;
        public int playerUnitID;
        public Vector3 objectPosition;
        public Quaternion objectRotation;
        public float time;
    }
}
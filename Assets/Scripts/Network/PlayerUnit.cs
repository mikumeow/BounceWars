using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using PlayerManager;
#pragma warning disable 0618 //ignore UNET warnings

public class PlayerUnit : NetworkMessageHandler
{
    public Rigidbody rb;
    Vector3 velocity;


    public String playerID { set; get; }
    public int playerUnitID { set; get; }

    [Header("Ship Movement Properties")]
    public bool canSendNetworkMovement;
    public float speed;
    public float networkSendRate = 5;
    public float timeBetweenMovementStart;
    public float timeBetweenMovementEnd;

    [Header("Lerping Properties")]
    public bool isLerpingPosition;
    public bool isLerpingRotation;
    public Vector3 realPosition;
    public Quaternion realRotation;
    public Vector3 lastRealPosition;
    public Quaternion lastRealRotation;
    public float timeStartedLerping;
    public float timeToLerp;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            canSendNetworkMovement = false;
        }
        rb = GetComponent<Rigidbody>();
    }


    //change opsition info
    public void ReceiveMovementMessage(Vector3 _position, Quaternion _rotation, float _timeToLerp)
    {
        lastRealPosition = realPosition;
        lastRealRotation = realRotation;
        realPosition = _position;
        realRotation = _rotation;
        timeToLerp = _timeToLerp;

        if (realPosition != transform.position)
        {
            isLerpingPosition = true;
        }

        if (realRotation.eulerAngles != transform.rotation.eulerAngles)
        {
            isLerpingRotation = true;
        }

        timeStartedLerping = Time.time;
    }
    //network lerp

    public void NetworkLerp()
    {
        if (isLerpingPosition)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;

            transform.position = Vector3.Lerp(lastRealPosition, realPosition, lerpPercentage);
        }

        if (isLerpingRotation)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;

            transform.rotation = Quaternion.Lerp(lastRealRotation, realRotation, lerpPercentage);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!hasAuthority)
        {
            return;
        }

        UpdatePlayerMovement();
        //delete object
        if (Input.GetKeyDown(KeyCode.X))
        {
            Destroy(gameObject);
            CmdDestroyObject();
        }
        //bump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            velocity = new Vector3(0, (float)0.5, 0);
            //rb.velocity += velocity;
            this.transform.Translate(0, 1, 0);
            CmdUpdateVelocity(velocity, transform.position);
        }
        //move
        /*
        if (Input.GetKeyDown(KeyCode.D))
        {
            velocity = new Vector3(1, 0, 0);
            //rb.velocity += velocity;
            CmdUpdateVelocity(velocity, transform.position);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            velocity = new Vector3(-1, 0, 0);
            //rb.velocity += velocity;
            CmdUpdateVelocity(velocity, transform.position);
        }*/
    }

    private void UpdatePlayerMovement()
    {
        var y = Input.GetAxis("Horizontal") * Time.deltaTime * 20;
        var x = Input.GetAxis("Vertical") * Time.deltaTime * 20;
        var z = 0;
        if (y != 0)
        {
            Debug.Log(y);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (speed < 10)
                speed++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (speed > -5)
                speed--;
        }

        if (speed != 0)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        if (y != 0 || x != 0 || z != 0)
        {
            transform.Rotate(x, y, -z);
        }

        if (!canSendNetworkMovement)
        {
            canSendNetworkMovement = true;
            StartCoroutine(StartNetworkSendCooldown());
        }
    }

    private IEnumerator StartNetworkSendCooldown()
    {
        timeBetweenMovementStart = Time.time;
        yield return new WaitForSeconds((1 / networkSendRate));
        SendNetworkMovement();
    }

    private void SendNetworkMovement()
    {
        timeBetweenMovementEnd = Time.time;
        SendMovementMessage(playerID, playerUnitID, transform.position, transform.rotation, (timeBetweenMovementEnd - timeBetweenMovementStart));
        canSendNetworkMovement = false;
    }

    public void SendMovementMessage(string _playerID,int _playerUnitID, Vector3 _position, Quaternion _rotation, float _timeTolerp)
    {
        PlayerMovementMessage _msg = new PlayerMovementMessage()
        {
            objectPosition = _position,
            objectRotation = _rotation,
            targetPlayerID = _playerID,
            playerUnitID = _playerUnitID,
            time = _timeTolerp
        };

        NetworkManager.singleton.client.Send(movement_msg, _msg);
    }

    [Command]
    void CmdUpdateVelocity(Vector3 v, Vector3 p)
    {
        transform.position = p;
        velocity = v;

        RpcUpdateVelocity(velocity, transform.position);
    }

    [ClientRpc]
    void RpcUpdateVelocity(Vector3 v, Vector3 p)
    {
        Debug.Log("PlayerUnit:: ClientRPC");
        //Owner of object don't update again
        if (hasAuthority)
            return;

        velocity = v;
        //bestGuessPosition = p + (velocity * (ourLatency));

    }

    [Command]
    void CmdDestroyObject()
    {
        NetworkServer.Destroy(gameObject);
    }
}

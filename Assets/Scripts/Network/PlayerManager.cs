using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 0618 //ignore UNET warnings

namespace PlayerManagerSpc
{
    public class PlayerManager
    {
        private static PlayerManager _instance;
        public Dictionary<string, GameObject> ConnectedPlayers { get; set; }

        public int NumberConnectedPlayers { get; private set; }

        public string PlayerID { get; private set; }

        private PlayerManager()
        {
            if (_instance != null)
            {
                return;
            }

            ConnectedPlayers = new Dictionary<string, GameObject>();
            NumberConnectedPlayers = 0;

            _instance = this;
        }

        public static PlayerManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    new PlayerManager();
                }

                return _instance;
            }
        }

        public void AddPlayerToConnectedPlayers(string _playerID, GameObject _playerObject)
        {
            if (!ConnectedPlayers.ContainsKey(_playerID))
            {
                ConnectedPlayers.Add(_playerID, _playerObject);
                NumberConnectedPlayers++;
            }
        }

        public void RemovePlayerFromConnectedPlayers(string _playerID)
        {
            if (ConnectedPlayers.ContainsKey(_playerID))
            {
                ConnectedPlayers.Remove(_playerID);
                NumberConnectedPlayers--;
            }
        }

        public GameObject[] GetConnectedPlayers()
        {
            return ConnectedPlayers.Values.ToArray();
        }

        public void SetLocalPlayerID(string _playerID)
        {
            PlayerID = _playerID;
        }

        public GameObject GetPlayerFromConnectedPlayers(string _playerID)
        {
            if (ConnectedPlayers.ContainsKey(_playerID))
            {
                return ConnectedPlayers[_playerID];
            }

            return null;
        }
    }
}

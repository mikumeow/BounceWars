using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerUnitManagerSpc
{
    public class PlayerUnitManager
    {
        private static PlayerUnitManager _instance;
        public Dictionary<string, GameObject> ConnectedPlayerUnits { get; set; }

        public int NumberConnectedPlayerUnits { get; private set; }

        public string LocalUnitID { get; private set; }

        private PlayerUnitManager()
        {
            if (_instance != null)
            {
                return;
            }

            ConnectedPlayerUnits = new Dictionary<string, GameObject>();
            NumberConnectedPlayerUnits = 0;

            _instance = this;
        }

        public static PlayerUnitManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    new PlayerUnitManager();
                }

                return _instance;
            }
        }

        public void AddUnit(string _unitID, GameObject _playerObject)
        {
            if (!ConnectedPlayerUnits.ContainsKey(_unitID))
            {
                ConnectedPlayerUnits.Add(_unitID, _playerObject);
                NumberConnectedPlayerUnits++;
            }
        }

        public void RemoveUnit(string _unitID)
        {
            if (ConnectedPlayerUnits.ContainsKey(_unitID))
            {
                ConnectedPlayerUnits.Remove(_unitID);
                NumberConnectedPlayerUnits--;
            }
        }

        public void RemoveUnitByPlayerID(string _playerID)
        {
            ArrayList del = new ArrayList();
            foreach (KeyValuePair<string, GameObject> _unit in ConnectedPlayerUnits)
            {
                if (_unit.Value.GetComponent<PlayerUnit>().getPlayerID() == _playerID)
                    del.Add(_unit.Key);
            }
            foreach (string uID in del)
            {
                RemoveUnit(uID);
            }
        }

        public GameObject[] GetConnectedUnits()
        {
            return ConnectedPlayerUnits.Values.ToArray();
        }

        public void SetLocalUnitID(string _unitID)
        {
            LocalUnitID = _unitID;
        }

        public GameObject GetUnit(string _unitID)
        {
            if (ConnectedPlayerUnits.ContainsKey(_unitID))
            {
                return ConnectedPlayerUnits[_unitID];
            }

            return null;
        }
    }
}

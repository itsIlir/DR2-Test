using System;
using System.Collections.Generic;
using GameModels;
using Networking;
using Services;
using UnityEngine;

namespace Gameplay
{
    public class ObjectManager : MonoBehaviour
    {
        [SerializeField]
        PlayerManager _playerManager;

        readonly Dictionary<uint, IObjectManager> _objectManagers = new Dictionary<uint, IObjectManager>();
        public ushort LocalClientId { get; set; }

        private void Awake()
        {
            var networkService = ServiceLocator<INetworkService>.Get();

            networkService.GetProcessor<ObjectInit>().OnMessage += OnObjectInit;
            networkService.GetProcessor<ObjectLocation>().OnMessage += OnObjectLocation;
            networkService.GetProcessor<ObjectRemove>().OnMessage += OnObjectRemove;
        }

        public bool ObjectExists(uint id) => _objectManagers.ContainsKey(id);

        public void OnObjectInit(ObjectInit init)
        {
            Debug.Log($"Init Object ID {init.Id} of type {init.Type} for owner {init.OwnerId}.");

            switch (init.Type)
            {
                case ObjectType.Player:
                    _playerManager.InitializePlayer(init.Id, init.OwnerId == LocalClientId, init.Location);
                    _objectManagers.Add(init.Id, _playerManager);
                    break;
                case ObjectType.Physics:
                case ObjectType.NPC:
                    break;
            }
        }

        public void OnObjectLocation(ObjectLocation location)
        {
            _objectManagers[location.Id].UpdateLocation(location.Id, location.Location);
        }

        public void OnObjectRemove(ObjectRemove remove)
        {
            Debug.Log($"Remove Object ID {remove.Id}.");
            _objectManagers[remove.Id].Remove(remove.Id);
        }
    }
}

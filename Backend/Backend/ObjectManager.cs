﻿using System.Collections.Generic;
using System.Linq;
using DarkRift.Server;
using GameModels;

namespace Backend
{
    public sealed class ObjectManager
    {
        private readonly Dictionary<IClient, HashSet<uint>> _clientOwnedObjects =
            new Dictionary<IClient, HashSet<uint>>();

        private readonly Dictionary<uint, PlayerObject> _objects = new Dictionary<uint, PlayerObject>();

        public bool TryGetObject(uint id, out PlayerObject networkObject)
            => _objects.TryGetValue(id, out networkObject);

        public bool TryGetClientObjects(IClient client, out IEnumerable<PlayerObject> clientObjects)
        {
            if (_clientOwnedObjects.TryGetValue(client, out var clientObjectIds))
            {
                clientObjects = clientObjectIds.Select(id => _objects[id]);
                return true;
            }

            clientObjects = default;
            return false;
        }

        public bool TryGetClientObjectIds(IClient client, out HashSet<uint> clientObjectIds)
            => _clientOwnedObjects.TryGetValue(client, out clientObjectIds);

        public bool ClientInitObject(IClient client, ObjectInit objectInit, out PlayerObject networkObject)
        {
            if (_objects.TryGetValue(objectInit.Id, out networkObject))
                return false;

            if (!_clientOwnedObjects.TryGetValue(client, out var clientObjects))
            {
                clientObjects = new HashSet<uint>();
                _clientOwnedObjects.Add(client, clientObjects);
            }

            // Create object.
            networkObject = new PlayerObject(objectInit.Id)
            {
                Owner = client,
                Location = objectInit.Location,
                Region = null,
            };
            _objects.Add(objectInit.Id, networkObject);
            clientObjects.Add(networkObject.Id);

            return true;
        }

        public bool ClientRemoveObject(IClient client, ObjectRemove objectRemove, out PlayerObject networkObject)
        {
            networkObject = default;

            // Fail if client doesn't own any objects.
            if (!_clientOwnedObjects.TryGetValue(client, out var clientObjects))
                return false;

            // Fail if client doesn't own this object (thus cannot be removed).
            if (!clientObjects.Remove(objectRemove.Id))
                return false;

            // Fail if object didn't exist in the object pool - should never happen, really.
            if (!_objects.Remove(objectRemove.Id, out networkObject))
                return false;

            return true;
        }
    }
}

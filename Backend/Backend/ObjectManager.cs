using System.Collections.Generic;
using System.Linq;
using DarkRift.Server;
using GameModels.Player;

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

        public bool ClientInitObject(IClient client, ClientPlayerInit objectInit, out PlayerObject playerObject)
        {
            //if (_objects.TryGetValue(objectInit.ClientId, out networkObject))
            //    return false;

            if (!_clientOwnedObjects.TryGetValue(client, out var clientObjects))
            {
                clientObjects = new HashSet<uint>();
                _clientOwnedObjects.Add(client, clientObjects);
            }

            // Create object.
            playerObject = new PlayerObject(client.ID)
            {
                Owner = client,
                Region = null,
                PlayerInit = objectInit.Init
            };
            _objects.Add(client.ID, playerObject);
            clientObjects.Add(playerObject.Id);

            return true;
        }

        public bool ClientRemoveObject(IClient client, out PlayerObject playerObject)
        {
            playerObject = default;

            // Fail if client doesn't own any objects.
            if (!_clientOwnedObjects.TryGetValue(client, out var clientObjects))
                return false;

            // Fail if client doesn't own this object (thus cannot be removed).
            if (!clientObjects.Remove(client.ID))
                return false;

            // Fail if object didn't exist in the object pool - should never happen, really.
            if (!_objects.Remove(client.ID, out playerObject))
                return false;

            return true;
        }
    }
}

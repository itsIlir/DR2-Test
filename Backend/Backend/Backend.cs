using System;
using DarkRift;
using System.Linq;
using DarkRift.Server;
using GameModels;
using System.Collections.Generic;

namespace Backend
{
    public class Backend : Plugin
    {
        private readonly Dictionary<IClient, HashSet<uint>> _clientOwnedObjects =
            new Dictionary<IClient, HashSet<uint>>();

        private readonly Dictionary<uint, IClient> _objectOwners = new Dictionary<uint, IClient>();

        private readonly Dictionary<uint, NetworkObject> _objects = new Dictionary<uint, NetworkObject>();

        public uint NetworkObjectIdCounter { get; private set; } = 1000;

        public override bool ThreadSafe => false;

        public override Version Version => new Version(1, 0, 0);

        public Backend(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            ClientManager.ClientConnected += OnClientConnected;
            ClientManager.ClientDisconnected += OnClientDisconnected;
        }

        private void OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            e.Client.MessageReceived += OnMessageReceived;

            var newPlayer = CreateNewPlayer(e);

            _objects.Add(newPlayer.Id, newPlayer);
            _objectOwners.Add(newPlayer.Id, e.Client);
            _clientOwnedObjects.Add(e.Client, new HashSet<uint> {newPlayer.Id});

            SendNewPlayerToOldClients(e.Client, newPlayer);
            SendAllPlayersToNewClient(e.Client);
        }

        private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            e.Client.MessageReceived -= OnMessageReceived;

            var removeClientObjectsMessage = _clientOwnedObjects[e.Client].Select(id => new ObjectRemove
            {
                Id = id
            }).Package();
            SendMessageToAll(removeClientObjectsMessage, ObjectRemove.StaticSendMode);

            foreach (var networkObjectId in _clientOwnedObjects[e.Client])
            {
                // TODO [Emanuel, 2022-08-17]: Check if object should be persistent and be repossessed.
                _objects.Remove(networkObjectId);
                _objectOwners.Remove(networkObjectId);
            }
            _clientOwnedObjects.Remove(e.Client);
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            using var message = e.GetMessage();
            if (VerifyAndProcessMessage((NetworkMessageType)e.Tag, message, e.Client))
                SendMessageToAllExcept(message, e.SendMode, e.Client);
        }

        private readonly List<(NetworkObject, LocationData)> _objectLocationUpdateQueue =
            new List<(NetworkObject, LocationData)>();
        private bool VerifyAndProcessMessage(NetworkMessageType type, Message message, IClient client)
        {
            _objectLocationUpdateQueue.Clear();
            switch (type)
            {
                case NetworkMessageType.ChatMessage:
                    // TODO [Emanuel, 2022-08-17]: Add chat message verification here.
                    return true;

                case NetworkMessageType.ObjectInit:
                case NetworkMessageType.ObjectRemove:
                    return false;

                case NetworkMessageType.ObjectLocation:
                {
                    using var reader = message.GetReader();
                    while (reader.Position < reader.Length)
                    {
                        var location = reader.ReadSerializable<ObjectLocation>();
                        if (!_objectOwners.TryGetValue(location.Id, out var objectOwner) || objectOwner != client)
                            return false;

                        _objectLocationUpdateQueue.Add((_objects[location.Id], location.Location));
                    }

                    break;
                }
                case NetworkMessageType.PlayerMovement:
                {
                    using var reader = message.GetReader();
                    while (reader.Position < reader.Length)
                    {
                        var move = reader.ReadSerializable<PlayerMovement>();
                        if (!_objectOwners.TryGetValue(move.Id, out var objectOwner) || objectOwner != client)
                            return false;

                        _objectLocationUpdateQueue.Add((_objects[move.Id], move.Location));
                    }

                    break;
                }
                case NetworkMessageType.PlayerInteract:
                {
                    using var reader = message.GetReader();
                    while (reader.Position < reader.Length)
                    {
                        var interact = reader.ReadSerializable<PlayerInteract>();
                        if (!_objectOwners.TryGetValue(interact.Id, out var objectOwner) || objectOwner != client)
                            return false;

                        _objectLocationUpdateQueue.Add((_objects[interact.Id], interact.Location));
                    }

                    break;
                }
            }

            foreach (var (networkObject, location) in _objectLocationUpdateQueue)
                UpdateLocation(ref networkObject.Location, location);

            return true;
        }

        private static void UpdateLocation(ref LocationData location, in LocationData updateData)
        {
            location.Flags = updateData.Flags;
            if ((updateData.Flags & (MovementFlags.Position2D | MovementFlags.Position3D)) != 0)
            {
                location.PositionX = updateData.PositionX;
                location.PositionY = updateData.PositionY;
                location.PositionZ = updateData.PositionZ;
            }

            if ((updateData.Flags & MovementFlags.Rotation2D) != 0)
            {
                location.Angle = updateData.Angle;
            }

            if ((updateData.Flags & MovementFlags.Rotation3D) != 0)
            {
                location.Pitch = updateData.Pitch;
                location.Yaw = updateData.Yaw;
                location.Roll = updateData.Roll;
            }

            if ((updateData.Flags & (MovementFlags.LinearVelocity2D | MovementFlags.LinearVelocity3D)) != 0)
            {
                location.LinearVelocityX = updateData.LinearVelocityX;
                location.LinearVelocityY = updateData.LinearVelocityY;
                location.LinearVelocityZ = updateData.LinearVelocityZ;
            }

            if ((updateData.Flags & MovementFlags.AngularVelocity2D) != 0)
            {
                location.AngularVelocitySpeed = updateData.AngularVelocitySpeed;
            }

            if ((updateData.Flags & MovementFlags.AngularVelocity3D) != 0)
            {
                location.AngularVelocitySpeed = updateData.AngularVelocitySpeed;
                location.AngularVelocityAxisX = updateData.AngularVelocityAxisX;
                location.AngularVelocityAxisY = updateData.AngularVelocityAxisY;
                location.AngularVelocityAxisZ = updateData.AngularVelocityAxisZ;
            }
        }

        private NetworkObject CreateNewPlayer(ClientConnectedEventArgs e)
        {
            return new NetworkObject
            (
                NetworkObjectIdCounter++,
                ObjectType.Player,
                new LocationData
                {
                    Flags = MovementFlags.None,
                }
            );
        }

        private void SendNewPlayerToOldClients(IClient newClient, NetworkObject newNetworkObject)
        {
            using var message = new ObjectInit
            {
                Id = newNetworkObject.Id,
                OwnerId = newClient.ID,
                Type = newNetworkObject.Type,
                Location = newNetworkObject.Location,
            }.Package();
            SendMessageToAllExcept(message, ObjectInit.StaticSendMode, newClient);
        }

        private void SendAllPlayersToNewClient(IClient newClient)
        {
            using var message = _objects.Values.Select(networkObject => new ObjectInit
            {
                Id = networkObject.Id,
                OwnerId = _objectOwners[networkObject.Id].ID,
                Type = networkObject.Type,
                Location = networkObject.Location,
            }).Package();
            Console.WriteLine($"Sending object initialization to new client {newClient.ID}.");
            newClient.SendMessage(message, ObjectInit.StaticSendMode);
        }

        private void SendMessageToAll(Message message, SendMode sendMode)
        {
            foreach (var client in _clientOwnedObjects.Keys)
                client.SendMessage(message, sendMode);
        }

        private void SendMessageToAllExcept(Message message, SendMode sendMode, IClient ignoredClient)
        {
            foreach (var client in _clientOwnedObjects.Keys)
            {
                if (client == ignoredClient)
                    continue;

                client.SendMessage(message, sendMode);
            }
        }
    }
}

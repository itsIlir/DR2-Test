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
        private readonly RegionManager _roomManager = new RegionManager();
        private readonly ObjectManager _objectManager = new ObjectManager();

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
        }

        private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            e.Client.MessageReceived -= OnMessageReceived;

            if (!_objectManager.TryGetClientObjects(e.Client, out var clientObjects))
                return;

            var removeObjectList = new List<ObjectRemove>();
            foreach (var clientObject in clientObjects)
            {
                if (clientObject.Type == ObjectType.Player)
                {
                    removeObjectList.Add(new ObjectRemove
                    {
                        Id = clientObject.Id,
                    });
                }
            }

            foreach (var objectRemove in removeObjectList)
            {
                _objectManager.ClientRemoveObject(e.Client, objectRemove, out var networkObject);
                _roomManager.RemoveObjectFromRegion(e.Client, objectRemove, networkObject);
            }
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            using var message = e.GetMessage();
            VerifyAndProcessMessage((NetworkMessageType) e.Tag, message, e.Client);
        }

        /// TODO [Emanuel, 2022-08-25]: Refactor this method by splitting out functionality in some sensible way.
        private void VerifyAndProcessMessage(NetworkMessageType type, Message message, IClient client)
        {
            using var reader = message.GetReader();
            switch (type)
            {
                case NetworkMessageType.ChatMessage:
                    // TODO [Emanuel, 2022-08-17]: Add chat message verification here.
                    foreach (var networkClient in ClientManager.GetAllClients())
                    {
                        if (networkClient == client)
                            continue;

                        networkClient.SendMessage(message, ChatMessage.StaticSendMode);
                    }

                    break;

                case NetworkMessageType.RegionJoin:
                    while (reader.Position < reader.Length)
                    {
                        var regionJoin = reader.ReadSerializable<RegionJoin>();
                        if (!_roomManager.ClientJoinRegion(client, regionJoin))
                        {
                            LogManager.GetLoggerFor(nameof(RegionManager))
                                .Warning($"Client {client.ID} failed to join room {regionJoin.RegionId}.");
                            continue;
                        }
                        LogManager.GetLoggerFor(nameof(Backend))
                            .Info($"Client {client.ID} joined room {regionJoin.RegionId}.");
                    }
                    break;

                case NetworkMessageType.RegionLeave:
                    while (reader.Position < reader.Length)
                    {
                        var regionLeave = reader.ReadSerializable<RegionLeave>();
                        if (!_roomManager.ClientLeaveRegion(client, regionLeave))
                        {
                            LogManager.GetLoggerFor(nameof(RegionManager))
                                .Warning($"Client {client.ID} failed to leave room {regionLeave.RoomId}.");
                            continue;
                        }
                        LogManager.GetLoggerFor(nameof(Backend))
                            .Info($"Client {client.ID} left room {regionLeave.RoomId}.");
                    }
                    break;

                case NetworkMessageType.ObjectInit:
                    while (reader.Position < reader.Length)
                    {
                        var objectInit = reader.ReadSerializable<ObjectInit>();
                        if (objectInit.OwnerId != client.ID)
                        {
                            LogManager.GetLoggerFor(nameof(Backend))
                                .Warning($"Client {client.ID} failed to init object with owner {objectInit.OwnerId}.");
                            continue;
                        }

                        objectInit.Id = NetworkObjectIdCounter++;
                        if (!_objectManager.ClientInitObject(client, objectInit, out var networkObject))
                        {
                            LogManager.GetLoggerFor(nameof(ObjectManager))
                                .Warning($"Client {client.ID} failed to init object of type {objectInit.Type}.");
                            continue;
                        }

                        if (!_roomManager.InitObjectInRegion(client, objectInit, networkObject))
                        {
                            LogManager.GetLoggerFor(nameof(RegionManager))
                                .Warning($"Client {client.ID} failed to init object inside room {objectInit.RegionId}.");
                            continue;
                        }

                        LogManager.GetLoggerFor(nameof(Backend))
                            .Info($"Client {client.ID} initialized object {objectInit.Id} of type {objectInit.Type} into room {objectInit.RegionId}.");
                    }
                    break;

                case NetworkMessageType.ObjectRemove:
                    while (reader.Position < reader.Length)
                    {
                        var objectRemove = reader.ReadSerializable<ObjectRemove>();
                        if (!_objectManager.ClientRemoveObject(client, objectRemove, out var networkObject))
                        {
                            LogManager.GetLoggerFor(nameof(ObjectManager))
                                .Warning($"Client {client.ID} failed to remove object {objectRemove.Id}.");
                            continue;
                        }

                        if (!_roomManager.RemoveObjectFromRegion(client, objectRemove, networkObject))
                        {
                            LogManager.GetLoggerFor(nameof(RegionManager))
                                .Warning($"Client {client.ID} failed to remove object {objectRemove.Id} from room its room.");
                            continue;
                        }

                        LogManager.GetLoggerFor(nameof(Backend))
                            .Info($"Client {client.ID} removed object {objectRemove.Id}.");
                    }
                    break;

                case NetworkMessageType.ObjectLocation:
                    while (reader.Position < reader.Length)
                    {
                        var location = reader.ReadSerializable<ObjectLocation>();
                        if (!_objectManager.TryGetObject(location.Id, out var networkObject)
                            || networkObject.Owner != client)
                            continue;

                        UpdateLocation(ref networkObject.Location, location.Location);
                        networkObject.Region.SendMessageToAllExcept(location.Package(), location.SendMode, client);
                    }
                    break;

                case NetworkMessageType.PlayerMovement:
                    while (reader.Position < reader.Length)
                    {
                        var move = reader.ReadSerializable<PlayerMovement>();
                        if (!_objectManager.TryGetObject(move.Id, out var networkObject)
                            || networkObject.Owner != client)
                            continue;

                        UpdateLocation(ref networkObject.Location, move.Location);
                        networkObject.Region.SendMessageToAllExcept(move.Package(), move.SendMode, client);
                    }
                    break;

                case NetworkMessageType.PlayerInteract:
                    while (reader.Position < reader.Length)
                    {
                        var interact = reader.ReadSerializable<PlayerInteract>();
                        if (!_objectManager.TryGetObject(interact.Id, out var networkObject)
                            || networkObject.Owner != client)
                            continue;

                        UpdateLocation(ref networkObject.Location, interact.Location);
                        networkObject.Region.SendMessageToAllExcept(interact.Package(), interact.SendMode, client);
                    }
                    break;
            }
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
    }
}

using System;
using System.Collections.Generic;
using DarkRift;
using DarkRift.Server;
using GameModels;
using GameModels.Player;
using GameModels.Region;

namespace Backend
{
    public class Backend : Plugin
    {
        private Dictionary<IClient, NetworkPlayer> _networkPlayer = new Dictionary<IClient, NetworkPlayer>();

        private readonly RegionManager _roomManager = new RegionManager();

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

            _networkPlayer.TryGetValue(e.Client, out var networkPlayer);
            _roomManager.RemoveObjectFromRegion(e.Client, new ClientPlayerRemove(), networkPlayer);
            _networkPlayer.Remove(e.Client);
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            using var message = e.GetMessage();
            VerifyAndProcessMessage((NetworkMessageType) e.Tag, message, e.Client);
        }

        // TODO [Emanuel, 2022-08-25]: Refactor this method by splitting out functionality in some sensible way.
        private void VerifyAndProcessMessage(NetworkMessageType type, Message message, IClient client)
        {
            if(message == null || client == null)
                return;
            using var reader = message.GetReader();
            switch (type)
            {
                case NetworkMessageType.ClientChatMessage:
                    // TODO [Emanuel, 2022-08-17]: Add chat message verification here.
                    var clientChatMessage = reader.ReadSerializable<ClientChatMessage>();
                    foreach (var networkClient in ClientManager.GetAllClients())
                    {
                        if (networkClient == client)
                            continue;
                        networkClient.SendMessage(new ServerChatMessage
                        {
                            ClientId = client.ID, Message = clientChatMessage.Message 
                        }.Package(), ChatMessage.StaticSendMode);
                    }
                    break;

                case NetworkMessageType.ClientRegionJoin:
                    while (reader.Position < reader.Length)
                    {
                        var regionJoin = reader.ReadSerializable<ClientRegionJoin>();
                        if (!_roomManager.ClientJoinRegion(client, regionJoin))
                        {
                            LogManager.GetLoggerFor(nameof(RegionManager))
                                .Warning($"Client {client.ID} failed to join region {regionJoin.RegionId}.");
                            continue;
                        }
                        LogManager.GetLoggerFor(nameof(Backend))
                            .Info($"Client {client.ID} joined region {regionJoin.RegionId}.");
                    }
                    break;

                case NetworkMessageType.ClientRegionLeave:
                    while (reader.Position < reader.Length)
                    {
                        var regionLeave = reader.ReadSerializable<ClientRegionLeave>();
                        if (!_roomManager.ClientLeaveRegion(client, regionLeave))
                        {
                            LogManager.GetLoggerFor(nameof(RegionManager))
                                .Warning($"Client {client.ID} failed to leave region {regionLeave.RegionId}.");
                            continue;
                        }
                        LogManager.GetLoggerFor(nameof(Backend))
                            .Info($"Client {client.ID} left region {regionLeave.RegionId}.");
                    }
                    break;

                case NetworkMessageType.ClientPlayerInit:
                    while (reader.Position < reader.Length)
                    {
                        var objectInit = reader.ReadSerializable<ClientPlayerInit>();
                        if (!_roomManager.InitPlayerInRegion(client, objectInit, out var networkPlayer))
                        {
                            LogManager.GetLoggerFor(nameof(RegionManager))
                                .Warning($"Failed to init a player for client {client.ID} inside region {networkPlayer.Region.RegionId}.");
                            continue;
                        }
                        _networkPlayer.Add(client, networkPlayer);

                        LogManager.GetLoggerFor(nameof(Backend))
                            .Info($"Client {client.ID} initialized a player into region {networkPlayer.Region.RegionId}.");
                    }
                    break;

                case NetworkMessageType.ClientPlayerRemove:
                    while (reader.Position < reader.Length)
                    {
                        var objectRemove = reader.ReadSerializable<ClientPlayerRemove>();
                        if (!_networkPlayer.TryGetValue(client, out var networkPlayer))
                        {
                            LogManager.GetLoggerFor(nameof(Backend))
                                .Warning($"Failed to remove client {client.ID}.");
                            continue;
                        }

                        if (!_roomManager.RemoveObjectFromRegion(client, objectRemove, networkPlayer))
                        {
                            LogManager.GetLoggerFor(nameof(RegionManager))
                                .Warning($"Failed to remove client {client.ID} from room its region.");
                            continue;
                        }

                        _networkPlayer.Remove(client);
                        LogManager.GetLoggerFor(nameof(Backend)).Info($"Removed client {client.ID}.");
                    }
                    break;

                //TODO::Check if we need new message type here ot not
                //case NetworkMessageType.ObjectLocation:
                //    while (reader.Position < reader.Length)
                //    {
                //        var location = reader.ReadSerializable<ObjectLocation>();
                //        if (!_objectManager.TryGetObject(location.Id, out var networkObject)
                //            || networkObject.Owner != client)
                //            continue;

                //        UpdateLocation(ref networkObject.Location, location.Location);
                //        networkObject.Region.SendMessageToAllExcept(location.Package(), location.SendMode, client);
                //    }
                //    break;

                case NetworkMessageType.ClientPlayerMovement:
                    while (reader.Position < reader.Length)
                    {
                        var move = reader.ReadSerializable<ClientPlayerMovement>();
                        if (!_networkPlayer.TryGetValue(client, out var networkPlayer))
                            continue;

                        networkPlayer.PlayerMovement = move.Movement;
                        networkPlayer.Region.SendMessageToAllExcept(move.Package(), move.SendMode, client);
                    }
                    break;

                case NetworkMessageType.ClientPlayerJump:
                    while (reader.Position < reader.Length)
                    {
                        var jump = reader.ReadSerializable<ClientPlayerJump>();
                        if (!_networkPlayer.TryGetValue(client, out var networkPlayer))
                            continue;

                        networkPlayer.PlayerMovement = jump.Jump.Movement;
                        networkPlayer.Region.SendMessageToAllExcept(jump.Package(), jump.SendMode, client);
                    }
                    break;
            }
        }
    }
}

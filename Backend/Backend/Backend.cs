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
        readonly Dictionary<IClient, Player> Players = new Dictionary<IClient, Player>();

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

            Player newPlayer = CreateNewPlayer(e);

            Players.Add(e.Client, newPlayer);

            SendNewPlayerToOldClients(e.Client, newPlayer);
            SendAllPlayersToNewClient(e.Client);
        }

        private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            e.Client.MessageReceived -= OnMessageReceived;
            Players.Remove(e.Client);
            RemovePlayer(e.Client);
        }

        private void RemovePlayer(IClient client)
        {
            using var message = new ObjectRemove
            {
                Id = client.ID
            }.Package();
            SendMessageToAll(message, ObjectRemove.StaticSendMode);
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            using var message = e.GetMessage();
            if (VerifyMessage((NetworkMessageType)e.Tag, message, e.Client))
                SendMessageToAllExcept(message, e.SendMode, e.Client);
        }

        private bool VerifyMessage(NetworkMessageType type, Message message, IClient client)
        {
            switch (type)
            {
                case NetworkMessageType.ObjectInit:
                case NetworkMessageType.ObjectRemove:
                    return false;

                case NetworkMessageType.ObjectLocation:
                {
                    using var reader = message.GetReader();
                    while (reader.Position < reader.Length)
                    {
                        var move = reader.ReadSerializable<ObjectLocation>();
                        if (move.Id != client.ID)
                            return false;
                    }

                    break;
                }
                case NetworkMessageType.PlayerMovement:
                {
                    using var reader = message.GetReader();
                    while (reader.Position < reader.Length)
                    {
                        var move = reader.ReadSerializable<PlayerMovement>();
                        if (move.Id != client.ID)
                            return false;
                    }

                    break;
                }
                case NetworkMessageType.PlayerInteract:
                {
                    using var reader = message.GetReader();
                    while (reader.Position < reader.Length)
                    {
                        var move = reader.ReadSerializable<PlayerInteract>();
                        if (move.Id != client.ID)
                            return false;
                    }

                    break;
                }
            }
            return true;
        }

        private Player CreateNewPlayer(ClientConnectedEventArgs e)
        {
            Random r = new Random();
            return new Player
            (
                e.Client.ID,
                r.Next(-10, 20),
                r.Next(-10, 10)
            );
        }

        private void SendNewPlayerToOldClients(IClient newClient, Player newPlayer)
        {
            using var message = new ObjectInit
            {
                Id = newPlayer.ID,
                Movement = new MovementData
                {
                    Flags = MovementFlags.Position2D,
                    PositionX = newPlayer.X,
                    PositionY = newPlayer.Y,
                }
            }.Package();
            SendMessageToAllExcept(message, ObjectInit.StaticSendMode, newClient);
        }

        private void SendAllPlayersToNewClient(IClient newClient)
        {
            using var message = Players.Values.Select(player => new ObjectInit
            {
                Id = player.ID,
                Movement = new MovementData
                {
                    Flags = MovementFlags.Position2D,
                    PositionX = player.X,
                    PositionY = player.Y,
                }
            }).Package();
            Console.WriteLine($"Sending object initialization to new client {newClient.ID}.");
            newClient.SendMessage(message, ObjectInit.StaticSendMode);
        }

        private void SendMessageToAll(Message message, SendMode sendMode)
        {
            foreach (var client in Players.Keys)
                client.SendMessage(message, sendMode);
        }

        private void SendMessageToAllExcept(Message message, SendMode sendMode, IClient ignoredClient)
        {
            foreach (var client in Players.Keys)
            {
                if (client == ignoredClient)
                    continue;

                client.SendMessage(message, sendMode);
            }
        }
    }
}

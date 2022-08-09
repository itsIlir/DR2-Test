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
        Dictionary<IClient, Player> players = new Dictionary<IClient, Player>();

        public override bool ThreadSafe => false;

        public override Version Version => new Version(1, 0, 0);

        public Backend(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            ClientManager.ClientConnected += OnClientConnected;
            ClientManager.ClientDisconnected += OnClientDisconected;
        }

        private void OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Console.WriteLine("Player Connected");
            e.Client.MessageReceived += OnMessageRecived;

            Player newPlayer = CreateNewPlayer(e);

            players.Add(e.Client, newPlayer);

            SendNewPlayerToAll(e, newPlayer);
            SendAllPlayersToNew(e);
        }

        private void OnClientDisconected(object sender, ClientDisconnectedEventArgs e)
        {
            Console.WriteLine("Player Disconnected");
            players.Remove(e.Client);
            RemovePlayer(e);
        }

        private void RemovePlayer(ClientDisconnectedEventArgs e)
        {
            Remove remove = new Remove();
            remove.ID = e.Client.ID;
            using (Message removePlayerMessage = Message.Create((ushort)Tags.Tag.PLAYER_REMOVE, remove))
            {
                foreach (var client in ClientManager.GetAllClients().Where(x => x != e.Client))
                {
                    client.SendMessage(removePlayerMessage, SendMode.Unreliable);
                }
            }

        }

        private void OnMessageRecived(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage())
            {
                using (DarkRiftReader reader = message.GetReader())
                {
                    switch (message.Tag)
                    {
                        case (ushort)Tags.Tag.TEXT_MSG:
                            while (reader.Position < reader.Length)
                            {
                                Chat chat = reader.ReadSerializable<Chat>();

                                using (Message newMessage = Message.Create((ushort)Tags.Tag.TEXT_MSG, chat))
                                {
                                    foreach (var client in ClientManager.GetAllClients().Where(x => x != e.Client))
                                    {
                                        client.SendMessage(newMessage, SendMode.Reliable);
                                    }
                                }
                            }
                            break;

                        case (ushort)Tags.Tag.PLAYER_MOVE:
                            foreach (var client in ClientManager.GetAllClients().Where(x => x != e.Client))
                            {
                                client.SendMessage(message, SendMode.Unreliable);
                            }
                            break;
                    }
                }
            }
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

        private void SendNewPlayerToAll(ClientConnectedEventArgs e, Player newPlayer)
        {
            Spawn spawnToAll = new Spawn();
            spawnToAll.ID = newPlayer.ID;
            spawnToAll.X = newPlayer.X;
            spawnToAll.Y = newPlayer.Y;

            using (Message newPlayerMessage = Message.Create((ushort)Tags.Tag.SPAWN_PLAYER, spawnToAll))
            {
                foreach (IClient client in ClientManager.GetAllClients().Where(x => x != e.Client))//TODO::Check performance
                {
                    client.SendMessage(newPlayerMessage, SendMode.Reliable);
                }
            }
        }

        private void SendAllPlayersToNew(ClientConnectedEventArgs e)
        {
            using (DarkRiftWriter playerWriter = DarkRiftWriter.Create())
            {
                Spawn s = new Spawn();
                foreach (Player player in players.Values)
                {
                    s.ID = player.ID;
                    s.X = player.X;
                    s.Y = player.Y;
                    playerWriter.Write(s);
                }

                using (Message playerMessage = Message.Create((ushort)Tags.Tag.SPAWN_PLAYER, playerWriter))
                {
                    e.Client.SendMessage(playerMessage, SendMode.Reliable);
                }
            }
        }
    }
}

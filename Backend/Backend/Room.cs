using System.Collections.Generic;
using System.Linq;
using DarkRift;
using DarkRift.Server;
using GameModels;

namespace Backend
{
    public class Room
    {
        public Room(uint roomId)
        {
            RoomId = roomId;
        }

        public readonly uint RoomId;
        public HashSet<IClient> Clients { get; } = new HashSet<IClient>();
        public HashSet<NetworkObject> Objects { get; } = new HashSet<NetworkObject>();

        public void AddObject(NetworkObject networkObject)
        {
            networkObject.Room = this;
            SendMessageToAll(new ObjectInit
            {
                Id = networkObject.Id,
                Location = networkObject.Location,
                Type = networkObject.Type,
                OwnerId = networkObject.Owner.ID,
            }.Package(), ObjectInit.StaticSendMode);
        }

        public void RemoveObject(NetworkObject networkObject)
        {
            networkObject.Room = null;
            SendMessageToAll(new ObjectRemove
            {
                Id = networkObject.Id,
            }.Package(), ObjectRemove.StaticSendMode);
        }

        public void TransferObject(NetworkObject networkObject)
        {
            var oldRoom = networkObject.Room;
            networkObject.Room = this;

            SendMessageToAll(new ObjectRemove
            {
                Id = networkObject.Id,
            }.Package(), ObjectRemove.StaticSendMode);
        }

        public void JoinRoom(IClient client)
        {
            Clients.Add(client);
            client.SendMessage(Objects.Select(o => new ObjectInit
            {
                Id = o.Id,
                Location = o.Location,
                Type = o.Type,
                OwnerId = o.Owner.ID,
            }).Package(), ObjectInit.StaticSendMode);
        }

        public void LeaveRoom(IClient client)
        {
            Clients.Remove(client);
            client.SendMessage(Objects.Select(o => new ObjectRemove
            {
                Id = o.Id,
            }).Package(), ObjectRemove.StaticSendMode);
        }

        public void SendMessageToAll(Message message, SendMode sendMode)
        {
            foreach (var client in Clients)
                client.SendMessage(message, sendMode);
        }

        public void SendMessageToAllExcept(Message message, SendMode sendMode, IClient ignoredClient)
        {
            foreach (var client in Clients)
            {
                if (client == ignoredClient)
                    continue;

                client.SendMessage(message, sendMode);
            }
        }

        public void SendMessageToAllExcept(Message message, SendMode sendMode, HashSet<IClient> ignoredClients)
        {
            foreach (var client in Clients)
            {
                if (ignoredClients.Contains(client))
                    continue;

                client.SendMessage(message, sendMode);
            }
        }
    }
}

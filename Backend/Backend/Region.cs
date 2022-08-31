using System.Collections.Generic;
using System.Linq;
using DarkRift;
using DarkRift.Server;
using GameModels;

namespace Backend
{
    public class Region
    {
        public Region(uint roomId)
        {
            RegionId = roomId;
        }

        public readonly uint RegionId;
        public HashSet<IClient> Clients { get; } = new HashSet<IClient>();
        public HashSet<PlayerObject> Objects { get; } = new HashSet<PlayerObject>();

        public void AddObject(PlayerObject networkObject)
        {
            networkObject.Region = this;
            SendMessageToAll(new ObjectInit
            {
                Id = networkObject.Id,
                Location = networkObject.Location,
                //Type = networkObject.Type,
                OwnerId = networkObject.Owner.ID,
            }.Package(), ObjectInit.StaticSendMode);
        }

        public void RemoveObject(PlayerObject networkObject)
        {
            networkObject.Region = null;
            SendMessageToAll(new ObjectRemove
            {
                Id = networkObject.Id,
            }.Package(), ObjectRemove.StaticSendMode);
        }

        public void TransferObject(PlayerObject networkObject)
        {
            var oldRoom = networkObject.Region;
            networkObject.Region = this;

            SendMessageToAll(new ObjectRemove
            {
                Id = networkObject.Id,
            }.Package(), ObjectRemove.StaticSendMode);
        }

        public void JoinRegion(IClient client)
        {
            Clients.Add(client);
            client.SendMessage(Objects.Select(o => new ObjectInit
            {
                Id = o.Id,
                Location = o.Location,
                //Type = o.Type,
                OwnerId = o.Owner.ID,
            }).Package(), ObjectInit.StaticSendMode);
        }

        public void LeaveRegion(IClient client)
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

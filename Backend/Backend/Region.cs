using System.Collections.Generic;
using System.Linq;
using DarkRift;
using DarkRift.Server;
using GameModels;

namespace Backend
{
    public class Region
    {
        public Region(uint regionId)
        {
            RegionId = regionId;
        }

        public readonly uint RegionId;
        public HashSet<IClient> Clients { get; } = new HashSet<IClient>();
        public HashSet<NetworkPlayer> Objects { get; } = new HashSet<NetworkPlayer>();

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

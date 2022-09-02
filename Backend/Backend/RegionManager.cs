using System.Collections.Generic;
using System.Linq;
using DarkRift.Server;
using GameModels;
using GameModels.Player;
using GameModels.Region;

namespace Backend
{
    public sealed class RegionManager
    {
        private readonly Dictionary<uint, Region> _regions = new Dictionary<uint, Region>();
        private readonly Dictionary<IClient, Region> _clientsRegion = new Dictionary<IClient, Region>();

        public Region GetRegion(uint id)
        {
            if (!_regions.TryGetValue(id, out var region))
            {
                region = new Region(id);
                _regions.Add(id, region);
            }

            return region;
        }

        public bool ClientJoinRegion(IClient client, ClientRegionJoin regionJoin)
        {
            var region = GetRegion(regionJoin.RegionId);
            if(!_clientsRegion.TryGetValue(client, out var clientRegion))
                _clientsRegion.Add(client, region);
            if (!region.Clients.Add(client))
                return false;

            //Send all players to new client
            client.SendMessage(region.Objects.Select(o => new ServerPlayerInit
            {
                ClientId = o.Id,
                Init = o.PlayerInit,
                //RegionId = o.Region.RegionId,//TODO::Do we need this
            }).Package(), PlayerInit.StaticSendMode);

            //Send new player to all clients
            region.SendMessageToAllExcept(new ServerPlayerInit{ClientId = client.ID}.Package(),
                PlayerInit.StaticSendMode, client);

            return true;
        }

        public bool ClientLeaveRegion(IClient client, ClientRegionLeave regionLeave)
        {
            if (!_clientsRegion.TryGetValue(client, out var clientRegion))
                return false;

            _clientsRegion.Remove(client);
            var region = GetRegion(regionLeave.RegionId);
            if (region.Clients.TryGetValue(client, out var thisClient))
                region.Clients.Remove(thisClient);

            region.SendMessageToAllExcept(new ServerPlayerRemove { ClientId = client.ID }.Package(),
                ServerPlayerRemove.StaticSendMode, client);

            return true;
        }

        public bool InitPlayerInRegion(IClient client, ClientPlayerInit init, NetworkPlayer networkPlayer)
        {
            if (!_clientsRegion.TryGetValue(client, out var region))
                return false;

            region.Objects.Add(networkPlayer);
            networkPlayer.Region = region;

            region.SendMessageToAll(init.Package(), init.SendMode);

            return true;
        }

        public bool RemoveObjectFromRegion(IClient client, ClientPlayerRemove remove, NetworkPlayer networkPlayer)
        {
            var region = networkPlayer.Region;
            region.Objects.Remove(networkPlayer);
            networkPlayer.Region = null;

            region.SendMessageToAllExcept(new ServerPlayerRemove { ClientId = client.ID }.Package(),
                ServerPlayerRemove.StaticSendMode, client);

            return true;
        }
    }
}

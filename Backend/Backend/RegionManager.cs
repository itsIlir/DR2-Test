using System.Collections.Generic;
using System.Linq;
using DarkRift.Server;
using GameModels;
using GameModels.Player;
using GameModels.Region;
using GameModels.Geometry;

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

            client.SendMessage(region.Objects.Select(o => new ServerPlayerInit
            {
                ClientId = o.Id,
                Init = o.PlayerInit,
                //RegionId = o.Region.RegionId,//TODO::Do we need this

            }).Package(), PlayerInit.StaticSendMode);

            return true;
        }

        public bool ClientLeaveRegion(IClient client, ClientRegionLeave regionLeave)
        {
            if (!_clientsRegion.TryGetValue(client, out var clientRegion))
                return false;
            _clientsRegion.Remove(client);
            var room = GetRegion(regionLeave.RegionId);

            client.SendMessage(room.Objects.Select(o => new ServerPlayerRemove
            {
                ClientId = o.Id,
            }).Package(), ServerPlayerRemove.StaticSendMode);

            return true;
        }

        public bool InitPlayerInRegion(IClient client, ClientPlayerInit init, PlayerObject playerObject)
        {
            if (!_clientsRegion.TryGetValue(client, out var region))
                return false;

            region.Objects.Add(playerObject);
            playerObject.Region = region;

            region.SendMessageToAll(init.Package(), init.SendMode);

            return true;
        }

        public bool RemoveObjectFromRegion(IClient client, ClientPlayerRemove remove, PlayerObject playerObject)
        {
            var region = playerObject.Region;
            region.Objects.Remove(playerObject);
            playerObject.Region = null;

            region.SendMessageToAllExcept(remove.Package(), remove.SendMode, client);

            return true;
        }
    }
}

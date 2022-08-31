using System.Collections.Generic;
using System.Linq;
using DarkRift.Server;
using GameModels;

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

        public bool ClientJoinRegion(IClient client, RegionJoin regionJoin)
        {
            var region = GetRegion(regionJoin.RegionId);
            if(!_clientsRegion.TryGetValue(client, out var reg))
                _clientsRegion.Add(client, region);
            if (!region.Clients.Add(client))
                return false;

            client.SendMessage(region.Objects.Select(o => new ObjectInit
            {
                Id = o.Id,
                Location = o.Location,
                //Type = o.Type,
                RegionId = o.Region.RegionId,
                OwnerId = o.Owner.ID,
            }).Package(), ObjectInit.StaticSendMode);

            return true;
        }

        public bool ClientLeaveRegion(IClient client, RegionLeave regionLeave)
        {
            if (!_clientsRegion.TryGetValue(client, out var clientRegion))
                return false;
            _clientsRegion.Remove(client);
            var room = GetRegion(regionLeave.RoomId);

            client.SendMessage(room.Objects.Select(o => new ObjectRemove
            {
                Id = o.Id,
            }).Package(), ObjectRemove.StaticSendMode);

            return true;
        }

        public bool InitPlayerInRegion(IClient client, ObjectInit init, PlayerObject playerObject)
        {
            if (!_clientsRegion.TryGetValue(client, out var clientRegion) || (clientRegion.RegionId != init.RegionId))
                return false;

            var region = GetRegion(init.RegionId);
            region.Objects.Add(playerObject);
            playerObject.Region = region;

            region.SendMessageToAll(init.Package(), init.SendMode);

            return true;
        }

        public bool RemoveObjectFromRegion(IClient client, ObjectRemove remove, PlayerObject playerObject)
        {
            var region = playerObject.Region;
            region.Objects.Remove(playerObject);
            playerObject.Region = null;

            region.SendMessageToAllExcept(remove.Package(), remove.SendMode, client);

            return true;
        }
    }
}

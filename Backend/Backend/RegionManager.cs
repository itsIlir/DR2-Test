﻿using System;
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
            Console.WriteLine($"ClientJoinRegion 1 Passed");
            //Send all players to new client
            client.SendMessage(region.NetworkPlayers.Select(o => new ServerPlayerInit
            {
                ClientId = o.Value.Id,
                Init = o.Value.PlayerInit,
            }).Package(), PlayerInit.StaticSendMode);

            return true;
        }

        public bool ClientLeaveRegion(IClient client, ClientRegionLeave regionLeave)
        {
            if (!_clientsRegion.TryGetValue(client, out var clientRegion))
                return false;

            _clientsRegion.Remove(client);
            var region = GetRegion(regionLeave.RegionId);
            if (region.Clients.TryGetValue(client, out var thisClient))
                region.Clients.Remove(client);

            region.SendMessageToAllExcept(new ServerPlayerRemove { ClientId = client.ID }.Package(),
                ServerPlayerRemove.StaticSendMode, client);

            return true;
        }

        public bool InitPlayerInRegion(IClient client, ClientPlayerInit clientPlayerInit, out NetworkPlayer networkPlayer)
        {
            networkPlayer = new NetworkPlayer(client.ID)
            {
                Owner = client,
                PlayerInit = clientPlayerInit.Init
            };
            Console.WriteLine($"InitPlayerInRegion 1 Passed");
            if (!_clientsRegion.TryGetValue(client, out Region region))
                return false;
            Console.WriteLine($"InitPlayerInRegion 2 Passed");

            region.NetworkPlayers.Add(client, networkPlayer);
            networkPlayer.Region = region;
            Console.WriteLine($"InitPlayerInRegion 3 Passed");

            region.SendMessageToAllExcept(new ServerPlayerInit
            {
                ClientId = client.ID,
                Init = networkPlayer.PlayerInit
            }.Package(), PlayerInit.StaticSendMode, client);
            Console.WriteLine($"InitPlayerInRegion 4 Passed");

            return true;
        }

        public bool RemoveObjectFromRegion(IClient client, ClientPlayerRemove remove, NetworkPlayer networkPlayer)
        {
            var region = networkPlayer.Region;
            region.NetworkPlayers.Remove(client);
            networkPlayer.Region = null;

            region.SendMessageToAllExcept(new ServerPlayerRemove { ClientId = client.ID }.Package(),
                ServerPlayerRemove.StaticSendMode, client);

            return true;
        }
    }
}

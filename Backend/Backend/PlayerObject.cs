﻿using DarkRift.Server;
using GameModels;
using GameModels.Geometry;
using GameModels.Player;

namespace Backend
{
    public class PlayerObject
    {
        public ushort Id { get; }
        public IClient Owner { get; set; }
        public Region Region { get; set; }
        public FloatVector2 Position { get; set; }
        public PlayerInit PlayerInit { get; set; }

        public PlayerObject(ushort id)
        {
            Id = id;
        }
    }
}
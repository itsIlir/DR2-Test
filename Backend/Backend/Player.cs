using DarkRift.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend
{
    public class Player
    {
        public ushort ID { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public Player(ushort ID, float x, float y)
        {
            this.ID = ID;
            this.X = x;
            this.Y = y;
        }
    }
}

using DarkRift.Server;
using GameModels;
using GameModels.Geometry;
using GameModels.Player;

namespace Backend
{
    public class NetworkPlayer
    {
        public ushort Id { get; }
        public Region Region { get; set; }
        public PlayerMovement PlayerMovement { get; set; }
        public PlayerInit PlayerInit { get; set; }

        public NetworkPlayer(ushort id)
        {
            Id = id;
        }
    }
}

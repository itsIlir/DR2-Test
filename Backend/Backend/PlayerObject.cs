using DarkRift.Server;
using GameModels;

namespace Backend
{
    public class PlayerObject
    {
        public uint Id { get; }
        public LocationData Location;
        public IClient Owner { get; set; }
        public Region Region { get; set; }

        public PlayerObject(uint id)
        {
            Id = id;
        }
    }
}

using DarkRift.Server;
using GameModels;

namespace Backend
{
    public class NetworkObject
    {
        public uint Id { get; }
        public ObjectType Type { get; }
        public LocationData Location;
        public IClient Owner { get; set; }
        public Region Region { get; set; }

        public NetworkObject(uint id, ObjectType type)
        {
            Id = id;
            Type = type;
        }
    }
}

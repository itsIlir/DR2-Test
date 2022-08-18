using GameModels;

namespace Backend
{
    public class NetworkObject
    {
        public uint Id { get; }
        public ObjectType Type { get; }
        public LocationData Location;

        public NetworkObject(uint id, ObjectType type, LocationData location)
        {
            Id = id;
            Type = type;
            Location = location;
        }
    }
}

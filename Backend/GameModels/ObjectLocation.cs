using DarkRift;

namespace GameModels
{
    public struct ObjectLocation : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ObjectLocation;
        public const SendMode StaticSendMode = SendMode.Unreliable;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

        /// The object's Id.
        public uint Id;

        /// Updated object transform.
        public LocationData Location;

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadUInt32();
            Location.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Id);
            Location.Serialize(e);
        }
    }
}

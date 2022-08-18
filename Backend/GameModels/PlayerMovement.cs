using DarkRift;

namespace GameModels
{
    public struct PlayerMovement : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.PlayerMovement;
        public const SendMode StaticSendMode = SendMode.Unreliable;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

        /// The player object's Id.
        public uint Id;

        /// The global input vector.
        public float GlobalInputX, GlobalInputY;

        /// Optional transform update.
        public LocationData Location;

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadUInt32();
            GlobalInputX = e.Reader.ReadSingle();
            GlobalInputY = e.Reader.ReadSingle();
            Location.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Id);
            e.Writer.Write(GlobalInputX);
            e.Writer.Write(GlobalInputY);
            Location.Serialize(e);
        }
    }
}

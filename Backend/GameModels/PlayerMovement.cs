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
        public ushort Id;

        /// The global input vector.
        public float GlobalInputX, GlobalInputY;

        /// Optional transform update.
        public MovementData Movement;

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadUInt16();
            GlobalInputX = e.Reader.ReadSingle();
            GlobalInputY = e.Reader.ReadSingle();
            Movement.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Id);
            e.Writer.Write(GlobalInputX);
            e.Writer.Write(GlobalInputY);
            Movement.Serialize(e);
        }
    }
}

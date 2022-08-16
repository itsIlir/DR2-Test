using DarkRift;

namespace GameModels
{
    public struct ObjectInit : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ObjectInit;
        public const SendMode StaticSendMode = SendMode.Reliable;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

        /// The object's Id.
        public ushort Id;

        /// Initial object transform.
        public MovementData Movement;

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadUInt16();
            Movement.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Id);
            Movement.Serialize(e);
        }
    }
}

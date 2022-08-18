using DarkRift;

namespace GameModels
{
    public struct ObjectRemove : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ObjectRemove;
        public const SendMode StaticSendMode = SendMode.Reliable;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

        /// The object's Id.
        public uint Id;

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadUInt32();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Id);
        }
    }
}

using DarkRift;

namespace GameModels
{
    public struct ObjectRemove : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ObjectRemove;
        public const SendMode StaticSendMode = SendMode.Reliable;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;
        public ushort Id;

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Id);
        }

    }
}

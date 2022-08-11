using DarkRift;

namespace GameModels
{
    public struct ObjectLocation : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ObjectLocation;
        public const SendMode StaticSendMode = SendMode.Unreliable;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

        public ushort Id;
        public float X;
        public float Y;

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadUInt16();
            X = e.Reader.ReadSingle();
            Y = e.Reader.ReadSingle();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Id);
            e.Writer.Write(X);
            e.Writer.Write(Y);
        }
    }
}

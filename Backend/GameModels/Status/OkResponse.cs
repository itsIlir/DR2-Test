using DarkRift;

namespace GameModels.Status
{
    public struct OkResponse : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.Ok;
        public const SendMode StaticSendMode = SendMode.Reliable;

        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

        public NetworkMessageType Request;

        public void Deserialize(DeserializeEvent e)
        {
            Request = (NetworkMessageType)e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write((ushort)Request);
        }
    }
}

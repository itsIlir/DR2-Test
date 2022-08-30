using DarkRift;

namespace GameModels
{
    public struct RegionJoin : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.RegionJoin;
        public const SendMode StaticSendMode = SendMode.Reliable;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

        // The id of the room the client wants to join.
        public uint RegionId;

        public void Deserialize(DeserializeEvent e)
        {
            RegionId = e.Reader.ReadUInt32();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(RegionId);
        }
    }
}

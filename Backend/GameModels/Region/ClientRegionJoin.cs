using DarkRift;

namespace GameModels.Region
{
    public struct ClientRegionJoin : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ClientRegionJoin;
        public const SendMode StaticSendMode = SendMode.Reliable;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

        /// The id of the region the client wants to join.
        public ushort RegionId;

        public void Deserialize(DeserializeEvent e)
        {
            RegionId = e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(RegionId);
        }
    }
}
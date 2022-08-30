using DarkRift;

namespace GameModels
{
    public struct RegionLeave : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.RegionLeave;
        public const SendMode StaticSendMode = SendMode.Reliable;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

        /// The id of the room the client wants to leave.
        public uint RoomId;

        public void Deserialize(DeserializeEvent e)
        {
            RoomId = e.Reader.ReadUInt32();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(RoomId);
        }
    }
}

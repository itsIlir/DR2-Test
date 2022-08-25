using DarkRift;

namespace GameModels
{
    public struct RoomJoin : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.RoomJoin;
        public const SendMode StaticSendMode = SendMode.Reliable;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

        /// The id of the room the client wants to join.
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

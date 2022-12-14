 using DarkRift;

namespace GameModels.Player
{
    public struct ClientPlayerRemove : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ClientPlayerRemove;
        public const SendMode StaticSendMode = SendMode.Reliable;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;
        public bool PlayerLeft; //added just to have a message
        public void Deserialize(DeserializeEvent e)
        {
            PlayerLeft = e.Reader.ReadBoolean();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(PlayerLeft);
        }
    }

    public struct ServerPlayerRemove : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ServerPlayerRemove;
        public const SendMode StaticSendMode = SendMode.Reliable;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

        /// Originator.
        public ushort ClientId;

        public void Deserialize(DeserializeEvent e)
        {
            ClientId = e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(ClientId);
        }
    }
}

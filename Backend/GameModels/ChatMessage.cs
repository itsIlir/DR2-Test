using DarkRift;

namespace GameModels
{
    public struct ChatMessage : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ChatMessage;
        public const SendMode StaticSendMode = SendMode.Reliable;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

        public string ChatText;

        void IDarkRiftSerializable.Deserialize(DeserializeEvent e)
        {
            ChatText = e.Reader.ReadString();
        }

        void IDarkRiftSerializable.Serialize(SerializeEvent e)
        {
            e.Writer.Write(ChatText);
        }
    }
}

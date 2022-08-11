using DarkRift;

namespace GameModels
{
    public struct ChatMessage : INetworkData
    {
        public NetworkMessageType MessageType => NetworkMessageType.ChatMessage;
        public SendMode SendMode => SendMode.Reliable;

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

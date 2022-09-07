using DarkRift;

namespace GameModels
{
    public struct ChatMessage : IDarkRiftSerializable
    {
        public const SendMode StaticSendMode = SendMode.Reliable;

        public string Text;

        public void Deserialize(DeserializeEvent e)
        {
            Text = e.Reader.ReadString();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Text);
        }
    }

    public struct ClientChatMessage : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ClientChatMessage;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => ChatMessage.StaticSendMode;

        /// Chat message contents.
        public ChatMessage Message;

        public void Deserialize(DeserializeEvent e)
        {
            Message.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            Message.Serialize(e);
        }
    }

    public struct ServerChatMessage : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ServerChatMessage;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => ChatMessage.StaticSendMode;

        // Originator.
        public ushort ClientId;

        // Chat message contents.
        public ChatMessage Message;

        public void Deserialize(DeserializeEvent e)
        {
            ClientId = e.Reader.ReadUInt16();
            Message.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(ClientId);
            Message.Serialize(e);
        }
    }
}

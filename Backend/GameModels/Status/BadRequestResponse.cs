using System.Text;
using DarkRift;

namespace GameModels.Status
{
    public struct BadRequestResponse : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.BadRequest;
        public const SendMode StaticSendMode = SendMode.Reliable;

        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

        public NetworkMessageType Request;
        public string Message;

        public void Deserialize(DeserializeEvent e)
        {
            Request = (NetworkMessageType)e.Reader.ReadUInt16();
            Message = e.Reader.ReadString(Encoding.UTF8);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write((ushort)Request);
            e.Writer.Write(Message, Encoding.UTF8);
        }
    }
}


using DarkRift;
using GameModels.Geometry;

namespace GameModels.Player
{
    public struct PlayerInit : IDarkRiftSerializable
    {
        public const SendMode StaticSendMode = SendMode.Reliable;

        /// Initial object transform.
        public FloatVector2 Position;

        public void Deserialize(DeserializeEvent e)
        {
            Position.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            Position.Serialize(e);
        }
    }

    public struct ClientPlayerInit : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ClientPlayerInit;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => PlayerInit.StaticSendMode;

        /// Player's initial data.
        public PlayerInit Init;

        public void Deserialize(DeserializeEvent e)
        {
            Init.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            Init.Serialize(e);
        }
    }

    public struct ServerPlayerInit : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ServerPlayerInit;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => PlayerInit.StaticSendMode;

        /// Originator.
        public ushort ClientId;

        /// Player's initial data.
        public PlayerInit Init;

        public void Deserialize(DeserializeEvent e)
        {
            ClientId = e.Reader.ReadUInt16();
            Init.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(ClientId);
            Init.Serialize(e);
        }
    }
}

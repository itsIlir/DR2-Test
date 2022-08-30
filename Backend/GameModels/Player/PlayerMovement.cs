using DarkRift;
using GameModels.Geometry;

namespace GameModels.Player
{
    public struct PlayerMovement : IDarkRiftSerializable
    {
        public const SendMode StaticSendMode = SendMode.Unreliable;

        /// The global input vector.
        public FloatVector2 GlobalInput;

        public bool UpdatePosition;

        /// Optional position update.
        public FloatVector2 Position;

        public void Deserialize(DeserializeEvent e)
        {
            GlobalInput.Deserialize(e);
            UpdatePosition = e.Reader.ReadBoolean();
            if (UpdatePosition)
                Position.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            GlobalInput.Serialize(e);
            e.Writer.Write(UpdatePosition);
            if (UpdatePosition)
                Position.Serialize(e);
        }
    }

    public struct ClientPlayerMovement : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ClientPlayerMovement;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => PlayerMovement.StaticSendMode;

        /// The player's movement.
        public PlayerMovement Movement;

        public void Deserialize(DeserializeEvent e)
        {
            Movement.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            Movement.Serialize(e);
        }
    }

    public struct ServerPlayerMovement : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ServerPlayerMovement;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => PlayerMovement.StaticSendMode;

        /// Originator.
        public ushort ClientId;

        /// The player's movement.
        public PlayerMovement Movement;

        public void Deserialize(DeserializeEvent e)
        {
            ClientId = e.Reader.ReadUInt16();
            Movement.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(ClientId);
            Movement.Serialize(e);
        }
    }
}

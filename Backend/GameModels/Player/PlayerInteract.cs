using DarkRift;

namespace GameModels.Player
{
    public struct NoTarget : IDarkRiftSerializable
    {
        public void Deserialize(DeserializeEvent e)
        {
        }

        public void Serialize(SerializeEvent e)
        {
        }
    }

    public struct PlayerInteract<T> : IDarkRiftSerializable where T : struct, IDarkRiftSerializable
    {
        public const SendMode StaticSendMode = SendMode.Reliable;

        public PlayerInteractType Interaction;

        public PlayerMovement Movement;

        public T Target;

        public void Deserialize(DeserializeEvent e)
        {
            Interaction = (PlayerInteractType) e.Reader.ReadUInt16();
            Movement.Deserialize(e);
            Target.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write((ushort)Interaction);
            Movement.Serialize(e);
            Target.Serialize(e);
        }
    }

    public struct ClientPlayerInteract : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ClientPlayerInteract;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => PlayerInteract<NoTarget>.StaticSendMode;

        /// The player interaction.
        public PlayerInteract<NoTarget> Interaction;

        public void Deserialize(DeserializeEvent e)
        {
            Interaction.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            Interaction.Serialize(e);
        }
    }

    public struct ServerPlayerInteract : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ServerPlayerInteract;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => PlayerInteract<NoTarget>.StaticSendMode;

        /// Originator.
        public ushort ClientId;

        /// The player interaction.
        public PlayerInteract<NoTarget> Interaction;

        public void Deserialize(DeserializeEvent e)
        {
            ClientId = e.Reader.ReadUInt16();
            Interaction.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(ClientId);
            Interaction.Serialize(e);
        }
    }
}

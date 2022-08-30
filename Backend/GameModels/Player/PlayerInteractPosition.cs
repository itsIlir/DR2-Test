using DarkRift;
using GameModels.Geometry;

namespace GameModels.Player
{
    public struct ClientPlayerInteractPosition
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ClientPlayerInteractPosition;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => PlayerInteract<FloatVector2>.StaticSendMode;

        /// The player interaction (with position).
        public PlayerInteract<FloatVector2> Interaction;

        public void Deserialize(DeserializeEvent e)
        {
            Interaction.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            Interaction.Serialize(e);
        }
    }

    public struct ServerPlayerInteractPosition : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ServerPlayerInteractPosition;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => PlayerInteract<FloatVector2>.StaticSendMode;

        /// Originator.
        public ushort ClientId;

        /// The player interaction (with position).
        public PlayerInteract<FloatVector2> Interaction;

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

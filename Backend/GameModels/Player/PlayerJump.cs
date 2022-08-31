using System;
using DarkRift;

namespace GameModels.Player
{
    public struct PlayerJump : IDarkRiftSerializable
    {
        public const SendMode StaticSendMode = SendMode.Reliable;

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

    public struct ClientPlayerJump : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ClientPlayerJump;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => PlayerJump.StaticSendMode;

        /// The player jump.
        public PlayerJump Jump;

        public void Deserialize(DeserializeEvent e)
        {
            Jump.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            Jump.Serialize(e);
        }
    }

    public struct ServerPlayerJump : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ServerPlayerJump;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => PlayerJump.StaticSendMode;

        /// Originator.
        public ushort ClientId;

        /// The player jump.
        public PlayerJump Jump;

        public void Deserialize(DeserializeEvent e)
        {
            ClientId = e.Reader.ReadUInt16();
            Jump.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(ClientId);
            Jump.Serialize(e);
        }
    }
}

﻿using DarkRift;

namespace GameModels
{
    public struct ObjectLocation : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ObjectLocation;
        public const SendMode StaticSendMode = SendMode.Unreliable;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

        /// The object's Id.
        public ushort Id;

        /// Updated object transform.
        public MovementData Movement;

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadUInt16();
            Movement.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Id);
            Movement.Serialize(e);
        }
    }
}

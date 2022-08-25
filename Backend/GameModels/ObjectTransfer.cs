﻿using DarkRift;

namespace GameModels
{
    public struct ObjectTransfer : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ObjectTransfer;
        public const SendMode StaticSendMode = SendMode.Reliable;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

        /// The object's Id.
        public uint Id;

        /// The object's client's Id.
        public ushort OwnerId;

        /// The id of the room the object should be transferred to.
        public uint RoomId;

        /// The object's type. Player objects cannot be possessed.
        public ObjectType Type;

        /// If the object isn't a player, this object is possessing a world entity.
        public uint PossessId;

        /// Transfer object transform.
        public LocationData Location;

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadUInt32();
            OwnerId = e.Reader.ReadUInt16();
            RoomId = e.Reader.ReadUInt32();
            Type = (ObjectType) e.Reader.ReadByte();
            if (Type != ObjectType.Player)
            {
                PossessId = e.Reader.ReadUInt32();
            }

            Location.Deserialize(e);
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Id);
            e.Writer.Write(OwnerId);
            e.Writer.Write(RoomId);
            e.Writer.Write((byte)Type);
            if (Type != ObjectType.Player)
            {
                e.Writer.Write(PossessId);
            }

            Location.Serialize(e);
        }
    }
}

using DarkRift;

namespace GameModels
{
    public enum ObjectType : byte
    {
        Player,
        Physics,
        NPC,
    }

    public struct ObjectInit : INetworkData
    {
        public const NetworkMessageType StaticMessageType = NetworkMessageType.ObjectInit;
        public const SendMode StaticSendMode = SendMode.Reliable;
        public NetworkMessageType MessageType => StaticMessageType;
        public SendMode SendMode => StaticSendMode;

        /// The object's Id.
        /// Only the host may provide an Id, any Id provided by a client will be ignored.
        public uint Id;

        /// The object's client's Id.
        public ushort OwnerId;

        /// The id of the room the object should be created in.
        public uint RegionId;

        /// The object's type. Player objects cannot be possessed.
        public ObjectType Type;

        /// If the object isn't a player, this object is possessing a world entity.
        public uint PossessId;

        /// Initial object transform.
        public LocationData Location;

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadUInt32();
            OwnerId = e.Reader.ReadUInt16();
            RegionId = e.Reader.ReadUInt32();
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
            e.Writer.Write(RegionId);
            e.Writer.Write((byte)Type);
            if (Type != ObjectType.Player)
            {
                e.Writer.Write(PossessId);
            }

            Location.Serialize(e);
        }
    }
}

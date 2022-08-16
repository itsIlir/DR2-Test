using System;
using DarkRift;

namespace GameModels
{
    [Flags]
    public enum TargetFlags : byte
    {
        Position2D = 1 << 0,
        Position3D = 1 << 1,
        ObjectId = 1 << 2,
    }

    public struct TargetData : IDarkRiftSerializable
    {
        /// Serialization flags.
        public TargetFlags Flags;

        /// Serialized if TargetFlags.Position2D or TargetFlags.Position3D are set.
        public float PositionX, PositionY;
        /// Serialized if TargetFlags.Position3D is set.
        public float PositionZ;

        /// Serialized if TargetFlags.ObjectId is set, TargetId an object's id number.
        public ushort TargetId;

        public void Deserialize(DeserializeEvent e)
        {
            Flags = (TargetFlags) e.Reader.ReadByte();

            if ((Flags & TargetFlags.Position2D) != 0)
            {
                PositionX = e.Reader.ReadSingle();
                PositionY = e.Reader.ReadSingle();
            }

            if ((Flags & TargetFlags.Position3D) != 0)
            {
                PositionX = e.Reader.ReadSingle();
                PositionY = e.Reader.ReadSingle();
                PositionZ = e.Reader.ReadSingle();
            }

            if ((Flags & TargetFlags.ObjectId) != 0)
            {
                TargetId = e.Reader.ReadUInt16();
            }
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write((byte)Flags);

            if ((Flags & TargetFlags.Position2D) != 0)
            {
                e.Writer.Write(PositionX);
                e.Writer.Write(PositionY);
            }

            if ((Flags & TargetFlags.Position3D) != 0)
            {
                e.Writer.Write(PositionX);
                e.Writer.Write(PositionY);
                e.Writer.Write(PositionZ);
            }

            if ((Flags & TargetFlags.ObjectId) != 0)
            {
                e.Writer.Write(TargetId);
            }
        }
    }
}

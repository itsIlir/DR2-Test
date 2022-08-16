using System;
using DarkRift;

namespace GameModels
{
    [Flags]
    public enum MovementFlags : byte
    {
        Position2D = 1 << 0,
        Position3D = 1 << 1,
        Rotation2D = 1 << 2,
        Rotation3D = 1 << 3,
        LinearVelocity2D = 1 << 4,
        LinearVelocity3D = 1 << 5,
        AngularVelocity2D = 1 << 6,
        AngularVelocity3D = 1 << 7,
    }

    public struct MovementData : IDarkRiftSerializable
    {
        /// Serialization flags.
        public MovementFlags Flags;

        /// Serialized if either MovementFlags.Position2D or MovementFlags.Position3D are set.
        public float PositionX, PositionY;
        /// Serialized if MovementFlags.Position3D is set.
        public float PositionZ;

        /// Serialized if MovementFlags.Rotation2D is set.
        public float Angle;
        /// Serialized if MovementFlags.Rotation3D is set.
        public float Pitch, Yaw, Roll;

        /// Serialized if either MovementFlags.LinearVelocity2D or MovementFlags.LinearVelocity3D are set.
        public float LinearVelocityX, LinearVelocityY;
        /// Serialized if MovementFlags.LinearVelocity3D is set.
        public float LinearVelocityZ;

        /// Serialized if either MovementFlags.AngularVelocity2D or MovementFlags.AngularVelocity3D are set.
        public float AngularVelocitySpeed;
        /// Serialized if MovementFlags.AngularVelocity3D is set.
        public float AngularVelocityAxisX, AngularVelocityAxisY, AngularVelocityAxisZ;

        public void Deserialize(DeserializeEvent e)
        {
            Flags = (MovementFlags) e.Reader.ReadByte();

            if ((Flags & MovementFlags.Position2D) != 0)
            {
                PositionX = e.Reader.ReadSingle();
                PositionY = e.Reader.ReadSingle();
            }

            if ((Flags & MovementFlags.Position3D) != 0)
            {
                PositionX = e.Reader.ReadSingle();
                PositionY = e.Reader.ReadSingle();
                PositionZ = e.Reader.ReadSingle();
            }

            if ((Flags & MovementFlags.Rotation2D) != 0)
            {
                Angle = e.Reader.ReadSingle();
            }

            if ((Flags & MovementFlags.Rotation3D) != 0)
            {
                Pitch = e.Reader.ReadSingle();
                Yaw = e.Reader.ReadSingle();
                Roll = e.Reader.ReadSingle();
            }

            if ((Flags & MovementFlags.LinearVelocity2D) != 0)
            {
                LinearVelocityX = e.Reader.ReadSingle();
                LinearVelocityY = e.Reader.ReadSingle();
            }

            if ((Flags & MovementFlags.LinearVelocity3D) != 0)
            {
                LinearVelocityX = e.Reader.ReadSingle();
                LinearVelocityY = e.Reader.ReadSingle();
                LinearVelocityZ = e.Reader.ReadSingle();
            }

            if ((Flags & MovementFlags.AngularVelocity2D) != 0)
            {
                AngularVelocitySpeed = e.Reader.ReadSingle();
            }

            if ((Flags & MovementFlags.AngularVelocity3D) != 0)
            {
                AngularVelocitySpeed = e.Reader.ReadSingle();
                AngularVelocityAxisX = e.Reader.ReadSingle();
                AngularVelocityAxisY = e.Reader.ReadSingle();
                AngularVelocityAxisZ = e.Reader.ReadSingle();
            }
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write((byte)Flags);
            if ((Flags & MovementFlags.Position2D) != 0)
            {
                e.Writer.Write(PositionX);
                e.Writer.Write(PositionY);
            }

            if ((Flags & MovementFlags.Position3D) != 0)
            {
                e.Writer.Write(PositionX);
                e.Writer.Write(PositionY);
                e.Writer.Write(PositionZ);
            }

            if ((Flags & MovementFlags.Rotation2D) != 0)
            {
                e.Writer.Write(Angle);
            }

            if ((Flags & MovementFlags.Rotation3D) != 0)
            {
                e.Writer.Write(Pitch);
                e.Writer.Write(Yaw);
                e.Writer.Write(Roll);
            }

            if ((Flags & MovementFlags.LinearVelocity2D) != 0)
            {
                e.Writer.Write(LinearVelocityX);
                e.Writer.Write(LinearVelocityY);
            }

            if ((Flags & MovementFlags.LinearVelocity3D) != 0)
            {
                e.Writer.Write(LinearVelocityX);
                e.Writer.Write(LinearVelocityY);
                e.Writer.Write(LinearVelocityZ);
            }

            if ((Flags & MovementFlags.AngularVelocity2D) != 0)
            {
                e.Writer.Write(AngularVelocitySpeed);
            }

            if ((Flags & MovementFlags.AngularVelocity3D) != 0)
            {
                e.Writer.Write(AngularVelocitySpeed);
                e.Writer.Write(AngularVelocityAxisX);
                e.Writer.Write(AngularVelocityAxisY);
                e.Writer.Write(AngularVelocityAxisZ);
            }
        }
    }
}

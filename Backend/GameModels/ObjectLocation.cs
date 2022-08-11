using DarkRift;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels
{
    public struct ObjectLocation : INetworkData
    {
        public NetworkMessageType MessageType => NetworkMessageType.ObjectLocation;
        public SendMode SendMode => SendMode.Unreliable;

        public ushort Id;
        public float X;
        public float Y;

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadUInt16();
            X = e.Reader.ReadSingle();
            Y = e.Reader.ReadSingle();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Id);
            e.Writer.Write(X);
            e.Writer.Write(Y);
        }
    }
}

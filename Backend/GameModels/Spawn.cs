using DarkRift;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels
{
    public struct Spawn : IDarkRiftSerializable
    {
        public ushort ID;
        public float X;
        public float Y;

        public void Deserialize(DeserializeEvent e)
        {
            ID = e.Reader.ReadUInt16();
            X = e.Reader.ReadSingle();
            Y = e.Reader.ReadSingle();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(ID);
            e.Writer.Write(X); 
            e.Writer.Write(Y);
        }
    }
}

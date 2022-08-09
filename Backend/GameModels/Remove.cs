using DarkRift;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels
{
    public class Remove : IDarkRiftSerializable
    {
        public ushort ID;

        public void Deserialize(DeserializeEvent e)
        {
            ID = e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(ID);
        }
    }
}

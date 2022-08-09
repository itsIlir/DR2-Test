using DarkRift;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameModels
{
    public class Chat : IDarkRiftSerializable
    {
        public string chatMsg;

        public void Deserialize(DeserializeEvent e)
        {
            chatMsg = e.Reader.ReadString();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(chatMsg);
        }
    }
}

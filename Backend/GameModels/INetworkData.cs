using System.Collections.Generic;
using DarkRift;

namespace GameModels
{
    public interface INetworkData : IDarkRiftSerializable
    {
        NetworkMessageType MessageType { get; }
        SendMode SendMode { get; }
    }

    public static class NetworkMessageExtensions
    {
        public static Message Package<T>(this T message) where T : struct, INetworkData =>
            Message.Create((ushort)message.MessageType, message);

        public static Message Package<T>(this IEnumerable<T> messages) where T : struct, INetworkData
        {
            using var writer = DarkRiftWriter.Create();
            foreach (var message in messages)
                writer.Write(message);
            return Message.Create((ushort)default(T).MessageType, writer);
        }
    }
}

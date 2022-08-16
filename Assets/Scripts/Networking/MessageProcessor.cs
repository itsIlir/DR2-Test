using System;
using System.Collections.Concurrent;
using DarkRift;
using GameModels;

namespace Networking
{
    internal interface IMessageProcessor
    {
        void ProcessMessages();
        void EnqueueMessages(DarkRiftReader reader);
        void ClearMessageHandlers();
    }

    public sealed class MessageProcessor<T> : IMessageProcessor where T : struct, INetworkData
    {
        public event Action<T> OnMessage;
        private readonly ConcurrentQueue<T> _messageQueue = new ConcurrentQueue<T>();

        public void ProcessMessages()
        {
            if (OnMessage == null)
                return;

            while (_messageQueue.TryDequeue(out var message))
                OnMessage(message);
        }

        public void EnqueueMessages(DarkRiftReader reader)
        {
            while (reader.Position < reader.Length)
                _messageQueue.Enqueue(reader.ReadSerializable<T>());
        }

        public void ClearMessageHandlers()
        {
            OnMessage = null;
        }
    }
}

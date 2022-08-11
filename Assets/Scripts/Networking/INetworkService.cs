using System;
using System.Collections.Generic;
using DarkRift;
using GameModels;

namespace Networking
{
    public interface INetworkService
    {
        MessageProcessor<T> GetProcessor<T>() where T : struct, INetworkData;
        void Connect();
        void Disconnect();
        void SendMessage<T>(T networkMessage) where T : struct, INetworkData;
        void SendMessages<T>(IEnumerable<T> networkMessage) where T : struct, INetworkData;
    }

    interface IMessageProcessor
    {
        void ProcessMessage(DarkRiftReader reader);
        void ClearMessageEvents();
    }

    public sealed class MessageProcessor<T> : IMessageProcessor where T : struct, INetworkData
    {
        public event Action<T> OnMessage;

        public void ProcessMessage(DarkRiftReader reader)
        {
            if (OnMessage == null)
                return;

            while (reader.Position < reader.Length)
                OnMessage(reader.ReadSerializable<T>());
        }

        public void ClearMessageEvents()
        {
            OnMessage = null;
        }
    }
}

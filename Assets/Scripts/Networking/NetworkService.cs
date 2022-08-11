using System;
using System.Collections.Generic;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using GameModels;
using UnityEngine;

namespace Networking
{
    public class NetworkService : MonoBehaviour, INetworkService
    {
        [SerializeField]
        UnityClient _client;

        readonly Dictionary<NetworkMessageType, IMessageProcessor>
            MessageProcessors = new Dictionary<NetworkMessageType, IMessageProcessor>();

        public UnityClient Client => _client;

        public MessageProcessor<T> GetProcessor<T>() where T : struct, INetworkData
        {
            var messageType = default(T).MessageType;
            if (!MessageProcessors.TryGetValue(messageType, out var processor))
            {
                processor = new MessageProcessor<T>();
                MessageProcessors.Add(messageType, processor);
            }
            return processor as MessageProcessor<T>;
        }

        public void Connect()
        {
            Client.MessageReceived += OnMessageReceived;
            Client.Disconnected += OnDisconnected;
        }

        public void Disconnect()
        {
            Client.MessageReceived -= OnMessageReceived;
            Client.Disconnected -= OnDisconnected;

            // Disconnect *after* unhooking the events to avoid the disconnected event to loop.
            if (Client.ConnectionState == ConnectionState.Connected ||
                Client.ConnectionState == ConnectionState.Connecting ||
                Client.ConnectionState == ConnectionState.Interrupted)
                Client.Disconnect();

            foreach (var processor in MessageProcessors.Values)
                processor.ClearMessageEvents();
        }

        public void SendMessage<T>(T networkMessage) where T : struct, INetworkData
        {
            using var message = networkMessage.Package();
            Client.SendMessage(message, networkMessage.SendMode);
        }

        public void SendMessages<T>(IEnumerable<T> networkMessage) where T : struct, INetworkData
        {
            using var message = networkMessage.Package();
            Client.SendMessage(message, default(T).SendMode);
        }

        void OnDisconnected(object sender, DisconnectedEventArgs e)
            => Disconnect();

        void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            using var message = e.GetMessage();
            var messageType = (NetworkMessageType) message.Tag;
            if (!MessageProcessors.TryGetValue(messageType, out var processor))
            {
                Debug.LogWarning($"No processor for message type {messageType}");
                return;
            }

            using var reader = message.GetReader();
            processor.ProcessMessage(reader);
        }
    }
}

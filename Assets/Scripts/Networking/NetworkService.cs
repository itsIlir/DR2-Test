using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DarkRift;
using DarkRift.Client;
using GameModels;
using UnityEngine;

namespace Networking
{
    public class NetworkService : MonoBehaviour, INetworkService
    {
        readonly Dictionary<NetworkMessageType, IMessageProcessor>
            MessageProcessors = new Dictionary<NetworkMessageType, IMessageProcessor>();

        readonly HashSet<NetworkMessageType> UnhandledMessageTypes = new HashSet<NetworkMessageType>();

        bool _mayProcessMessages = false;

        public DarkRiftClient Client { get; private set; }

        public MessageProcessor<T> GetProcessor<T>() where T : struct, INetworkData
        {
            var messageType = default(T).MessageType;
            if (!MessageProcessors.TryGetValue(messageType, out var processor))
            {
                processor = new MessageProcessor<T>();
                MessageProcessors.Add(messageType, processor);

                if (UnhandledMessageTypes.Remove(messageType))
                {
                    Debug.LogWarning($"Messages for {messageType} was previously unhandled. Did the processor register too late?");
                }
            }
            return processor as MessageProcessor<T>;
        }

        private void Awake()
        {
            Client = new DarkRiftClient();
        }

        private void Update()
        {
            if (!_mayProcessMessages)
                return;

            foreach (var processor in MessageProcessors.Values)
                processor.ProcessMessages();
        }

        public async Task Connect(IPAddress ip, int port)
        {
            var connectionTask = new UniTaskCompletionSource();
            Client.MessageReceived += OnMessageReceived;
            Client.Disconnected += OnDisconnected;
            Client.ConnectInBackground(ip, port, false, exception =>
            {
                if (exception == null)
                    connectionTask.TrySetResult();
                else
                    connectionTask.TrySetException(exception);
            });
            await connectionTask.Task;

            // The await on the callback will be called on a separate thread.
            // For the MessageProcessors to behave logically, we need to return to main thread before processing.
            await UniTask.SwitchToMainThread();
            _mayProcessMessages = true;
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

            _mayProcessMessages = false;

            foreach (var processor in MessageProcessors.Values)
            {
                processor.ProcessMessages();
                processor.ClearMessageHandlers();
            }
        }

        public void SendMessage<T>(T networkMessage) where T : struct, INetworkData
        {
            using var message = networkMessage.Package();
            Client.SendMessage(message, networkMessage.SendMode);
        }

        public void SendMessages<T>(IEnumerable<T> networkMessages) where T : struct, INetworkData
        {
            using var message = networkMessages.Package();
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
                // Send a single warning message, ignore if previously warned to avoid spam.
                if (UnhandledMessageTypes.Add(messageType))
                {
                    Debug.LogWarning($"No processor for message type {messageType}");
                }

                return;
            }

            using var reader = message.GetReader();
            processor.EnqueueMessages(reader);
        }
    }
}

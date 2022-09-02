using System;
using GameModels;
using Networking;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class ChatManager : MonoBehaviour
    {
        [SerializeField] private InputField _input;

        [SerializeField] private Text _chatText;

        private INetworkService _networkService;

        public event Action<ClientChatMessage> OnSendMessage;

        private void Awake()
        {
            _networkService = ServiceLocator<INetworkService>.Get();
        }

        private void OnEnable()
        {
            _input.onEndEdit.AddListener(OnInputMessage);
            _networkService.GetProcessor<ServerChatMessage>().OnMessage += OnReceiveMessage;
        }

        private void OnDisable()
        {
            _input.onEndEdit.RemoveListener(OnInputMessage);
            _networkService.GetProcessor<ServerChatMessage>().OnMessage -= OnReceiveMessage;
        }

        private void OnInputMessage(string text)
        {
            _input.SetTextWithoutNotify("");
            var chatMessage = new ClientChatMessage
            {
                Message = new ChatMessage
                {
                    Text = text,
                },
            };
            _networkService.SendMessage(chatMessage);
            //OnSendMessage?.Invoke(chatMessage);
            AddMessageToChatLog(_networkService.Client.ID, text);
        }

        public void OnReceiveMessage(ServerChatMessage message)
        {
            Debug.Log($"message recived {message.Message.Text} from client {message.ClientId}");
            AddMessageToChatLog(message.ClientId, message.Message.Text);
        }

        private void AddMessageToChatLog(ushort sender, string message)
        {
            _chatText.text += $"{sender}: {message}\n";
        }
    }
}

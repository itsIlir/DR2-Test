using System;
using GameModels;
using UnityEngine;
using UnityEngine.UI;

namespace DR2Test.Network
{
    public class ChatManager : MonoBehaviour
    {
        [SerializeField] private InputField _input;

        [SerializeField] private Text _chatText;

        public event Action<ChatMessage> OnSendMessage;

        private void OnEnable()
        {
            _input.onEndEdit.AddListener(OnInputMessage);
        }

        private void OnDisable()
        {
            _input.onEndEdit.RemoveListener(OnInputMessage);
        }

        void OnInputMessage(string message)
        {
            _input.SetTextWithoutNotify("");
            var chatMessage = new ChatMessage
            {
                ChatText = message,
            };
            OnSendMessage?.Invoke(chatMessage);
            ReceiveMessage(chatMessage);
        }

        public void ReceiveMessage(ChatMessage message)
        {
            _chatText.text += "\n" + message.ChatText;
        }
    }
}

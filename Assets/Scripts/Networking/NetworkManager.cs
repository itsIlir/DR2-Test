using System.Collections;
using System.Net;
using UnityEngine;
using DarkRift;
using GameModels;
using Gameplay;
using Services;

namespace Networking
{
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField]
        ObjectManager _objectManager;

        [SerializeField]
        PlayerManager _playerManager;

        [SerializeField]
        ChatManager _chatManager;

        INetworkService _networkService;

        private void Awake()
        {
            Application.runInBackground = true;
            Application.targetFrameRate = 20;

            _networkService = ServiceLocator<INetworkService>.Get();
            _networkService.GetProcessor<ChatMessage>().OnMessage += _chatManager.ReceiveMessage;
            _chatManager.OnSendMessage += _networkService.SendMessage;
        }

        private async void Start()
        {
            await _networkService.Connect(IPAddress.Parse("127.0.0.1"), 4296);
            Debug.Log($"Connected! Client ID {_networkService.Client.ID}");
            _objectManager.LocalClientId = _networkService.Client.ID;
        }

        private void OnDestroy()
        {
            _networkService.Disconnect();
            _chatManager.OnSendMessage -= _networkService.SendMessage;
        }
    }
}

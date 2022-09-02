using System.Collections;
using System.Net;
using UnityEngine;
using DarkRift;
using GameModels;
using GameModels.Player;
using GameModels.Region;
using GameModels.Unity;
using Gameplay;
using Services;

namespace Networking
{
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField]
        PlayerManager _playerManager;

        [SerializeField]
        ChatManager _chatManager;

        INetworkService _networkService;

        private void Awake()
        {
            Application.runInBackground = true;
            Application.targetFrameRate = Screen.currentResolution.refreshRate * 2;

            _networkService = ServiceLocator<INetworkService>.Get();

            //_networkService.GetProcessor<ServerChatMessage>().OnMessage += _chatManager.OnReceiveMessage;
            _chatManager.OnSendMessage += _networkService.SendMessage;
        }

        private async void Start()
        {
            await _networkService.Connect(IPAddress.Parse("127.0.0.1"), 4296);
            Debug.Log($"Connected! Client ID {_networkService.Client.ID}");

            _networkService.SendMessage(new ClientRegionJoin()
            {
                RegionId = 10,
            });
            _playerManager.ConnectLocalPlayer();
        }

        private void OnDestroy()
        {
            _networkService.Disconnect();
            _chatManager.OnSendMessage -= _networkService.SendMessage;
        }
    }
}

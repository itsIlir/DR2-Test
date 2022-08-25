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
        ObjectService _objectService;

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

            _networkService.GetProcessor<ChatMessage>().OnMessage += _chatManager.ReceiveMessage;
            _chatManager.OnSendMessage += _networkService.SendMessage;
        }

        private async void Start()
        {
            await _networkService.Connect(IPAddress.Parse("127.0.0.1"), 4296);
            Debug.Log($"Connected! Client ID {_networkService.Client.ID}");
            _objectService.LocalClientId = _networkService.Client.ID;

            _networkService.SendMessage(new RoomJoin
            {
                RoomId = 10,
            });

            var localPlayerPosition = _playerManager.LocalPlayer.transform.position;
            _networkService.SendMessage(new ObjectInit
            {
                OwnerId = _networkService.Client.ID,
                Type = ObjectType.Player,
                RoomId = 10,
                Location = new LocationData
                {
                    Flags = MovementFlags.Position2D,
                    PositionX = localPlayerPosition.x,
                    PositionY = localPlayerPosition.y,
                },
            });
        }

        private void OnDestroy()
        {
            _networkService.Disconnect();
            _chatManager.OnSendMessage -= _networkService.SendMessage;
        }
    }
}

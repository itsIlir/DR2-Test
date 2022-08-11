using System.Collections;
using UnityEngine;
using DarkRift.Client.Unity;
using DarkRift;
using GameModels;
using DarkRift.Client;
using Gameplay;
using Services;

namespace Networking
{
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField]
        ObjectHandler _objectHandler;

        [SerializeField]
        ChatManager _chatManager;

        INetworkService _networkService;

        public void Awake()
        {
            Application.runInBackground = true;

            _networkService = ServiceLocator<INetworkService>.Get();
            _networkService.Client.MessageReceived += InitialMessageReceived;

            _networkService.Connect();

            _networkService.GetProcessor<ObjectInit>().OnMessage += _objectHandler.OnObjectInit;
            _networkService.GetProcessor<ObjectLocation>().OnMessage += _objectHandler.OnObjectLocation;
            _networkService.GetProcessor<ObjectRemove>().OnMessage += _objectHandler.OnObjectRemove;
            _networkService.GetProcessor<ChatMessage>().OnMessage += _chatManager.ReceiveMessage;
            _chatManager.OnSendMessage += _networkService.SendMessage;
        }

        private void InitialMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var client = _networkService.Client;
            Debug.Log($"Connected! Client ID {client.ID}");
            _objectHandler.LocalClientId = client.ID;
            client.MessageReceived -= InitialMessageReceived;
            StartCoroutine(UpdateLocalPlayerLocation());
        }

        private IEnumerator UpdateLocalPlayerLocation()
        {
            var sendDelay = new WaitForSecondsRealtime(0.01f);
            var client = _networkService.Client;
            while (client.ConnectionState == ConnectionState.Connected)
            {
                if (_objectHandler.ObjectExists(client.ID))
                    _networkService.SendMessage(_objectHandler.GetObjectLocation(client.ID));
                yield return sendDelay;
            }
        }

        private void OnDestroy()
        {
            _networkService.Disconnect();
            _chatManager.OnSendMessage -= _networkService.SendMessage;
        }
    }
}

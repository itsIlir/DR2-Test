using System.Collections;
using UnityEngine;
using DarkRift.Client.Unity;
using DarkRift;
using GameModels;
using DarkRift.Client;
using DR2Test.Network;

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    UnityClient client;

    [SerializeField]
    ObjectHandler _objectHandler;

    [SerializeField]
    ChatManager _chatManager;

    NetworkService _networkService;

    public void Awake()
    {
        Application.runInBackground = true;

        if (client == null)
        {
            Debug.LogError("Client unassigned in PlayerSpawner.");
            Application.Quit();
        }

        client.MessageReceived += InitialMessageReceived;

        _networkService = new NetworkService(client);
        _networkService.Connect();

        _networkService.GetProcessor<ObjectInit>().OnMessage += _objectHandler.OnObjectInit;
        _networkService.GetProcessor<ObjectLocation>().OnMessage += _objectHandler.OnObjectLocation;
        _networkService.GetProcessor<ObjectRemove>().OnMessage += _objectHandler.OnObjectRemove;
        _networkService.GetProcessor<ChatMessage>().OnMessage += _chatManager.ReceiveMessage;
        _chatManager.OnSendMessage += _networkService.SendMessage;
    }

    private void InitialMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log($"Connected! Client ID {client.ID}");
        _objectHandler.LocalClientId = client.ID;
        client.MessageReceived -= InitialMessageReceived;
        StartCoroutine(UpdateLocalPlayerLocation());
    }

    private IEnumerator UpdateLocalPlayerLocation()
    {
        var sendDelay = new WaitForSecondsRealtime(0.01f);
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

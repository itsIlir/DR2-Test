using System.Collections.Generic;
using UnityEngine;
using DarkRift.Client.Unity;
using DarkRift;
using GameModels;
using DarkRift.Client;
using System;

public class NetworkService : MonoBehaviour, INetworkService
{
    [SerializeField]
    UnityClient client;

    int _networkID;

    public event Action<object, MessageReceivedEventArgs> OnMessageRecived;

    public int NetworkID
    {
        get => _networkID;
        set => _networkID = value;
    }

    void Awake()
    {
        if (client == null)
        {
            Debug.LogError("Client unassigned in PlayerSpawner.");
            Application.Quit();
        }
        SetNetworkID();
        client.MessageReceived += MessageRecived;
    }

    void OnDestroy()
    {
        client.MessageReceived -= MessageRecived;
    }

    void MessageRecived(object sender, MessageReceivedEventArgs e)
    {
        OnMessageRecived?.Invoke(sender, e);
    }

    void SetNetworkID()
    {
        NetworkID = client.ID;
    }

    public void SendTextMessage(string inputedText)
    {
        Chat chat = new Chat();
        chat.chatMsg = inputedText;
        using (Message message = Message.Create((ushort)Tags.Tag.TEXT_MSG, chat))
        {
            client.SendMessage(message, SendMode.Reliable);
        }
    }
}

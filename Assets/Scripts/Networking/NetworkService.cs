using System.Collections.Generic;
using UnityEngine;
using DarkRift.Client.Unity;
using DarkRift;
using GameModels;
using DarkRift.Client;
using System;

public class NetworkService : MonoBehaviour, INetworkService
{
    public event Action<object, MessageReceivedEventArgs> OnMessageRecived;

    [SerializeField]
    UnityClient client;

    public int NetworkID => client.ID;

    void Awake()
    {
        if (client == null)
        {
            Debug.LogError("Client unassigned in PlayerSpawner.");
            Application.Quit();
        }
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

    public void SendTextMessage(string inputedText)
    {
        Chat chat = new Chat();
        chat.chatMsg = inputedText;
        using (Message message = Message.Create((ushort)Tags.Tag.TEXT_MSG, chat))
        {
            client.SendMessage(message, SendMode.Reliable);
        }
    }

    public void SendMoveMessage(Vector2 position)
    {
        Move move = new Move();
        move.X = position.x;
        move.Y = position.y;
        move.ID = client.ID;

        using (Message message = Message.Create((ushort)Tags.Tag.PLAYER_MOVE, move))
        {
            client.SendMessage(message, SendMode.Unreliable);
        }
    }

    public DarkRiftReader ReadMessage(MessageReceivedEventArgs e, out Message msg)
    {
        using (Message message = e.GetMessage())
        {
            msg = message;
            using (DarkRiftReader reader = message.GetReader())
            {
                return reader;
            }
        }
    }
}

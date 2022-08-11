using System.Collections.Generic;
using UnityEngine;
using DarkRift.Client.Unity;
using DarkRift;
using GameModels;
using DarkRift.Client;
using System;

public class NetworkService : MonoBehaviour, INetworkService
{
    public event Action<object, DarkRiftReader, Message> OnMessageRecived;

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
        DarkRiftReader reader = ReadMessage(e, out var message);
        OnMessageRecived?.Invoke(sender, reader, message);
    }

    DarkRiftReader ReadMessage(MessageReceivedEventArgs e, out Message msg)
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

    public CubeMovement ReadSpawn(DarkRiftReader reader, GameObject controllablePrefab, GameObject networkPrefab, out ushort id)
    {
        Spawn spawn = reader.ReadSerializable<Spawn>();
        id = spawn.ID;
        Vector3 position = new Vector3(spawn.X, spawn.Y, 10);

        GameObject gameObject;
        if (id == NetworkID)
            gameObject = Instantiate(controllablePrefab, position, Quaternion.identity);
        else
            gameObject = Instantiate(networkPrefab, position, Quaternion.identity);

        return gameObject.GetComponent<CubeMovement>();
    }

    public string ReadChat(DarkRiftReader reader)
    {
        Chat chat = reader.ReadSerializable<Chat>();
        return chat.chatMsg;
    }

    public void ReadMovement(DarkRiftReader reader, Dictionary<ushort, CubeMovement> players)
    {
        Move move = reader.ReadSerializable<Move>();
        if (players.TryGetValue(move.ID, out var player))
        {
            Vector2 moveVector = new Vector2(move.X, move.Y);
            player.PlayerMove(moveVector);
        }
    }

    public void ReadRemove(DarkRiftReader reader, Dictionary<ushort, CubeMovement> players)
    {
        Remove remove = reader.ReadSerializable<Remove>();
        if (players.TryGetValue(remove.ID, out var player))
            Destroy(player.gameObject);
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
}

using DarkRift;
using DarkRift.Client;
using System;
using UnityEngine;

public interface INetworkService
{
    event Action<object, DarkRiftReader, Message> OnMessageRecived;

    int NetworkID { get; }

    void SendTextMessage(string inputedText);

    void SendMoveMessage(Vector2 position);
}
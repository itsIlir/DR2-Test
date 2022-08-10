using DarkRift.Client;
using System;
using UnityEngine;

public interface INetworkService
{
    event Action<object, MessageReceivedEventArgs> OnMessageRecived;

    int NetworkID { get; }

    void SendTextMessage(string inputedText);

    void SendMoveMessage(Vector2 position);
}
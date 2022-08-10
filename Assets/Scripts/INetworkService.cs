using DarkRift.Client;
using System;

public interface INetworkService
{
    event Action<object, MessageReceivedEventArgs> OnMessageRecived;

    int NetworkID { get; }

    void SendTextMessage(string inputedText);

}
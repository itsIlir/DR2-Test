using DarkRift;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface INetworkService
{
    event Action<object, DarkRiftReader, Message> OnMessageRecived;

    int NetworkID { get; }

    void SendTextMessage(string inputedText);

    void SendMoveMessage(Vector2 position);

    CubeMovement ReadSpawn(DarkRiftReader reader, GameObject controllablePrefab, GameObject networkPrefab, out ushort id);

    string ReadChat(DarkRiftReader reader);

    void ReadMovement(DarkRiftReader reader, Dictionary<ushort, CubeMovement> players);

    void ReadRemove(DarkRiftReader reader, Dictionary<ushort, CubeMovement> players);
}
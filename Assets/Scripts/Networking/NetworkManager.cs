using System.Collections.Generic;
using UnityEngine;
using DarkRift;
using GameModels;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    GameObject controllablePrefab, networkPrefab;

    [SerializeField]
    InputField inputField;

    [SerializeField]
    Text textToshow;

    readonly Dictionary<ushort, CubeMovement> players = new Dictionary<ushort, CubeMovement>();

    void Awake()
    {
        Application.runInBackground = true;

        if (controllablePrefab == null)
            Debug.LogError("Controllable Prefab unassigned in PlayerSpawner.");

        if (networkPrefab == null)
            Debug.LogError("Network Prefab unassigned in PlayerSpawner.");

        ServiceLocator<INetworkService>.Get().OnMessageRecived += OnMessageRecived;
    }

    void OnDestroy()
    {
        ServiceLocator<INetworkService>.Get().OnMessageRecived -= OnMessageRecived;
    }

    public void OnButtonClick()
    {
        ServiceLocator<INetworkService>.Get().SendTextMessage(inputField.text);
    }

    public void OnPlayerMove(Vector2 moveVector)
    {
        ServiceLocator<INetworkService>.Get().SendMoveMessage(moveVector);
    }

    void OnMessageRecived(object sender, DarkRiftReader reader, Message message)
    {
        switch (message.Tag)
        {
            case (ushort)Tags.Tag.SPAWN_PLAYER:
                while (reader.Position < reader.Length)
                {
                    CubeMovement gameObject = ServiceLocator<INetworkService>.Get().ReadSpawn(
                        reader, controllablePrefab, networkPrefab, out var id);
                    players.Add(id, gameObject);
                }
                break;

            case (ushort)Tags.Tag.TEXT_MSG:
                while (reader.Position < reader.Length)
                    textToshow.text = ServiceLocator<INetworkService>.Get().ReadChat(reader);
                break;

            case (ushort)Tags.Tag.PLAYER_MOVE:
                while (reader.Position < reader.Length)
                    ServiceLocator<INetworkService>.Get().ReadMovement(reader, players);
                break;

            case (ushort)Tags.Tag.PLAYER_REMOVE:
                while(reader.Position < reader.Length)
                    ServiceLocator<INetworkService>.Get().ReadRemove(reader, players);
                break;
        }
    }
}

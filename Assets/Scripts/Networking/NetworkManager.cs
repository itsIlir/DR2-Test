using System.Collections.Generic;
using UnityEngine;
using DarkRift.Client.Unity;
using DarkRift;
using GameModels;
using DarkRift.Client;
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
        {
            Debug.LogError("Controllable Prefab unassigned in PlayerSpawner.");
            Application.Quit();
        }

        if (networkPrefab == null)
        {
            Debug.LogError("Network Prefab unassigned in PlayerSpawner.");
            Application.Quit();
        }

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
                Debug.Log("AAA reader.Position: " + reader.Position);
                while (reader.Position < reader.Length)
                {
                    
                    Spawn spawn = reader.ReadSerializable<Spawn>();
                    ushort id = spawn.ID;
                    Vector3 position = new Vector3(spawn.X, spawn.Y, 10);

                    GameObject gameObject;
                    if (id == ServiceLocator<INetworkService>.Get().NetworkID)
                        gameObject = Instantiate(controllablePrefab, position, Quaternion.identity);
                    else
                        gameObject = Instantiate(networkPrefab, position, Quaternion.identity);

                    players.Add(id, gameObject.GetComponent<CubeMovement>());
                }
                break;
            case (ushort)Tags.Tag.TEXT_MSG:
                while (reader.Position < reader.Length)
                {
                    Chat chat = reader.ReadSerializable<Chat>();
                    textToshow.text = chat.chatMsg;
                }
                break;
            case (ushort)Tags.Tag.PLAYER_MOVE:
                while (reader.Position < reader.Length)
                {
                    Move move = reader.ReadSerializable<Move>();
                    if (players.TryGetValue(move.ID, out var player))
                    {
                        Vector2 moveVector = new Vector2(move.X, move.Y);
                        player.PlayerMove(moveVector);
                    }
                }
                break;
            case (ushort)Tags.Tag.PLAYER_REMOVE:
                while(reader.Position < reader.Length)
                {
                    Remove remove = reader.ReadSerializable<Remove>();
                    if(players.TryGetValue(remove.ID, out var player))
                        Destroy(player.gameObject);
                }
                break;
        }
    }
}

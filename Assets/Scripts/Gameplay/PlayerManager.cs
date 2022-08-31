using System;
using System.Collections;
using System.Collections.Generic;
using DarkRift;
using GameModels.Geometry;
using GameModels.Player;
using GameModels.Unity;
using Networking;
using Services;
using UnityEngine;

namespace Gameplay
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField]
        private PlayerController _controllablePrefab, _networkPrefab;

        private readonly Dictionary<ushort, PlayerController> _networkPlayers = new Dictionary<ushort, PlayerController>();

        private PlayerController _localPlayer = null;
        private INetworkService _networkService;
        private ushort LocalClientId => _networkService.Client.ID;
        private Coroutine _localPlayerUpdateLoop;

        public PlayerController LocalPlayer => _localPlayer;

        public event Action OnLocalPlayerNetworkInit, OnLocalPlayerNetworkRemove;


        private void Awake()
        {
            _localPlayer = Instantiate(_controllablePrefab);

            _networkService = ServiceLocator<INetworkService>.Get();

            _networkService.GetProcessor<ServerPlayerInit>().OnMessage += OnPlayerInit;
            _networkService.GetProcessor<ServerPlayerRemove>().OnMessage += OnPlayerRemove;
            _networkService.GetProcessor<ServerPlayerMovement>().OnMessage += OnPlayerMovement;
            _networkService.GetProcessor<ServerPlayerJump>().OnMessage += OnPlayerJump;

            OnLocalPlayerNetworkInit += () => _localPlayerUpdateLoop = StartCoroutine(PlayerMovementNetworkLoop());
            OnLocalPlayerNetworkRemove += () => StopCoroutine(_localPlayerUpdateLoop);
        }

        private IEnumerator PlayerMovementNetworkLoop()
        {
            const float positionUpdateTime = 1f;
            var nextPositionUpdate = 0f;
            var sendDelay = new WaitForSecondsRealtime(0.1f);

            var playerMovement = new ClientPlayerMovement();
            while (_networkService.Client.ConnectionState == ConnectionState.Connected)
            {
                var lastSentInputVector = playerMovement.Movement.GlobalInput.AsVector2();
                var needPositionUpdate = Time.time > nextPositionUpdate;
                if (needPositionUpdate || Vector2.Distance(lastSentInputVector, _localPlayer.InputVector) > 0.01f)
                {
                    playerMovement.Movement.GlobalInput = _localPlayer.InputVector.AsFloatVector2();
                    playerMovement.Movement.UpdatePosition = needPositionUpdate;
                    if (needPositionUpdate)
                    {
                        playerMovement.Movement.Position = _localPlayer.Position.AsFloatVector2();
                        nextPositionUpdate = Time.time + positionUpdateTime;
                    }
                    _networkService.SendMessage(playerMovement);
                }

                yield return sendDelay;
            }
        }

        public void ConnectLocalPlayer()
        {
            if (_networkPlayers.ContainsKey(_networkService.Client.ID))
                return;

            _networkService.SendMessage(new ClientPlayerInit
            {
                Init = new PlayerInit
                {
                    Position = LocalPlayer.Position.AsFloatVector2(),
                },
            });
        }

        public void DisconnectLocalPlayer()
        {
            if (!_networkPlayers.ContainsKey(_networkService.Client.ID))
                return;


            _networkService.SendMessage(new ClientPlayerRemove());
        }

        private void OnPlayerInit(ServerPlayerInit serverInit)
        {
            if (_networkPlayers.ContainsKey(serverInit.ClientId))
            {
                Debug.LogWarning("Tried to initialize existing player!");
                return;
            }

            var localPlayer = serverInit.ClientId == LocalClientId;
            var player = localPlayer
                ? _localPlayer
                : Instantiate(_networkPrefab, serverInit.Init.Position.AsVector2(), Quaternion.identity);
            _networkPlayers.Add(serverInit.ClientId, player);

            if (localPlayer)
                OnLocalPlayerNetworkInit?.Invoke();
        }

        private void OnPlayerRemove(ServerPlayerRemove serverRemove)
        {
            if (!_networkPlayers.TryGetValue(serverRemove.ClientId, out var networkPlayer))
                return;

            _networkPlayers.Remove(serverRemove.ClientId);
            if (networkPlayer == _localPlayer)
            {
                OnLocalPlayerNetworkRemove?.Invoke();
                return;
            }

            Destroy(networkPlayer.gameObject);
        }

        public void OnPlayerMovement(ServerPlayerMovement serverMovement)
        {
            if (!_networkPlayers.TryGetValue(serverMovement.ClientId, out var networkPlayer))
                return;

            if (networkPlayer == _localPlayer)
            {
                Debug.LogWarning("Trying to move local player.");
                return;
            }

            UpdatePlayerMovement(networkPlayer, serverMovement.Movement);
        }

        private void OnPlayerJump(ServerPlayerJump serverJump)
        {
            if (!_networkPlayers.TryGetValue(serverJump.ClientId, out var networkPlayer))
                return;

            if (networkPlayer == _localPlayer)
            {
                Debug.LogWarning("Trying to jump local player.");
                return;
            }

            networkPlayer.PlayerJump();
            UpdatePlayerMovement(networkPlayer, serverJump.Jump.Movement);
        }

        private void UpdatePlayerMovement(PlayerController networkPlayer, PlayerMovement move)
        {
            networkPlayer.InputVector = move.GlobalInput.AsVector2();
            if (move.UpdatePosition)
            {
                networkPlayer.PlayerMove(move.Position.AsVector2());
            }
        }
    }
}

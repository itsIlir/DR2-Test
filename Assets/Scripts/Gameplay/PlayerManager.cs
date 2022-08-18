using System;
using System.Collections;
using System.Collections.Generic;
using DarkRift;
using GameModels;
using GameModels.Unity;
using Networking;
using Services;
using UnityEngine;

namespace Gameplay
{
    public class PlayerManager : MonoBehaviour, IObjectManager
    {
        [SerializeField]
        private PlayerController _controllablePrefab, _networkPrefab;

        private readonly Dictionary<uint, PlayerController> _networkPlayers = new Dictionary<uint, PlayerController>();

        private uint _localPlayerId = uint.MaxValue;
        private PlayerController _localPlayer = null;
        private INetworkService _networkService;

        private void Awake()
        {
            _localPlayer = Instantiate(_controllablePrefab);

            _networkService = ServiceLocator<INetworkService>.Get();
            _networkService.GetProcessor<PlayerMovement>().OnMessage += OnPlayerMovement;
            _networkService.GetProcessor<PlayerInteract>().OnMessage += OnPlayerInteract;
        }

        private void Start()
        {
            StartCoroutine(PlayerMovementNetworkLoop());
        }

        private IEnumerator PlayerMovementNetworkLoop()
        {
            yield return new WaitUntil(() => _localPlayerId != uint.MaxValue);
            var playerMovement = new PlayerMovement
            {
                Id = _localPlayerId,
            };
            var sendDelay = new WaitForSecondsRealtime(0.1f);
            var nextPositionUpdate = 0f;
            const float positionUpdateTime = 1f;
            while (_networkService.Client.ConnectionState == ConnectionState.Connected)
            {
                var lastSentInputVector = new Vector2(playerMovement.GlobalInputX, playerMovement.GlobalInputY);
                var needPositionUpdate = Time.time > nextPositionUpdate;
                if (needPositionUpdate || Vector2.Distance(lastSentInputVector, _localPlayer.InputVector) > 0.01f)
                {
                    playerMovement.GlobalInputX = _localPlayer.InputVector.x;
                    playerMovement.GlobalInputY = _localPlayer.InputVector.y;
                    if (needPositionUpdate)
                    {
                        playerMovement.Location.SetPosition2D(_localPlayer.transform.position);
                        nextPositionUpdate = Time.time + positionUpdateTime;
                    }
                    else
                    {
                        playerMovement.Location.Flags = MovementFlags.None;
                    }

                    _networkService.SendMessage(playerMovement);
                }

                yield return sendDelay;
            }
        }

        public void InitializePlayer(uint id, bool localPlayer, LocationData location)
        {
            if (localPlayer)
                _localPlayerId = id;

            var player = localPlayer ? _localPlayer : Instantiate(_networkPrefab,
                new Vector3(location.PositionX, location.PositionY, location.PositionZ),
                Quaternion.Euler(location.Pitch, location.Yaw, location.Yaw)
            );
            _networkPlayers.Add(id, player);
        }

        public void UpdateLocation(uint id, LocationData data)
        {
            if (!_networkPlayers.TryGetValue(id, out var networkPlayer))
                return;

            if (networkPlayer == _localPlayer)
            {
                Debug.LogWarning("Trying to update local player location.");
                return;
            }

            networkPlayer.PlayerMove(new Vector2(data.PositionX, data.PositionY));
        }

        public void Remove(uint id)
        {
            if (!_networkPlayers.TryGetValue(id, out var networkPlayer))
                return;

            _networkPlayers.Remove(id);
            if (networkPlayer == _localPlayer)
            {
                return;
            }

            Destroy(networkPlayer.gameObject);
        }

        public void OnPlayerMovement(PlayerMovement movement)
        {
            if (!_networkPlayers.TryGetValue(movement.Id, out var networkPlayer))
                return;

            if (networkPlayer == _localPlayer)
            {
                Debug.LogWarning("Trying to move local player.");
                return;
            }

            networkPlayer.InputVector = new Vector2(movement.GlobalInputX, movement.GlobalInputY);
            if ((movement.Location.Flags & MovementFlags.Position2D) != 0)
            {
                networkPlayer.PlayerMove(new Vector2(
                    movement.Location.PositionX,
                    movement.Location.PositionY
                ));
            }
        }

        public void OnPlayerInteract(PlayerInteract interaction)
        {
            if (!_networkPlayers.TryGetValue(interaction.Id, out var networkPlayer))
                return;

            if (networkPlayer == _localPlayer)
            {
                Debug.LogWarning("Trying to interact local player.");
                return;
            }
        }
    }

    public interface IObjectManager
    {
        void UpdateLocation(uint id, LocationData data);
        void Remove(uint id);
    }
}

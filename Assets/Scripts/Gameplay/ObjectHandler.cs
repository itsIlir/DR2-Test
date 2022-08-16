using System.Collections.Generic;
using GameModels;
using UnityEngine;

namespace Gameplay
{
    public class ObjectHandler : MonoBehaviour
    {
        [SerializeField]
        CubeMovement _controllablePrefab, _networkPrefab;

        private readonly Dictionary<ushort, CubeMovement> _players = new Dictionary<ushort, CubeMovement>();
        public ushort LocalClientId { get; set; }

        public bool ObjectExists(ushort id) => _players.ContainsKey(id);

        public ObjectLocation GetObjectLocation(ushort id)
            => new ObjectLocation
            {
                Id = id,
                Movement = new MovementData
                {
                    Flags = MovementFlags.Position2D,
                    PositionX = _players[id].transform.position.x,
                    PositionY = _players[id].transform.position.y,
                }
            };

        public void OnObjectInit(ObjectInit init)
        {
            Debug.Log($"Init Object ID {init.Id}.");
            var prefab = init.Id == LocalClientId ? _controllablePrefab : _networkPrefab;
            var cubeMovement = Instantiate(
                prefab,
                new Vector3(init.Movement.PositionX, init.Movement.PositionY),
                Quaternion.identity
            );

            _players.Add(init.Id, cubeMovement);
        }

        public void OnObjectLocation(ObjectLocation location)
        {
            if (location.Id == LocalClientId)
            {
                Debug.LogWarning("Message received for local client movement!");
                return;
            }

            _players[location.Id].PlayerMove(new Vector2(location.Movement.PositionX, location.Movement.PositionY));
        }

        public void OnObjectRemove(ObjectRemove remove)
        {
            Debug.Log($"Remove Object ID {remove.Id}.");
            Destroy(_players[remove.Id].gameObject);
            _players.Remove(remove.Id);
        }
    }
}

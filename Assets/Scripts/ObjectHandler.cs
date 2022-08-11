using System.Collections.Generic;
using GameModels;
using UnityEngine;

namespace DR2Test.Network
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
                X = _players[id].transform.position.x,
                Y = _players[id].transform.position.y,
            };

        public void OnObjectInit(ObjectInit init)
        {
            Debug.Log($"Init Object ID {init.Id} at {init.X}, {init.Y}.");
            var prefab = init.Id == LocalClientId ? _controllablePrefab : _networkPrefab;
            var cubeMovement = Instantiate(
                prefab,
                new Vector3(init.X, init.Y),
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

            _players[location.Id].PlayerMove(new Vector2(location.X, location.Y));
        }

        public void OnObjectRemove(ObjectRemove remove)
        {
            Debug.Log($"Remove Object ID {remove.Id}.");
            Destroy(_players[remove.Id].gameObject);
            _players.Remove(remove.Id);
        }
    }
}


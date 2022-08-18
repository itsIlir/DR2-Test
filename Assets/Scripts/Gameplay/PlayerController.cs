using System;
using UnityEngine;

namespace Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        public const float Speed = 10;

        [SerializeField]
        bool isOwner;

        private static readonly Bounds PlayBounds = new Bounds(Vector3.zero, new Vector3(10, 10));

        public Vector2 InputVector { get; set; } = Vector2.zero;
        public Vector2 Position { get; private set; }

        private void Start()
        {
            Position = transform.position;
        }

        private void FixedUpdate()
        {
            if (isOwner)
                InputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            var newPos = Position + InputVector * Speed * Time.deltaTime;
            PlayerMove(newPos);
        }

        public void PlayerMove(Vector2 newPos)
        {
            newPos = PlayBounds.ClosestPoint(newPos);
            Position = newPos;

            if (isOwner)
                transform.position = Position;
        }
    }
}

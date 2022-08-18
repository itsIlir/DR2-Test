using UnityEngine;

namespace Gameplay
{
    public class PlayerNetworkVisual : MonoBehaviour
    {
        [SerializeField] private PlayerController _controller;

        private Vector2 VisualVelocity
        {
            get
            {
                var controlMove = Time.deltaTime * PlayerController.Speed * _controller.InputVector;
                var controlDiff = _controller.Position - (Vector2) transform.position;
                if (controlDiff.sqrMagnitude > 2 * 2)
                {
                    return controlMove + controlDiff;
                }
                if (controlDiff.sqrMagnitude > 0.05 * 0.05)
                {
                    return controlMove + controlDiff * GetFrameIndependentLerpValue(0.025f, Time.deltaTime);
                }

                return controlMove;
            }
        }

        private void LateUpdate()
        {
            transform.position += (Vector3)VisualVelocity;
        }

        private static float GetFrameIndependentLerpValue(float smoothing, float dt)
            => 1f - Mathf.Pow(smoothing, dt);
    }
}

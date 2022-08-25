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
                var diffSqrMagnitude = controlDiff.sqrMagnitude;
                if (diffSqrMagnitude > 2 * 2 || diffSqrMagnitude < 0.05f * 0.05f)
                {
                    return controlMove + controlDiff;
                }
                else
                {
                    return controlMove + controlDiff * GetFrameIndependentLerpValue(0.025f, Time.deltaTime);
                }
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

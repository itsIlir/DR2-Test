using JetBrains.Annotations;
using UnityEngine;

namespace GameModels.Unity
{
    public static class LocationDataExtensions
    {
        public static void ApplyGlobalTransform(this LocationData location, Transform transform)
        {
            location.ApplyGlobalPosition(transform);
            location.ApplyGlobalRotation(transform);
        }

        public static void ApplyLocalTransform(this LocationData location, Transform transform)
        {
            location.ApplyLocalPosition(transform);
            location.ApplyLocalRotation(transform);
        }

        public static void ApplyGlobalPosition(this LocationData location, Transform transform)
            => transform.position = TransformPosition(location, transform.position);

        public static void ApplyLocalPosition(this LocationData location, Transform transform)
            => transform.localPosition = TransformPosition(location, transform.localPosition);

        public static void ApplyGlobalRotation(this LocationData location, Transform transform)
            => transform.rotation = TransformRotation(location, transform.rotation);

        public static void ApplyLocalRotation(this LocationData location, Transform transform)
            => transform.localRotation = TransformRotation(location, transform.localRotation);

        public static void SetPosition2D(this ref LocationData location, Vector2 position)
        {
            location.Flags |= MovementFlags.Position2D;
            location.Flags &= ~MovementFlags.Position3D;
            location.PositionX = position.x;
            location.PositionY = position.y;
        }

        private static Quaternion TransformRotation(LocationData location, Quaternion rotation)
        {
            if ((location.Flags & MovementFlags.Rotation2D) != 0)
            {
                var euler = rotation.eulerAngles;
                euler.y = location.Angle;
                return Quaternion.Euler(euler);
            }

            if ((location.Flags & MovementFlags.Rotation3D) != 0)
            {
                return Quaternion.Euler(
                    location.Pitch,
                    location.Yaw,
                    location.Roll);
            }

            return rotation;
        }

        private static Vector3 TransformPosition(LocationData location, Vector3 position)
        {
            if ((location.Flags & MovementFlags.Position2D) != 0)
            {
                return new Vector3(
                    location.PositionX,
                    location.PositionY,
                    position.z
                );
            }

            if ((location.Flags & MovementFlags.Position3D) != 0)
            {
                return new Vector3(
                    location.PositionX,
                    location.PositionY,
                    location.PositionZ
                );
            }

            return position;
        }
    }
}

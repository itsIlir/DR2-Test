using GameModels.Geometry;
using UnityEngine;

namespace GameModels.Unity
{
    public static class FloatVectorExtensions
    {
        public static Vector2 AsVector2(this FloatVector2 vector)
            => new Vector2(vector.X, vector.Y);

        public static FloatVector2 AsFloatVector2(this Vector2 vector)
            => new FloatVector2(vector.x, vector.y);
    }
}

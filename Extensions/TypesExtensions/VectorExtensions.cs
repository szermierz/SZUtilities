
namespace SZUtilities.VectorExtensions
{
    public static class VectorExtensions
    {
        public const float c_almostZero = 0.0001f;

        public static bool IsZero(this UnityEngine.Vector3 value) => value.sqrMagnitude < c_almostZero;
        public static bool IsZero(this UnityEngine.Vector2 value) => value.sqrMagnitude < c_almostZero;

        public static UnityEngine.Vector3 MultiplyComponents(this UnityEngine.Vector3 lhs, UnityEngine.Vector3 rhs)
        {
            return new UnityEngine.Vector3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
        }

        public static UnityEngine.Vector2 MultiplyComponents(this UnityEngine.Vector2 lhs, UnityEngine.Vector2 rhs)
        {
            return new UnityEngine.Vector2(lhs.x * rhs.x, lhs.y * rhs.y);
        }

        public static int CalculateHash(this UnityEngine.Vector3 value)
        {
            const float c_floatScale = 1000.0f;
            const int c_xMask = 0b1001001001001001;
            const int c_yMask = 0b0100100100100100;
            const int c_zMask = 0b0010010010010010;

            var x = SimpleHash((int)(c_floatScale * value.x)) & c_xMask;
            var y = SimpleHash((int)(c_floatScale * value.y)) & c_yMask;
            var z = SimpleHash((int)(c_floatScale * value.z)) & c_zMask;

            return x | y | z;

            int SimpleHash(int val)
            {
                val = 78321564 * val + 345334;
                return val % 0xFFFF;
            }
        }
    }
}
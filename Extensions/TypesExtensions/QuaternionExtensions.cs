using SZUtilities.VectorExtensions;

namespace SZUtilities.QuaternionExtensions
{
    public static class QuaternionExtensions
    {
        public static float FromToAngle(UnityEngine.Vector3 from, UnityEngine.Vector3 to, UnityEngine.Vector3 axis, bool absolute = true)
        {
            var rotation = UnityEngine.Quaternion.FromToRotation(from, to);
            var angles = rotation.eulerAngles.MultiplyComponents(axis);
            var angle = angles.x + angles.y + angles.z;

            while (angle > 180.0f)
                angle -= 360.0f;
            while (angle < -180.0f)
                angle += 360.0f;

            if (absolute)
                angle = UnityEngine.Mathf.Abs(angle);

            return angle;
        }
    }
}
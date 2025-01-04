using System.Collections.Generic;
using UnityEngine;

namespace SZUtilities.Math
{
    /* ref: https://github.com/shamim-akhtar/bezier-curve */

    public static class Bezier
    {
        private static float[] s_factorial = new float[]
        {
            1.0f,
            1.0f,
            2.0f,
            6.0f,
            24.0f,
            120.0f,
            720.0f,
            5040.0f,
            40320.0f,
            362880.0f,
            3628800.0f,
            39916800.0f,
            479001600.0f,
            6227020800.0f,
            87178291200.0f,
            1307674368000.0f,
            20922789888000.0f,
        };

        private static float Binomial(int n, int i)
        {
            float ni;
            float a1 = s_factorial[n];
            float a2 = s_factorial[i];
            float a3 = s_factorial[n - i];
            ni = a1 / (a2 * a3);
            return ni;
        }

        private static float Bernstein(int n, int i, float t)
        {
            float t_i = Mathf.Pow(t, i);
            float t_n_minus_i = Mathf.Pow((1 - t), (n - i));

            float basis = Binomial(n, i) * t_i * t_n_minus_i;
            return basis;
        }

#if UNITY_EDITOR
        public static void GizmoDrawBezier(IReadOnlyList<Vector3> controlPoints, float thickness = 2.0f, Vector3 centerOffset = default, int interpolationSteps = 20)
        {
            var previousPos = centerOffset + controlPoints[0];
            for (var step = 1; step <= interpolationSteps; ++step)
            {
                var progress = (float)step / interpolationSteps;

                var pos = centerOffset + Interpolate(controlPoints, progress);
                UnityEditor.Handles.DrawLine(previousPos, pos, thickness);

                previousPos = pos;
            }
        }
#endif

        public static Vector3 Interpolate(IReadOnlyList<Vector3> controlPoints, float t)
        {
            int n = controlPoints.Count - 1;
            if (n > s_factorial.Length - 1)
                throw new System.NotSupportedException();
            
            if (t <= 0) 
                return controlPoints[0];
            
            if (t >= 1) 
                return controlPoints[controlPoints.Count - 1];

            var result = Vector3.zero;

            for (int i = 0; i < controlPoints.Count; ++i)
            {
                var bn = Bernstein(n, i, t) * controlPoints[i];
                result += bn;
            }

            return result;
        }
    }
}
using System;
using UnityEngine;

namespace Agoston_R.Aim_Assist_Pro.Scripts.AimAssistCode.Helper.Numerics
{
    /// <summary>
    /// Contains additional logic for the aim assist.
    /// </summary>
    public static class NumberUtils
    {
        private const float Epsilon = 0.01f;

        /// <summary>
        /// Determines whether the two vectors are approximately equal. Uses a large enough epsilon to give consistent results and keep error margin to a minimum.
        /// </summary>
        /// <param name="first">first vector</param>
        /// <param name="second">second vector</param>
        /// <param name="epsilon">error range for the comparison. optional, add a set one or use the default</param>
        /// <returns>true if the two vectors' x and y are equal within the error margin, false otherwise</returns>
        public static bool EqualsApprox(this Vector2 first, Vector2 second, float epsilon = Epsilon)
        {
            return Math.Abs(first.x - second.x) < epsilon && Math.Abs(first.y - second.y) < epsilon;
        }

        /// <summary>
        /// Determines whether the two float values are approximately equal. Uses a large enough error margin to give consistent results and keep error margin to a minimum.
        /// </summary>
        /// <param name="f1">first value</param>
        /// <param name="f2">second value</param>
        /// <param name="epsilon">error range for the comparison. optional, add a set one or use the default</param>
        /// <returns>true if the values are equal within the error margin, false otherwise</returns>
        public static bool EqualsApprox(this float f1, float f2, float epsilon = Epsilon)
        {
            return Math.Abs(f1 - f2) < epsilon;
        }

        /// <summary>
        /// Ensures no NaN is returned.
        ///
        /// Useful when working with very narrow angles in division.
        /// </summary>
        /// <param name="f">value to sanitize</param>
        /// <returns>zero if the passed value is NaN, the value itself otherwise</returns>
        public static float Sanitized(this float f)
        {
            return float.IsNaN(f) ? 0f : f;
        }

        /// <summary>
        /// Determines whether the first value is between the other two.
        ///
        /// Have to pass the floor first, the ceiling second.
        /// </summary>
        /// <param name="f">the value to see if it's between the other two</param>
        /// <param name="floor">bottom value</param>
        /// <param name="ceiling">ceiling value</param>
        /// <returns></returns>
        public static bool Between(this float f, float floor, float ceiling)
        {
            return f >= floor && f <= ceiling;
        }
    }
}

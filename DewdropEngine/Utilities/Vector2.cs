using SFML.System;

namespace DewDrop.Utilities
{
    /// <summary>
    /// Represents a point in 2D space
    /// </summary>
    public struct Vector2
    {
        /// <summary>
        /// The X coordinate of the point
        /// </summary>
        public float x;

        /// <summary>
        /// The Y coordinate of the point
        /// </summary>
        public float y;

        /// <summary>
        /// Creates a new Vector2 with given X and Y
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        public Vector2(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"Vector2: ({x},{y})";
        }

        /// <summary>
        /// Casts the X and Y values to integer values
        /// </summary>
        public void ForceInteger() {
            x = (int)x;
            x = (int)y;
        }

        #region Translation methods

        /// <summary>
        /// Creates a new Vector2f from the Vector2.
        /// </summary>
        /// <returns>A Vector2f with the Vector2's X and Y</returns>
        public Vector2f TranslateToV2F()
        {
            return new Vector2f(x, y);
        }

        /// <summary>
        /// Creates a new Vector2i from the Vector2.
        /// </summary>
        /// <returns>A Vector2i with the Vector2's X and Y</returns>
        public Vector2i TranslateToV2I()
        {
            return new Vector2i((int)x, (int)y);
        }

        /// <summary>
        /// Creates a new Vector2U from the Vector2.
        /// </summary>
        /// <returns>A Vector2u with the Vector2's X and Y</returns>
        public Vector2u TranslateToV2U()
        {
            return new Vector2u((uint)x, (uint)y);
        }

        #endregion

        #region Operators

        // addition
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            a.x += b.x;
            a.y += b.y;
            return a;
        }

        // subtraction
        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            a.x -= b.x;
            a.y -= b.y;
            return a;
        }

        // multiplication 
        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            a.x *= b.x;
            a.y *= b.y;
            return a;
        }

        // divison with a vector
        public static Vector2 operator /(Vector2 a, Vector2 b)
        {
            a.x /= b.x;
            a.y /= b.y;
            return a;
        }

        // divison with an int
        public static Vector2 operator /(Vector2 a, int divider)
        {
            a.x /= divider;
            a.y /= divider;
            return a;
        }
        #endregion
    }
}

﻿using SFML.System;

namespace DewDrop.Utilities
{
    /// <summary>
    /// Represents a point in 2D space
    /// </summary>
    public struct Vector2
    {
        public static Vector2 Zero = new Vector2(0, 0);

        public static Vector2f Zero_F = new Vector2f(0, 0);

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

        public Vector2(Vector2f a) {
            x = a.X;
            y = a.Y;
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

        /// <summary>
        /// Casts a vector's X and Y to int.
        /// </summary>
        /// <param name="v">The vector to truncate</param>
        /// <returns></returns>
        public static Vector2 Truncate(Vector2 v)
        {
            int x = (int)v.x;
            int y = (int)v.y;
            return new Vector2(x, y);
        }

        /// <summary>
        /// Casts a vector's X and Y to int.
        /// </summary>
        /// <param name="v">The vector to truncate</param>
        /// <returns></returns>
        public static Vector2f Truncate(Vector2f v)
        {
            int x = (int)v.X;
            int y = (int)v.Y;
            return new Vector2f(x, y);
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

        public static float Magnitude(Vector2 v)
        {
            return (float)Math.Sqrt(v.x * v.x + v.y * v.y);
        }

        // vector 2f
        public static float Magnitude(Vector2f v)
        {
            return (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
        }

        public static Vector2 LeftNormal(Vector2 v)
        {
            return new Vector2(v.y, -v.x);
        }

        public static Vector2 RightNormal(Vector2 v)
        {
            return new Vector2(-v.y, v.x);
        }

        public static float DotProduct(Vector2 a, Vector2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        /// <summary>
        /// Normalizes a vector.
        /// </summary>
        /// <param name="v">The vector to normalize</param>
        /// <returns></returns>
        public static Vector2 Normalize(Vector2 v)
        {
            float vectorMagnitude = Magnitude(v);
            Vector2 zero_VECTOR;
            if (vectorMagnitude > 0f)
            {
                float x = v.x / vectorMagnitude;
                float y = v.y / vectorMagnitude;
                zero_VECTOR = new Vector2(x, y);
            }
            else
            {
                zero_VECTOR = Zero;
            }
            return zero_VECTOR;
        }


        /// <summary>
        /// Normalizes a vector.
        /// </summary>
        /// <param name="v">The vector to normalize</param>
        /// <returns></returns>
        public static Vector2f Normalize(Vector2f v)
        {
            float vectorMagnitude = Magnitude(v);
            Vector2f zero_VECTOR;
            if (vectorMagnitude > 0f)
            {
                float x = v.X/ vectorMagnitude;
                float y = v.Y / vectorMagnitude;
                zero_VECTOR = new Vector2f(x, y);
            }
            else
            {
                zero_VECTOR = Zero_F;
            }
            return zero_VECTOR;
        }


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

        // subtraction with a vector2f
        public static Vector2 operator -(Vector2 a, Vector2f b)
        {
            a.x -= b.Y;
            a.y -= b.Y;
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

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector2 a, Vector2 b) { 
            return !a.Equals(b);
        }

        public static Vector2 operator *(Vector2 v, float x) => new Vector2(v.x * x, v.y * x);

        public bool Equals(Vector2 b) 
        {
            return (x == b.x) && (y == b.y);

        }
        #endregion
    }
}
using System;
using Microsoft.Xna.Framework;

namespace MyGame.Utils
{
    static class MathUtils
    {
        private static Random random;
        public static float ClosestInRange(float value, float upper, float lower)
        {
            if (value < lower)
            {
                return lower;
            }
            if (value > upper)
            {
                return upper;
            }
            return value;
        }

        public static Vector2 RandomVector(Vector2 v)
        {
            if(random == null)
            {
                random = new Random();
            }
            return RandomVector(v, random);
        }

        public static Vector2 RandomVector(Vector2 v, Random random)
        {
            return new Vector2((float)(random.NextDouble() * v.X), (float)(random.NextDouble() * v.Y));
        }

        public static float Distance(Point p1, Point p2)
        {
            return (float)(Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2)));
        }

        public static Rectangle RectangleUnion(Rectangle r1, Rectangle r2)
        {
            int left = Math.Min(Math.Min(r1.X, r2.X), Math.Min(r1.Right, r2.Right));
            int right = Math.Max(Math.Max(r1.X, r2.X), Math.Max(r1.Right, r2.Right));

            int top = Math.Min(Math.Min(r1.Y, r2.Y), Math.Min(r1.Bottom, r2.Bottom));
            int bottom = Math.Max(Math.Max(r1.Y, r2.Y), Math.Max(r1.Bottom, r2.Bottom));

            return new Rectangle(left, top, right - left, bottom - top);
        }
    }
}

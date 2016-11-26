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
    }
}

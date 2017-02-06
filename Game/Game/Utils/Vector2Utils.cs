using System;
using Microsoft.Xna.Framework;


namespace MyGame.Utils
{
    public class Vector2Utils
    {
        public static Vector2 ConstructVectorFromPolar(double magnitude, double angle)
        {
            return new Vector2((float)(magnitude * Math.Cos(angle)), (float)(magnitude * Math.Sin(angle)));
        }

        public static float Vector2Angle(Vector2 point)
        {
            if (point.X == 0)
                return RestrictAngle(Math.PI / 2 * (point.Y / Math.Abs(point.Y)));
            else if (point.X > 0)
                return RestrictAngle(Math.Atan(point.Y / point.X));
            else
                return RestrictAngle(Math.Atan(point.Y / point.X) + Math.PI);
        }

        public static Vector2 RotateVector2(Vector2 vector, double rotation)
        {
            return Vector2.Transform(vector, Matrix.CreateRotationZ((float)rotation));
        }

        public static float RestrictAngle(double angle)
        {
            while (angle > Math.PI * 2)
            {
                angle = angle - (float)Math.PI * 2;
            }

            while (angle < 0)
            {
                angle = angle + (float)Math.PI * 2;
            }

            return (float)angle;
        }

        public static float RestrictAngle(float angle)
        {
            while (angle > Math.PI * 2)
            {
                angle = angle - (float)Math.PI * 2;
            }

            while (angle < 0)
            {
                angle = angle + (float)Math.PI * 2;
            }

            return angle;
        }

        public static float ShortestAngleDistance(float a, float b)
        {
            a = RestrictAngle(a);
            b = RestrictAngle(b);

            return (float)Math.Min(Math.Abs(a - b), Math.Abs(Math.Abs(a - b) - 2 * Math.PI));
        }

        public static float AngleDistance(float a, float b)
        {
            return Math.Abs(RestrictAngle(b - a));
        }

        public static float MinimizeMagnitude(float angle)
        {
            float rAngle = RestrictAngle(angle);
            float rAngleSmall = (float)(rAngle-Math.PI*2);
            if(Math.Abs(rAngleSmall) < rAngle)
            {
                return rAngleSmall;
            }
            return rAngle;
        }

        public static float Lerp(float draw, float simulation, float smoothing)
        {
            if (smoothing >= 1)
            {
                return simulation;
            }
            if (smoothing <= 0)
            {
                return draw;
            }
            draw = Utils.Vector2Utils.RestrictAngle(draw);
            simulation = Utils.Vector2Utils.RestrictAngle(simulation);
            float difference = Utils.Vector2Utils.MinimizeMagnitude(simulation - draw);
            return draw + smoothing * difference; //MathHelper.Lerp(0, difference, smoothing);
        }

        public static float Det(Vector2 v1, Vector2 v2)
        {
            return Det(v1, v2, new Vector2(0));
        }

        public static float Det(Vector2 v1, Vector2 v2, Vector2 center)
        {
            return (v1.X - center.X) * (v2.Y - center.Y) - (v2.X - center.X) * (v1.Y - center.Y);
        }

        public static float Distance(Rectangle rect, Vector2 point)
        {
            Vector2 rectCenter = new Vector2(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);

            float dx = Math.Max(Math.Abs(rectCenter.X - point.X) - rect.Width / 2f, 0);
            float dy = Math.Max(Math.Abs(rectCenter.Y - point.Y) - rect.Height / 2f, 0);
            return (float)(Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
        }

        public static float Distance(Rectangle rect1, Rectangle rect2)
        {
            Vector2 rectCenter1 = new Vector2(rect1.X + rect1.Width / 2f, rect1.Y + rect1.Height / 2f);
            Vector2 rectCenter2 = new Vector2(rect2.X + rect2.Width / 2f, rect2.Y + rect2.Height / 2f);

            float dx = Math.Max(Math.Abs(rectCenter1.X - rectCenter2.X) - rect1.Width / 2f - rect2.Width / 2f, 0);
            float dy = Math.Max(Math.Abs(rectCenter1.Y - rectCenter2.Y) - rect1.Height / 2f - rect2.Height / 2f, 0);
            return (float)(Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
        }

        public static bool Intersects(Rectangle rect1, Rectangle rect2)
        {
            Vector2 rectCenter1 = new Vector2(rect1.X + rect1.Width / 2f, rect1.Y + rect1.Height / 2f);
            Vector2 rectCenter2 = new Vector2(rect2.X + rect2.Width / 2f, rect2.Y + rect2.Height / 2f);

            float dx = Math.Max(Math.Abs(rectCenter1.X - rectCenter2.X) - rect1.Width / 2f - rect2.Width / 2f, 0);
            float dy = Math.Max(Math.Abs(rectCenter1.Y - rectCenter2.Y) - rect1.Height / 2f - rect2.Height / 2f, 0);
            return dx == 0 && dy == 0;
        }

        public static float DistanceInside(Rectangle rect, Vector2 point)
        {
            Vector2 rectCenter = new Vector2(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);

            float dx = Math.Max(rect.Width / 2f - Math.Abs(rectCenter.X - point.X), 0);
            float dy = Math.Max(rect.Height / 2f - Math.Abs(rectCenter.Y - point.Y), 0);
            return Math.Min(dx, dy);
        }
    } 
}

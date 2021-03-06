﻿using System;
using Microsoft.Xna.Framework;

namespace MyGame.Utils
{
    static class PhysicsUtils
    {
        public static Vector2 MoveTowardBounded(Vector2 position, Vector2 target, float maxDistance)
        {
            if(maxDistance <= 0)
            {
                return position;
            }
            if(Vector2.Distance(position, target) <=  maxDistance)
            {
                return target;
            }

            float totalDistance = Vector2.Distance(position, target);
            return Vector2.Lerp(position, target, maxDistance / totalDistance);
        }

        public static float MoveTowardBounded(float position, float target, float maxDistance)
        {
            if (maxDistance <= 0 || float.IsNaN(target))
            {
                return position;
            }
            if (Math.Abs(position - target) <= maxDistance)
            {
                return target;
            }
            float travel = Math.Sign(target - position);
            travel = travel * maxDistance;
            return position + travel;
        }

        public static float AngularMoveTowardBounded(float angle, float target, float maxDistance)
        {
            angle = Utils.Vector2Utils.RestrictAngle(angle);
            target = Utils.Vector2Utils.RestrictAngle(target);
            float difference = Utils.Vector2Utils.MinimizeMagnitude(target - angle);
            if (Math.Abs(difference) <= maxDistance)
            {
                return target;
            }
            else
            {
                return angle + MoveTowardBounded(0, difference, maxDistance);
            }
        }

        public static Boolean IsPointedAt(Vector2 position, float direction, Vector2 target, float errorDistance)
        {
            float angle = Utils.Vector2Utils.ShortestAngleDistance(Utils.Vector2Utils.Vector2Angle(target - position), direction);
            float distanceToTarget = Vector2.Distance(target, position);
            return Math.Abs(angle) <= Math.PI / 2 && Math.Abs((float)(Math.Sin(angle) * distanceToTarget)) < errorDistance;
        }
    }
}

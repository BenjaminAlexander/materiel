﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.Geometry;
using MyGame.GameStateObjects.QuadTreeUtils;


namespace MyGame.GameStateObjects
{
    public abstract class FlyingGameObject : CompositePhysicalObject
    {


        private Boolean speedUp = false;
        private Boolean slowDown = false;

        public Boolean SlowDown
        {
            get { return slowDown; }
            set { slowDown = value; }

        }

        public Boolean SpeedUp
        {
            get { return speedUp; }
            set { speedUp = value; }

        }

        private Vector2 velocity = new Vector2(0);
        private float maxSpeed = 200.0f;
        private float maxAngularSpeed = 1.0f;
        private Vector2 targetVelocity = new Vector2(0);

        private float maxAcceleration = 100;
        private Collidable collidable;
        protected FlyingGameObject(Collidable drawObject, Vector2 position, float direction, Vector2 velocity, float maxSpeed, /*float acceleration,*/ float maxAcceleration, float maxAngularSpeed)
            : base(position, direction)
        {
            this.velocity = velocity;
            this.maxSpeed = maxSpeed;
            this.maxAngularSpeed = maxAngularSpeed;
            this.maxAcceleration = maxAcceleration;
            this.collidable = drawObject;
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set
            {
                if (maxSpeed == 0)
                {
                    velocity = new Vector2(0);
                }
                if (value.Length() > maxSpeed)
                {
                    value.Normalize();
                    value = value * maxSpeed;
                }
                velocity = value;
            }
        }

        public Vector2 TargetVelocity
        {
            get { return targetVelocity; }
            set
            { targetVelocity = value;}
        }

        public void SetMaxTargetVelocity(float direction)
        {
            targetVelocity = Vector2Utils.ConstructVectorFromPolar(maxSpeed, direction);
        }

        public float MaxSpeed
        {
            get { return maxSpeed; }
            private set
            {
                maxSpeed = value;
                // We need to reset the speed to comply with the new max.
                Velocity = Velocity;
            }
        }

        public float MaxAngularSpeed
        {
            get { return maxAngularSpeed; }
        }

        public float MaxAcceleration
        {
            get { return maxAcceleration; }
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            if (speedUp && !slowDown)
            {
                this.SetMaxTargetVelocity(this.Direction);
            }
            if (!speedUp && slowDown)
            {
                this.SetMaxTargetVelocity((float)(this.Direction + Math.PI));
            }

            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            //TODO: accel toward target velocity
            Velocity = Physics.PhysicsUtils.MoveTowardBounded(Velocity, targetVelocity, maxAcceleration * secondsElapsed);
            Position = Position + Velocity * secondsElapsed;
            base.UpdateSubclass(gameTime);
            slowDown = false;
            speedUp = false;
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            base.Draw(gameTime, graphics);
            collidable.Position = this.Position;
            collidable.Rotation = this.Direction;
            collidable.Draw(graphics);
        }

        public Boolean CollidesWith(FlyingGameObject other)
        {
            collidable.Position = this.Position;
            collidable.Rotation = this.Direction;

            other.collidable.Position = other.Position;
            other.collidable.Rotation = other.Direction;
            return this.collidable.CollidesWith(other.collidable);
        }
        public Boolean CollidesWith(Collidable other)
        {
            collidable.Position = this.Position;
            collidable.Rotation = this.Direction;

            return this.collidable.CollidesWith(other);
        
        }
/*
        public Boolean CollidesWith(Vector2 point1, Vector2 point2)
        {
            drawObject.Position = this.Position;
            drawObject.Rotation = this.Direction;

            return this.drawObject.CollidesWith(point1, point2);
        }*/

        public virtual void TurnRight(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            float changeInAngle = (float)(secondsElapsed * maxAngularSpeed);
            Direction = Direction + changeInAngle;
        }

        public virtual void TurnLeft(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            float changeInAngle = (float)(secondsElapsed * maxAngularSpeed);
            Direction = Direction - changeInAngle;
        }

        private void TurnTowardsDirection(GameTime gameTime, float directionSetpoint)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            float changeInAngle = (float)(secondsElapsed * maxAngularSpeed);
            float angleDifference = Vector2Utils.RestrictAngle(directionSetpoint - Direction);

            Direction = Physics.PhysicsUtils.AngularMoveTowardBounded(Direction, directionSetpoint, changeInAngle);
        }

        public virtual void TurnTowards(GameTime gameTime, Vector2 target, float errorDistance)
        {
            float directionSetpoint = Vector2Utils.Vector2Angle(target - Position);
            this.TurnTowardsDirection(gameTime, directionSetpoint);
        }

        public virtual void TurnAway(GameTime gameTime, Vector2 target)
        {
            float directionSetpoint = Vector2Utils.RestrictAngle(Vector2Utils.Vector2Angle(target - Position) + Math.PI);
            this.TurnTowardsDirection(gameTime, directionSetpoint);
        }


        public Boolean Contains(Vector2 point)
        {
            return collidable.Contains(point);
        }

        public Circle BoundingCircle()
        {
            return collidable.BoundingCircle();
        }

        public Boolean IsPointedAt(Vector2 target, float errorDistance)
        {
            return Physics.PhysicsUtils.IsPointedAt(this.Position, this.Direction, target, errorDistance);
        }
    }
}

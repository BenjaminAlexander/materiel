﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;
using MyGame.Server;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.materiel
{
    public abstract class Vehicle : PhysicalObject, IPlayerControlled
    {
        public const float distancePerMateriel = 600;
        public const float secondsPerMateriel = 12;
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Enemy"), Color.White, TextureLoader.GetTexture("Enemy").CenterOfMass, .9f);
        public override Collidable Collidable
        {
            get { return collidable; }
        }

        private FloatGameObjectMember materiel;
        private FloatGameObjectMember maxMateriel;
        private GameObjectReferenceField<Company> company;
        private GameObjectReferenceField<PlayerGameObject> controllingPlayer;
        public const float maxSpeed = 100;

        public Vehicle(GameObjectCollection collection)
            : base(collection)
        {
            controllingPlayer = new GameObjectReferenceField<PlayerGameObject>(this);
            company = new GameObjectReferenceField<Company>(this);
            materiel = new FloatGameObjectMember(this, 10);
            maxMateriel = new FloatGameObjectMember(this, 10);            
        }

        public static void ServerInitialize(Vehicle vic, PlayerGameObject controllingPlayer, Vector2 position, float maxMateriel)
        {
            vic.controllingPlayer.Value = controllingPlayer;
            vic.materiel.Value = maxMateriel;
            vic.maxMateriel.Value = maxMateriel;
            PhysicalObject.ServerInitialize(vic, position, 0);
        }

        public GameObjectReference<Company> Company
        {
            set
            {
                this.company.Value = value;
            }

            get
            {
                return this.company.Value;
            }
        }

        public void Idle(float seconds)
        {
            float cost = seconds / secondsPerMateriel;
            this.materiel.Value = Math.Max(this.materiel - cost, 0);
        }

        public float MoveToward(Vector2 targetPos, float seconds)
        {
            float maxMoveDistance = Math.Min(maxSpeed * seconds, this.Range());
            float distanceToTarget = Vector2.Distance(this.Position, targetPos);
            this.Position = Utils.PhysicsUtils.MoveTowardBounded(this.Position, targetPos, maxMoveDistance);
            maxMoveDistance = Math.Min(distanceToTarget, maxMoveDistance);
            this.materiel.Value = Math.Max(this.materiel.Value - this.MoveCost(maxMoveDistance), 0f);
            float secondsLeft = seconds - (maxMoveDistance / maxSpeed);
            return seconds;
        }

        public override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
        {
        }

        public GameObjectReference<PlayerGameObject> ControllingPlayer
        {
            get 
            { 
                return controllingPlayer.Value; 
            }
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            base.Draw(gameTime, graphics);
            graphics.DrawDebugFont(this.materiel.Value.ToString(), this.Position + new Vector2(25, 0), 1f);
        }

        public virtual void DrawScreen(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics, Camera camera, Color color, float depth)
        {
            graphics.DrawCircle(camera.WorldToScreenPosition(this.Position), 15, color, depth);
        }

        public float Range()
        {
            return distancePerMateriel * materiel.Value;
        }

        public float MoveCost(float distance)
        {
            return distance / distancePerMateriel;
        }

        public float Materiel
        {
            get
            {
                return materiel.Value;
            }

            set
            {
                materiel.Value = value;
            }
        }

        public float MaxMateriel
        {
            get
            {
                return maxMateriel.Value;
            }
        }

        public float MaxSpeed
        {
            get
            {
                return maxSpeed;
            }
        }

        public float IdleTime(float materiel)
        {
            return materiel * secondsPerMateriel;
        }

        public float ResupplyAmount
        {
            get
            {
                return this.maxMateriel.Value - this.materiel.Value;
            }
        }
    }
}

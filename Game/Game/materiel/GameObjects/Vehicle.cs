using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;
using MyGame.Server;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.materiel.GameObjects;

namespace MyGame.materiel.GameObjects
{
    public abstract class Vehicle : MaterielContainer, IPlayerControlled
    {
        public const float distancePerMateriel = 400;
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("VehicleBody"), Color.White, TextureLoader.GetTexture("VehicleBody").CenterOfMass, .9f);
        public override Collidable Collidable
        {
            get { return collidable; }
        }

        private GameObjectReferenceField<Company> company;
        private GameObjectReferenceField<PlayerGameObject> controllingPlayer;
        public const float maxSpeed = 100;

        public Vehicle(GameObjectCollection collection)
            : base(collection)
        {
            controllingPlayer = new GameObjectReferenceField<PlayerGameObject>(this);
            company = new GameObjectReferenceField<Company>(this);            
        }

        public static void ServerInitialize(Vehicle vic, PlayerGameObject controllingPlayer, Vector2 position, float maxMateriel)
        {
            vic.controllingPlayer.Value = controllingPlayer;
            MaterielContainer.ServerInitialize(vic, position, 0, maxMateriel, maxMateriel);
        }

        public Company Company
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



        public void MoveToward(Vector2 targetPos, float seconds, out Vector2 resultPosition, out float secondsRemaining, out float cost)
        {
            float maxMoveDistance = Math.Min(maxSpeed * seconds, this.Range);
            float distanceToTarget = Vector2.Distance(this.Position, targetPos);
            resultPosition = Utils.PhysicsUtils.MoveTowardBounded(this.Position, targetPos, maxMoveDistance);
            maxMoveDistance = Math.Min(distanceToTarget, maxMoveDistance);            
            cost = this.MoveCost(maxMoveDistance);
            secondsRemaining = seconds - (maxMoveDistance / maxSpeed);
        }

        public void MoveTowardAndIdle(Vector2 targetPos, float seconds)
        {
            if (targetPos != this.Position)
            {
                float angleToTarget = Utils.Vector2Utils.Vector2Angle(targetPos - this.Position);
                this.Direction = Utils.PhysicsUtils.AngularMoveTowardBounded(this.Direction, angleToTarget, 2 * seconds);

                if (this.Direction == angleToTarget)
                {
                    Vector2 resultPosition;
                    float secondsRemaining;
                    float cost;

                    this.MoveToward(targetPos, seconds, out resultPosition, out secondsRemaining, out cost);

                    this.Position = resultPosition;
                    this.Materiel = Math.Max(this.Materiel - cost, 0f);
                }
            }
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
            if (controllingPlayer.Value == null)
            {
                this.Collidable.Draw(graphics, this.Position, this.Direction);
            }
            else
            {
                this.Collidable.Draw(graphics, this.Position, this.Direction, controllingPlayer.Value.Color);
            }
            graphics.DrawDebugFont(this.Materiel.ToString(), this.Position + new Vector2(25, 0), 1f);
        }

        public virtual void DrawScreen(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics, Camera camera, Color color, float depth)
        {
            Vector2 point = camera.WorldToScreenPosition(this.Position);
            graphics.DrawCircle(point, 15, color, depth);
        }

        public float Range
        {
            get
            {
                return distancePerMateriel * this.Materiel;
            }
        }

        public float MoveCost(float distance)
        {
            return distance / distancePerMateriel;
        }

        public float MaxSpeed
        {
            get
            {
                return maxSpeed;
            }
        }

        public virtual void ResupplyComplete()
        {
        }
    }
}

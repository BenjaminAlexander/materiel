using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Bullets;
using MyGame.GameServer;
using MyGame.GameClient;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    abstract public class Ship : CompositePhysicalObject 
    {
        private IntegerGameObjectMember health;
        private FloatGameObjectMember maxSpeed;
        private Vector2GameObjectMember targetPosition;

        public Ship(Game1 game)
            : base(game)
        {
            health = new IntegerGameObjectMember(this, 40);
            maxSpeed = new FloatGameObjectMember(this, 300);
            targetPosition = new Vector2GameObjectMember(this, new Vector2(0));
        }

        public static void ServerInitialize(Ship ship, Vector2 position, Vector2 velocity, float direction, int health, float maxSpeed, float acceleration, float maxAgularSpeed)
        {
            CompositePhysicalObject.ServerInitialize(ship, position, direction);
            ship.health.Value = health;
            ship.maxSpeed.Value = maxSpeed;
            ship.targetPosition.Value = position;
        }

        public static float MaxRadius
        {
            //TODO: can't we compute this for each texture at load time?
            get { return 600; }
        }

        public int Health
        {
            protected set { health.Value = value; }
            get { return health.Value; }
        }

        public Vector2 TargetPosition
        {
            set
            {
                this.targetPosition.Value = value;
            }
        }

        public override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
        {
        }

        public override void ServerOnlyUpdate(float seconds)
        {
            base.ServerOnlyUpdate(seconds);
            //TODO: wut?
        }

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);
            Vector2 velocity = this.targetPosition.Value - this.Position;
            velocity.Normalize();
            velocity = velocity * maxSpeed.Value * seconds;
            this.Position = Utils.PhysicsUtils.MoveTowardBounded(this.Position, this.targetPosition.Value, maxSpeed.Value * seconds);
        }

        public override void SimulationStateOnlyUpdate(float seconds)
        {
            base.SimulationStateOnlyUpdate(seconds);
            /*
            foreach (GameObject obj in this.Game.GameObjectCollection.Tree.GetObjectsInCircle(this.WorldPosition(), Ship.MaxRadius + Bullet.MaxRadius))
            {
                if (obj is Bullet)
                {
                    Bullet bullet = (Bullet)obj;
                    if (!bullet.BelongsTo(this) && this.CollidesWith(bullet))
                    {
                        this.Health = this.Health - bullet.Damage;
                        bullet.Destroy();
                    }
                }
            }*/

            if (this.Health <= 0)
            {
                this.Destroy();
            }
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            base.Draw(gameTime, graphics);
        }
    }
}

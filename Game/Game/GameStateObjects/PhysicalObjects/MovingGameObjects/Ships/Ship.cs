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
    abstract public class Ship : MovingGameObject 
    {
        private IntegerGameObjectMember health;
        private FloatGameObjectMember maxSpeed;
        private FloatGameObjectMember maxAgularSpeed;

        public Ship(Game1 game)
            : base(game)
        {
            health = new IntegerGameObjectMember(this, 40);
            maxSpeed = new FloatGameObjectMember(this, 300);
            maxAgularSpeed = new FloatGameObjectMember(this, 0.5f);
        }

        public static void ServerInitialize(Ship ship, Vector2 position, Vector2 velocity, float direction, int health, float maxSpeed, float acceleration, float maxAgularSpeed)
        {
            MovingGameObject.ServerInitialize(ship, position, new Vector2(0), direction, 0, 0);
            ship.health.Value = health;
            ship.maxSpeed.Value = maxSpeed;
            ship.maxAgularSpeed.Value = maxAgularSpeed;
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

        public override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
        {
            Velocity = new Vector2(0);
        }

        public override void ServerOnlyUpdate(float seconds)
        {
            base.ServerOnlyUpdate(seconds);
            //TODO: wut?
        }

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);
        }

        public override void SimulationStateOnlyUpdate(float seconds)
        {
            base.SimulationStateOnlyUpdate(seconds);

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
            }

            if (this.Health <= 0)
            {
                this.Destroy();
            }
        }
    }
}

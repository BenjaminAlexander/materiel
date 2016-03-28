using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.DrawingUtils;
using MyGame.Server;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects
{

    public abstract class PhysicalObject : GameObject
    {
        public static Vector2 GetSimulationPosition(PhysicalObject obj)
        {
            return obj.position.SimulationValue;
        }

        public static Vector2 GetPreviousPosition(PhysicalObject obj)
        {
            return obj.position.PreviousValue;
        }

        public static Vector2 GetDrawPosition(PhysicalObject obj)
        {
            return obj.position.DrawValue;
        }

        private TreeVector2Member position;
        private InterpolatedAngleGameObjectMember direction;

        public static void ServerInitialize(PhysicalObject obj, Vector2 position, float direction)
        {
            obj.position.Value = position;
            obj.direction.Value = direction;
        }

        public abstract Collidable Collidable
        {
            get;
        }

        public PhysicalObject(GameObjectCollection collection)
            : base(collection)
        {
            position = new TreeVector2Member(this, new Vector2(0));
            direction = new InterpolatedAngleGameObjectMember(this, 0);
        }

        public Vector2 Position
        {
            protected set
            {
                if (!this.Collection.GetWorldRectangle().Contains(value))
                {
                    this.MoveOutsideWorld(this.Position, value);
                }
                else
                {
                    position.Value = value;
                }
            }
            get { return this.position.Value; }
        }

        public float Direction
        {
            get { return direction.Value; }
            set { direction.Value = Utils.Vector2Utils.RestrictAngle(value); }
        }

        public Boolean CollidesWith(PhysicalObject other)
        {
            return this.Collidable.CollidesWith(this.Position, this.Direction, other.Collidable, other.Position, other.Direction);
        }

        public abstract void MoveOutsideWorld(Vector2 position, Vector2 movePosition);

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, MyGraphicsClass graphics)
        {
            base.Draw(gameTime, graphics);
            this.Collidable.Draw(graphics, this.Position, this.Direction);
        }
    }
}

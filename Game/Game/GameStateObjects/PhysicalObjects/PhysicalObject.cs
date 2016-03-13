using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.DrawingUtils;
using MyGame.Server;
using MyGame.Client;

namespace MyGame.GameStateObjects.PhysicalObjects
{

    public abstract class PhysicalObject : GameObject
    {
        public static void ServerInitialize(PhysicalObject obj, Vector2 position, float direction)
        {
            obj.position.Value = position;
            obj.direction.Value = direction;
        }

        public abstract Collidable Collidable
        {
            get;
        }

        private InterpolatedVector2GameObjectMember position;
        private InterpolatedAngleGameObjectMember direction;

        public PhysicalObject(Game1 game)
            : base(game)
        {
            position = new InterpolatedVector2GameObjectMember(this, new Vector2(0));
            direction = new InterpolatedAngleGameObjectMember(this, 0);
        }

        public Vector2 Position
        {
            protected set
            {
                if (!this.Game.GameObjectCollection.GetWorldRectangle().Contains(value))
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

        public Vector2 DrawPosition
        {
            get
            {
                Vector2 val = this.Position;
                return val;
            }
        }

        public float Direction
        {
            get { return direction.Value; }
            set { direction.Value = Utils.Vector2Utils.RestrictAngle(value); }
        }

        public void MoveInTree()
        {
            this.Game.GameObjectCollection.Tree.Move(this);
        }

        public PhysicalObject Root()
        {
            return this;
        }

        public Boolean CollidesWith(PhysicalObject other)
        {
            return this.Collidable.CollidesWith(this.WorldPosition(), this.WorldDirection(), other.Collidable, other.WorldPosition(), other.WorldDirection());
        }

        public Vector2 WorldPosition()
        {
            return this.Position;
        }


        public float WorldDirection()
        {
            return this.Direction;
        }

        public abstract void MoveOutsideWorld(Vector2 position, Vector2 movePosition);

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            base.Draw(gameTime, graphics);
            this.Collidable.Draw(graphics, this.Position, this.Direction);
        }

        public override void SimulationStateOnlyUpdate(float seconds)
        {
            base.SimulationStateOnlyUpdate(seconds);
            this.MoveInTree();
        }

        public override void LatencyAdjustment(GameTime gameTime, long messageTimeStamp)
        {
            this.MoveInTree();
            base.LatencyAdjustment(gameTime, messageTimeStamp);
        }
    }
}

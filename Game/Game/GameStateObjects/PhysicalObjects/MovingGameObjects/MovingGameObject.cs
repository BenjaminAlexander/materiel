using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.Server;
using MyGame.Client;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects
{
    public abstract class MovingGameObject : PhysicalObject
    {
        private InterpolatedVector2GameObjectMember velocity;

        public MovingGameObject(Game1 game)
            : base(game)
        {
            velocity = new InterpolatedVector2GameObjectMember(this, new Vector2(0));
        }

        public static void ServerInitialize(MovingGameObject obj, Vector2 position, Vector2 velocity, float direction)
        {
            PhysicalObject.ServerInitialize(obj, position, direction);
            obj.velocity.Value = velocity;
        }

        public Vector2 Velocity
        {
            get { return velocity.Value; }
            protected set { velocity.Value = value; }
        }

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);

            this.Position = this.Position + (this.Velocity * seconds);

        }
    }
}

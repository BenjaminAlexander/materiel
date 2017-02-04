using System;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects
{

    public abstract class PhysicalObject : GameObject
    {
        private InterpolatedVector2GameObjectMember position;
        private InterpolatedAngleGameObjectMember direction;

        public static void ServerInitialize(PhysicalObject obj, Vector2 position, float direction)
        {
            obj.position.Value = position;
            obj.direction.Value = direction;
            obj.Collection.MoveInTree(obj);
        }

        public abstract LoadedTexture Texture
        {
            get;
        }

        public abstract Vector2 TextureOrigin
        {
            get;
        }

        public PhysicalObject(GameObjectCollection collection)
            : base(collection)
        {
            position = new InterpolatedVector2GameObjectMember(this, new Vector2(0));
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
                    this.Collection.MoveInTree(this);
                }
            }
            get { return this.position.Value; }
        }

        public float Direction
        {
            get { return direction.Value; }
            set
            {
                direction.Value = Utils.Vector2Utils.RestrictAngle(value);
                this.Collection.MoveInTree(this);
            }
        }

        public Vector2 GetPosition(GameObjectField.Modes mode)
        {
            return this.position.GetValue(mode);
        }

        public float GetDirection(GameObjectField.Modes mode)
        {
            return this.direction.GetValue(mode);
        }

        public Boolean CollidesWith(PhysicalObject other)
        {
            return this.Texture.CollidesWith(this.Position, this.TextureOrigin, this.Direction, other.Texture, other.Position, other.TextureOrigin, other.Direction);
        }

        public Boolean CollidesWith(PhysicalObject other, GameObjectField.Modes mode)
        {
            return this.Texture.CollidesWith(this.GetPosition(mode), this.TextureOrigin, this.GetDirection(mode), other.Texture, other.GetPosition(mode), other.TextureOrigin, other.GetDirection(mode));
        }

        public Boolean CollidesWith(Vector2 point)
        {
            return this.Texture.Contains(point, this.Position, this.TextureOrigin, this.Direction);
        }

        public Boolean CollidesWith(Vector2 point, GameObjectField.Modes mode)
        {
            return this.Texture.Contains(point, this.GetPosition(mode), this.TextureOrigin, this.GetDirection(mode));
        }

        public Rectangle BoundingRectangle
        {
            get
            {
                return this.Texture.TransformBoundingRectangle(this.Position, this.TextureOrigin, this.Direction);
            }
        }

        public Rectangle GetBoundingRectangle(GameObjectField.Modes mode)
        {
            return this.Texture.TransformBoundingRectangle(this.GetPosition(mode), this.TextureOrigin, this.GetDirection(mode));
        }

        public abstract void MoveOutsideWorld(Vector2 position, Vector2 movePosition);

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, MyGraphicsClass graphics)
        {
            base.Draw(gameTime, graphics);
            this.Texture.Draw(graphics, this.Position, this.TextureOrigin, this.Direction, Color.White, 1);
        }

        protected override void Smooth(float secondsElapsed)
        {
            base.Smooth(secondsElapsed);
            this.Collection.MoveInTree(this);
        }

        public override void ApplyMessageComplete()
        {
            base.ApplyMessageComplete();
            this.Collection.MoveInTree(this);
        }

        public override void DrawScreen(GameTime gameTime, MyGraphicsClass graphics)
        {
            base.DrawScreen(gameTime, graphics);
            //graphics.DrawWorldRectangleOnScreen(this.BoundingRectangle, Color.Red, 1f);
        }
    }
}

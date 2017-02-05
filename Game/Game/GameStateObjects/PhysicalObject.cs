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

        private float maxRadiusFromOrigin;

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

            Rectangle r = this.Texture.BoundingRectangle;

            Vector2 p1 = new Vector2(r.X, r.Y);
            Vector2 p2 = new Vector2(r.X, r.Y + r.Height);
            Vector2 p3 = new Vector2(r.X + r.Width, r.Y);
            Vector2 p4 = new Vector2(r.X + r.Width, r.Y + r.Height);

            maxRadiusFromOrigin = Vector2.Distance(p1, this.TextureOrigin);
            maxRadiusFromOrigin = Math.Max(maxRadiusFromOrigin, Vector2.Distance(p2, this.TextureOrigin));
            maxRadiusFromOrigin = Math.Max(maxRadiusFromOrigin, Vector2.Distance(p3, this.TextureOrigin));
            maxRadiusFromOrigin = Math.Max(maxRadiusFromOrigin, Vector2.Distance(p4, this.TextureOrigin));
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

        public Rectangle GetFastBoundingRectangle(GameObjectField.Modes mode)
        {
            Vector2 p = this.GetPosition(mode);
            Rectangle r = new Rectangle((int)Math.Floor(p.X - maxRadiusFromOrigin), (int)Math.Floor(p.Y - maxRadiusFromOrigin), (int)Math.Ceiling(maxRadiusFromOrigin * 2), (int)Math.Ceiling(maxRadiusFromOrigin * 2));
            return r;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects;
using MyGame.DrawingUtils;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.materiel.GameObjects
{
    class Bullet : PhysicalObject
    {
        public override LoadedTexture Texture
        {
            get
            {
                return TextureLoader.GetTexture("Bullet");
            }
        }

        public override Vector2 TextureOrigin
        {
            get
            {
                return this.Texture.CenterOfMass;
            }
        }

        public override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
        {
            this.Destroy();
        }

        public Bullet(GameObjectCollection collection)
            : base(collection)
        {

        }

        public static void ServerInitialize(Bullet bullet, Vector2 position, float direction)
        {
            PhysicalObject.ServerInitialize(bullet, position, direction);
        }

        public static Bullet BulletFactory(GameObjectCollection collection, Vector2 position, float direction)
        {
            Bullet bullet = new Bullet(collection);
            collection.Add(bullet);
            Bullet.ServerInitialize(bullet, position, direction);
            return bullet;
        }

        public override void SubclassUpdate(float secondsElapsed)
        {
            base.SubclassUpdate(secondsElapsed);
            this.Position = this.Position + Utils.Vector2Utils.RotateVector2(new Vector2(1000f * secondsElapsed, 0), this.Direction);
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            base.Draw(gameTime, graphics);
            this.Texture.Draw(graphics, this.Position, this.TextureOrigin, this.Direction, Color.White, .9f);
        }
    }
}

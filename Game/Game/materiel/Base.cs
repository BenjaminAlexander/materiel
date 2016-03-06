using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects.PhysicalObjects;
using MyGame.DrawingUtils;
using Microsoft.Xna.Framework;
using MyGame.GameServer;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;

namespace MyGame.materiel
{
    class Base : CompositePhysicalObject
    {
        static Collidable collidable = new Collidable(TextureLoader.GetTexture("Star"), Color.Black, new Vector2(25), .1f);
        public override Collidable Collidable
        {
            get
            {
                return collidable;
            }
        }

        private float timeTillSpawn = 5;
        private int count = 0;
        private static Vector2 MaterielYard(Vector2 start, int index)
        {
            int spacing = 12;
            int sideLength = (int)Math.Sqrt(index);
            int remaining = index - sideLength * sideLength;
            Vector2 rtn = start + new Vector2(sideLength * spacing, 0);
            while (remaining > 0 && sideLength > 0)
            {
                rtn = rtn + new Vector2(0, spacing);
                remaining--;
                sideLength--;
            }

            while (remaining > 0)
            {
                rtn = rtn + new Vector2(-spacing, 0);
                remaining--;
            }
            return rtn;
        }

        private GameObjectReferenceField<PlayerGameObject> controllingPlayer;

        public static Base BaseFactory(ServerGame game, Vector2 position)
        {
            Base baseObj = new Base(game);
            Base.ServerInitialize(baseObj, position);
            game.GameObjectCollection.Add(baseObj);
            return baseObj;
        }

        public static void ServerInitialize(Base obj, Vector2 position)
        {
            CompositePhysicalObject.ServerInitialize(obj, position, 0);
        }

        public Base(Game1 game)
            : base(game)
        {
            controllingPlayer = new GameObjectReferenceField<PlayerGameObject>(this);
        }

        public override void ServerOnlyUpdate(float secondsElapsed)
        {
            base.ServerOnlyUpdate(secondsElapsed);
            if (controllingPlayer.Value != null)
            {
                timeTillSpawn = timeTillSpawn - secondsElapsed;
                if (timeTillSpawn < 0)
                {
                    timeTillSpawn = .1f;
                    Vector2 pos = MaterielYard(this.Position + new Vector2(50), count);//Utils.RandomUtils.RandomVector2(new Vector2(4000));
                    count++;

                    Tricon.TriconFactory(this.Game, pos, controllingPlayer.Value);
                    
                }
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            if (controllingPlayer.Value == null)
            {
                this.Collidable.Draw(graphics, this.Position, this.Direction);
            }
            else
            {
                this.Collidable.Draw(graphics, this.Position, this.Direction, controllingPlayer.Value.Color);
            }
        }
    
        public override void MoveOutsideWorld(Microsoft.Xna.Framework.Vector2 position, Microsoft.Xna.Framework.Vector2 movePosition)
        {
 	        throw new NotImplementedException();
        }

        public void SetPlayerInControll(RemotePlayer player)
        {
            if (player == null)
            {
                controllingPlayer.Value = null;
            }
            else
            {
                controllingPlayer.Value = player.GameObject;
            }
        }
    }
}

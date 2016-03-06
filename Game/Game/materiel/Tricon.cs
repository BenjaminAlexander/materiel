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
    class Tricon : CompositePhysicalObject
    {
        static Collidable collidable = new Collidable(TextureLoader.GetTexture("Tricon"), Color.White, new Vector2(25), .1f);
        public override Collidable Collidable
        {
            get
            {
                return collidable;
            }
        }

        private GameObjectReferenceField<PlayerGameObject> controllingPlayer;

        public static Tricon TriconFactory(Game1 game, Vector2 position, PlayerGameObject owner)
        {
            Tricon baseObj = new Tricon(game);
            Tricon.ServerInitialize(baseObj, position);
            game.GameObjectCollection.Add(baseObj);
            baseObj.controllingPlayer.Value = owner;
            return baseObj;
        }

        public static void ServerInitialize(Tricon obj, Vector2 position)
        {
            CompositePhysicalObject.ServerInitialize(obj, position, 0);
        }

        public Tricon(Game1 game)
            : base(game)
        {
            controllingPlayer = new GameObjectReferenceField<PlayerGameObject>(this);
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

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            //base.Draw(gameTime, graphics);
            //this.Collidable.Draw(graphics, this.Position, this.Direction);
            graphics.DrawSolidRectangle(this.Position, new Vector2(10), new Vector2(0), 0, Color.Black, .5);
        }
    }
}
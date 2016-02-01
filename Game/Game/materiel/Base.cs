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
                    timeTillSpawn = 5;
                    SmallShip ship = new SmallShip(this.Game);
                    SmallShip.ServerInitialize(ship, this.Position, 0);
                    this.Game.GameObjectCollection.Add(ship);
                    ship.TargetPosition = Utils.RandomUtils.RandomVector2(new Vector2(4000));
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

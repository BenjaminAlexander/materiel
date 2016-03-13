using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects;
using MyGame.DrawingUtils;
using Microsoft.Xna.Framework;
using MyGame.Server;

namespace MyGame.materiel
{
    class Base : PhysicalObject
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
        private FloatGameObjectMember materiel;
        private IntegerQueueGameObjectField buildQueue;

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
            PhysicalObject.ServerInitialize(obj, position, 0);
        }

        public Base(Game1 game)
            : base(game)
        {
            materiel = new FloatGameObjectMember(this, 0);
            controllingPlayer = new GameObjectReferenceField<PlayerGameObject>(this);
            buildQueue = new IntegerQueueGameObjectField(this);
        }

        public void BuildCombatVehicle()
        {
            buildQueue.Value.Enqueue(0);
        }

        public override void ServerOnlyUpdate(float secondsElapsed)
        {
            base.ServerOnlyUpdate(secondsElapsed);
            if (controllingPlayer.Value != null)
            {
                timeTillSpawn = timeTillSpawn - secondsElapsed;
                if (timeTillSpawn < 0)
                {
                    timeTillSpawn = 5f;
                    if (buildQueue.Value.Count > 0)
                    {
                        buildQueue.Value.Dequeue();
                        Vehicle.VehicleFactory((ServerGame)this.Game, this.controllingPlayer.Value, Utils.RandomUtils.RandomVector2(new Vector2(100)) + this.Position);
                    }
                    else
                    {
                        materiel.Value = materiel + 1f;
                    }
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
            graphics.DrawDebugFont(materiel.Value.ToString(), this.Position + new Vector2(25, 0) , 1f);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects;
using MyGame.DrawingUtils;
using Microsoft.Xna.Framework;
using MyGame.Server;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.materiel
{
    public class Base : PhysicalObject, IPlayerControlled
    {
        static Collidable collidable = new Collidable(TextureLoader.GetTexture("Star"), Color.Black, new Vector2(25), .1f);
        public override Collidable Collidable
        {
            get
            {
                return collidable;
            }
        }

        private FloatGameObjectMember spawnCountDown;
        private FloatGameObjectMember timeTillSpawn;
        private GameObjectReferenceQueueField<Vehicle> resupplyQueue;

        private FloatGameObjectMember materiel;
        private IntegerQueueGameObjectField buildQueue;
        private GameObjectReferenceField<PlayerGameObject> controllingPlayer;

        public static Base BaseFactory(ServerGame game, Vector2 position, float productionTime)
        {
            Base baseObj = new Base(game.GameObjectCollection);
            game.GameObjectCollection.Add(baseObj);
            Base.ServerInitialize(baseObj, position, productionTime);
            return baseObj;
        }

        public static void ServerInitialize(Base obj, Vector2 position, float productionTime)
        {
            PhysicalObject.ServerInitialize(obj, position, 0);
            obj.timeTillSpawn.Value = productionTime;
            obj.spawnCountDown.Value = productionTime;
        }

        public Base(GameObjectCollection collection)
            : base(collection)
        {
            spawnCountDown = new FloatGameObjectMember(this, 5);
            timeTillSpawn = new FloatGameObjectMember(this, 5);
            materiel = new FloatGameObjectMember(this, 0);
            controllingPlayer = new GameObjectReferenceField<PlayerGameObject>(this);
            buildQueue = new IntegerQueueGameObjectField(this);
            resupplyQueue = new GameObjectReferenceQueueField<Vehicle>(this);
        }

        public void BuildCombatVehicle()
        {
            buildQueue.Value.Enqueue(0);
        }

        public void BuildTransportVehicle()
        {
            buildQueue.Value.Enqueue(1);
        }

        public override void ServerOnlyUpdate(float secondsElapsed)
        {
            base.ServerOnlyUpdate(secondsElapsed);
            if (controllingPlayer.Value != null)
            {
                this.spawnCountDown.Value = this.spawnCountDown.Value - secondsElapsed;
                if (this.spawnCountDown.Value < 0)
                {
                    this.spawnCountDown.Value = this.timeTillSpawn.Value;
                    if (buildQueue.Value.Count > 0)
                    {
                        int buildValue = buildQueue.Value.Dequeue();
                        if (buildValue == 0)
                        {
                            CombatVehicle.CombatVehicleFactory(this.Collection, this.controllingPlayer.Value, Utils.RandomUtils.RandomVector2(new Vector2(100)) + this.Position);
                        }
                        else if (buildValue == 1)
                        {
                            Transport.TransportFactory(this.Collection, this.controllingPlayer.Value, Utils.RandomUtils.RandomVector2(new Vector2(100)) + this.Position);
                        }
                    }
                    else
                    {
                        materiel.Value = materiel + 1f;
                    }
                }
            }

        }

        public override void SimulationStateOnlyUpdate(float secondsElapsed)
        {
            base.SimulationStateOnlyUpdate(secondsElapsed);

            List<PhysicalObject> objectList = this.Collection.Tree.GetObjectsInCircle(this.Position, 50);
            Dictionary<PlayerGameObject, int> vicCount = new Dictionary<PlayerGameObject, int>();

            foreach (PhysicalObject obj in objectList)
            {
                try
                {
                    if (obj is Vehicle)
                    {
                        Vehicle vic = (Vehicle)obj;

                        PlayerGameObject player = vic.ControllingPlayer;
                        if (!vicCount.ContainsKey(player))
                        {
                            vicCount[player] = 1;
                        }
                        else
                        {
                            vicCount[player]++;
                        }
                    }
                }
                catch (FailedDereferenceException<PlayerGameObject> e)
                {

                }
            }

            try
            {
                PlayerGameObject best = null;
                foreach (PlayerGameObject player in vicCount.Keys)
                {
                    if (best == null || vicCount[best] < vicCount[player])
                    {
                        best = player;
                    }
                }

                if (best != null)
                {
                    this.controllingPlayer.Value = best;
                }
            }
            catch (Exception e)
            {

            }

        }

        public override void SubclassUpdate(float secondsElapsed)
        {
            base.SubclassUpdate(secondsElapsed);

            while (this.resupplyQueue.Value.Count > 0 && this.resupplyQueue.Value.Peek().Dereference().MaxMaterielDeposit <= this.materiel.Value)
            {
                float amount = this.resupplyQueue.Value.Peek().Dereference().MaxMaterielDeposit;
                this.materiel.Value = this.materiel.Value - amount;
                this.resupplyQueue.Value.Peek().Dereference().Materiel = this.resupplyQueue.Value.Peek().Dereference().Materiel + amount;
                this.resupplyQueue.Value.Peek().Dereference().ResupplyComplete();
                this.resupplyQueue.Value.Dequeue();
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, MyGraphicsClass graphics)
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

        public GameObjectReference<PlayerGameObject> ControllingPlayer
        {
            get
            {
                return this.controllingPlayer.Value;
            }
        }

        public void EnqueueTransport(Vehicle vic)
        {
            this.resupplyQueue.Value.Enqueue(vic);
        }

        public bool InResupplyQueue(Vehicle vic)
        {
            return this.resupplyQueue.Value.Contains(vic);
        }

        public float Materiel
        {
            get
            {
                return this.materiel.Value;
            }

            set
            {
                this.materiel.Value = value;
            }
        }
    }
}

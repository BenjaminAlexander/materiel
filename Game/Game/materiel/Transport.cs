using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;
using MyGame.Server;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.materiel
{
    public class Transport : Vehicle
    {
        private GameObjectReferenceField<Vehicle> resupplyTarget;
        private BoolGameObjectMember inQueue;

        public Vehicle ResupplyTarget
        {
            get
            {
                return this.resupplyTarget.Value;
            }
        }

        public Transport(GameObjectCollection collection)
            : base(collection)
        {
            this.resupplyTarget = new GameObjectReferenceField<Vehicle>(this);
            this.inQueue = new BoolGameObjectMember(this, false);
        }

        public static void ServerInitialize(Transport vic, PlayerGameObject controllingPlayer, Vector2 position)
        {
            Vehicle.ServerInitialize(vic, controllingPlayer, position, 20);
        }

        public static Transport TransportFactory(GameObjectCollection collection, PlayerGameObject controllingPlayer, Vector2 position)
        {
            Transport vehicle = new Transport(collection);
            collection.Add(vehicle);
            Transport.ServerInitialize(vehicle, controllingPlayer, position);
            return vehicle;
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            base.Draw(gameTime, graphics);
            graphics.DrawDebugFont("T", this.Position + new Vector2(25, 15), 1f);
        }

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);
            
            if (this.ResupplyTarget != null)
            {
                if (this.DistanceToResupplyTargetAndBack() > this.Range())
                {
                    this.resupplyTarget.Value = null;
                }
                else
                {
                    this.MoveToward(this.ResupplyTarget.Position, seconds);
                    if (Vector2.Distance(this.Position, this.ResupplyTarget.Position) < 10)
                    {
                        float amount = Math.Min(this.ExcessMateriel(), this.ResupplyTarget.ResupplyAmount);
                        this.Materiel = this.Materiel - amount;
                        this.ResupplyTarget.Materiel = this.ResupplyTarget.Materiel + amount;

                        if (this.Company.Dereference() != null)
                        {
                            resupplyTarget.Value = this.Company.Dereference().NextResupply();
                        }
                    }
                }
            }
            

            if (!this.inQueue.Value && this.ResupplyTarget == null)
            {
                if (this.ResupplyPoint() != null)
                {
                    this.MoveToward(this.ResupplyPoint().Position, seconds);
                    if (this.DistanceToResupplyPoint() < 10)
                    {
                        this.ResupplyPoint().EnqueueTransport(this);
                        this.inQueue.Value = true;
                    }
                }
            }
        }

        public Base ResupplyPoint()
        {
            if (this.Company.Dereference() != null)
            {
                return this.Company.Dereference().ResupplyPoint;
            }
            return null;
        }

        public float DistanceToResupplyPoint()
        {
            if (this.ResupplyPoint() != null)
            {
                return Vector2.Distance(this.Position, this.ResupplyPoint().Position);
            }
            return float.MaxValue;
        }

        public float DistanceToResupplyTarget()
        {
            if (this.ResupplyTarget != null)
            {
                return Vector2.Distance(this.Position, this.ResupplyTarget.Position);
            }
            return float.MaxValue;
        }

        public float DistanceToResupplyTargetAndBack()
        {
            if (this.ResupplyTarget != null && this.ResupplyPoint() != null)
            {
                return Vector2.Distance(this.Position, this.ResupplyTarget.Position) + Vector2.Distance(this.ResupplyTarget.Position, this.ResupplyPoint().Position);
            }
            return float.MaxValue;
        }

        public float ExcessMateriel()
        {
            return Math.Max(this.Materiel - this.MoveCost(this.DistanceToResupplyTargetAndBack()), 0);
        }

        public override void ResupplyComplete()
        {
            this.inQueue.Value = false;
            if (this.Company.Dereference() != null)
            {
                resupplyTarget.Value = this.Company.Dereference().NextResupply();
            }
        }
    }
}

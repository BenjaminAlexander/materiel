using System;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.materiel.GameObjects
{
    public class Transport : Vehicle
    {
        private GameObjectReferenceField<CombatVehicle> vicToResupply;
        private GameObjectReferenceField<Base> base1;
        private GameObjectReferenceField<Base> base2;

        public Transport(GameObjectCollection collection)
            : base(collection)
        {
            vicToResupply = new GameObjectReferenceField<CombatVehicle>(this);
            base1 = new GameObjectReferenceField<Base>(this);
            base2 = new GameObjectReferenceField<Base>(this);
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

        public void SetResupplyRoute(Base base1, Base base2)
        {
            if(this.Company != null)
            {
                this.Company.RemoveVehicle(this);
                this.Company = null;
            }

            this.vicToResupply.Value = null;
            this.base1.Value = base1;
            this.base2.Value = base2;
        }

        public CombatVehicle VicToResupply
        {
            get
            {
                return this.vicToResupply.Value;
            }
        }

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);

            if (this.Company != null)
            {
                this.base1.Value = null;
                this.base2.Value = null;

                if (this.vicToResupply.Value == null)
                {
                    this.vicToResupply.Value = this.Company.NextVehicleToResupply();
                }
            }

            //this code should work
            if (this.TransportFrom != null)
            {
                if (this.TransportTo == null || this.CostToResupplyAndBack() >= this.Materiel)
                {
                    this.vicToResupply.Value = null;
                    this.MoveTowardAndIdle(this.TransportFrom.Position, seconds);
                    if (this.DistanceToResupplyPoint() < 10 && !this.TransportFrom.InResupplyQueue(this))
                    {
                        this.TransportFrom.EnqueueTransport(this);
                    }
                }
                else
                {
                    this.MoveTowardAndIdle(this.TransportTo.Position, seconds);
                    if (Vector2.Distance(this.Position, this.TransportTo.Position) < 10)
                    {
                        MaterielContainer.MaximumMaterielTransfer(this, this.TransportTo);
                        this.vicToResupply.Value = null;
                    }
                }
            }
        }

        public Base TransportFrom
        {
            get
            {
                if (this.Company != null)
                {
                    return this.Company.ResupplyPoint;
                }
                else if (this.base1.Value != null & this.base2.Value != null)
                {
                    return base1.Value;
                }
                return null;
            }
        }

        public MaterielContainer TransportTo
        {
            get
            {
                if (this.Company != null)
                {
                    return this.vicToResupply.Value;
                }
                else if (this.base1.Value != null & this.base2.Value != null)
                {
                    return base2.Value;
                }
                return null;
            }
        }

        public float DistanceToResupplyPoint()
        {
            if (this.TransportFrom != null)
            {
                return Vector2.Distance(this.Position, this.TransportFrom.Position);
            }
            return float.MaxValue;
        }

        public float CostToMoveToResupplyPoint()
        {
            return this.MoveCost(this.DistanceToResupplyPoint());
        }

        public override void ResupplyComplete()
        {
            if (this.Company != null)
            {
                this.vicToResupply.Value = this.Company.NextVehicleToResupply();
            }
        }

        public override float MaxMaterielWithdrawl
        {
            get
            {
                return Math.Max(0, this.Materiel - this.CostToMoveToResupplyPoint());
            }
        }

        public float CostToResupplyAndBack(MaterielContainer resupplyTarget)
        {
            if (this.TransportFrom != null)
            {
                float distance = Vector2.Distance(this.Position, resupplyTarget.Position);
                distance = distance + Vector2.Distance(this.TransportFrom.Position, resupplyTarget.Position);
                return this.MoveCost(distance);
            }
            return float.PositiveInfinity;
        }

        public float CostToResupplyAndBack()
        {
            if (this.TransportFrom != null && this.TransportTo != null)
            {
                float distance = Vector2.Distance(this.Position, this.TransportTo.Position);
                distance = distance + Vector2.Distance(this.TransportFrom.Position, this.TransportTo.Position);
                return this.MoveCost(distance);
            }
            return float.PositiveInfinity;
        }

        public float EstimatedVehicleMaterielDelivery()
        {
            return this.Materiel - this.CostToResupplyAndBack(this.VicToResupply);
        }
    }
}

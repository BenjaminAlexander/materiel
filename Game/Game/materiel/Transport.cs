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
using MyGame.materiel.GameObjects;

namespace MyGame.materiel
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

            try
            {
                if (this.Company != null)
                {
                    this.base1.Value = null;
                    this.base2.Value = null;

                    if (this.vicToResupply.Value == null)
                    {
                        this.vicToResupply.Value = this.Company.NextVehicleToResupply();
                        if (this.vicToResupply.Value != null && this.CostToResupplyVehicleAndBack() >= this.Materiel)
                        {
                            this.vicToResupply.Value = null;
                        }
                    }
                    else if(this.CostToResupplyVehicleAndBack() >= this.Materiel)
                    {
                        this.vicToResupply.Value = null;
                    }

                    if (this.vicToResupply.Value != null)
                    {
                        this.MoveTowardAndIdle(this.vicToResupply.Value.Position, seconds);

                        if (Vector2.Distance(this.Position, this.vicToResupply.Value.Position) < 10)
                        {
                            MaterielContainer.MaximumMaterielTransfer(this, this.vicToResupply.Value);
                            this.vicToResupply.Value = this.Company.NextVehicleToResupply();
                        }
                    }
                    else
                    {
                        if (this.ResupplyPoint() != null)
                        {
                            this.MoveTowardAndIdle(this.ResupplyPoint().Position, seconds);
                            if (this.DistanceToResupplyPoint() < 10 && !this.ResupplyPoint().InResupplyQueue(this))
                            {
                                this.ResupplyPoint().EnqueueTransport(this);
                            }
                        }
                    }
                }
                else if(base1.Value != null && base2.Value != null)
                {
                    if(this.CostToResupplyBaseAndBack() >= this.Materiel)
                    {
                        this.MoveTowardAndIdle(this.ResupplyPoint().Position, seconds);
                        if (this.DistanceToResupplyPoint() < 10 && !this.ResupplyPoint().InResupplyQueue(this))
                        {
                            this.ResupplyPoint().EnqueueTransport(this);
                        }
                    }
                    else
                    {
                        this.MoveTowardAndIdle(this.base2.Value.Position, seconds);

                        if (Vector2.Distance(this.Position, this.base2.Value.Position) < 10)
                        {
                            MaterielContainer.MaximumMaterielTransfer(this, this.base2.Value);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public Base ResupplyPoint()
        {
            if (this.Company != null)
            {
                return this.Company.ResupplyPoint;
            }
            else if(this.base1.Value != null &  this.base2.Value != null)
            {
                return base1.Value;
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

        public float CostToResupplyVehicleAndBack()
        {
            if (this.ResupplyPoint() != null)
            {
                float distance = Vector2.Distance(this.Position, this.vicToResupply.Value.Position);
                distance = distance + Vector2.Distance(this.ResupplyPoint().Position, this.vicToResupply.Value.Position);
                return this.MoveCost(distance);
            }
            return float.PositiveInfinity;
        }

        public float CostToResupplyBaseAndBack()
        {
            if (this.ResupplyPoint() != null && this.base2.Value != null)
            {
                float distance = Vector2.Distance(this.Position, this.base2.Value.Position);
                distance = distance + Vector2.Distance(this.ResupplyPoint().Position, this.base2.Value.Position);
                return this.MoveCost(distance);
            }
            return float.PositiveInfinity;
        }

        public float EstimatedVehicleMaterielDelivery()
        {
            return this.Materiel - this.CostToResupplyVehicleAndBack();
        }

        public float EstimatedBaseMaterielDelivery()
        {
            return this.Materiel - this.CostToResupplyVehicleAndBack();
        }
    }
}

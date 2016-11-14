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
        private GameObjectReferenceField<CombatVehicle> vicToResupply;

        public Transport(GameObjectCollection collection)
            : base(collection)
        {
            vicToResupply = new GameObjectReferenceField<CombatVehicle>(this);
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

            try
            {
                if (this.vicToResupply.Value == null || this.CostToResupplyVehicleAndBack() >= this.Materiel)
                {
                    this.vicToResupply.Value = null;
                    if (this.ResupplyPoint() != null)
                    {
                        this.MoveTowardAndIdle(this.ResupplyPoint().Position, seconds);
                        if (this.DistanceToResupplyPoint() < 10 && !this.ResupplyPoint().InResupplyQueue(this))
                        {
                            this.ResupplyPoint().EnqueueTransport(this);
                        }
                    }
                }
                else if(this.vicToResupply.Value != null)
                {
                    this.MoveTowardAndIdle(this.vicToResupply.Value.Position, seconds);

                    if (Vector2.Distance(this.Position, this.vicToResupply.Value.Position) < 10)
                    {
                        this.ResupplyCombatVehicle(this.vicToResupply.Value);
                        this.vicToResupply.Value = this.Company.NextVehicleToResupply();
                    }
                }
                
            }
            catch (Exception)
            {
            }
        }

        public override void ResupplyComplete()
        {
            if (this.Company != null)
            {
                this.vicToResupply.Value = this.Company.NextVehicleToResupply();
            }
        }

        public void ResupplyCombatVehicle(CombatVehicle vic)
        {
            float resupply = Math.Min(this.MaxMaterielWithdrawl, vic.MaxMaterielDeposit);

            this.Materiel = this.Materiel - resupply;
            vic.Materiel = vic.Materiel + resupply;
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
    }
}

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
    public class CombatVehicle : Vehicle
    {
        public const float maxMateriel = 5;
        private IntegerGameObjectMember targetFightingPos;

        public CombatVehicle(GameObjectCollection collection)
            : base(collection)
        {
            targetFightingPos = new IntegerGameObjectMember(this, -1);
        }

        public static void ServerInitialize(CombatVehicle vic, PlayerGameObject controllingPlayer, Vector2 position)
        {
            Vehicle.ServerInitialize(vic, controllingPlayer, position, maxMateriel);
        }

        public static CombatVehicle CombatVehicleFactory(GameObjectCollection collection, PlayerGameObject controllingPlayer, Vector2 position)
        {
            CombatVehicle vehicle = new CombatVehicle(collection);
            collection.Add(vehicle);
            CombatVehicle.ServerInitialize(vehicle, controllingPlayer, position);
            return vehicle;
        }

        public int TargetFightingPosition
        {
            set
            {
                if (this.Company != null)
                {
                    this.Company.RemoveFromLastVic(this);
                }
                targetFightingPos.Value = value;
                if (this.Company != null)
                {
                    this.Company.AddToLastVic(this);
                }
            }

            get
            {
                return targetFightingPos.Value;
            }
        }

        public Vector2 TargetPosition
        {
            get
            {
                if (this.Company != null)
                {
                    if (this.TargetFightingPosition >= 0 && this.Company.FightingPositions.Count > this.TargetFightingPosition)
                    {
                        return this.Company.FightingPositions[this.TargetFightingPosition];
                    }

                    this.TargetFightingPosition = -1;
                    if (this.ResupplyPoint != null)
                    {
                        return this.ResupplyPoint.Position;
                    }
                }
                return this.Position;
            }
        }

        public override void ServerOnlyUpdate(float secondsElapsed)
        {
            base.ServerOnlyUpdate(secondsElapsed);
            if (this.TargetFightingPosition == -1 && 
                this.ResupplyPoint != null &&
                Vector2.Distance(this.Position, this.ResupplyPoint.Position) < 10 &&
                !this.ResupplyPoint.InResupplyQueue(this) &&
                this.ResupplyPoint != null)
            {
                this.ResupplyPoint.EnqueueTransport(this);
            }

            if (this.TargetFightingPosition != -1 &&
                this.Position == this.TargetPosition &&
                this.Company != null)
            {
                this.Company.OccupyFightingPosition(this);
            }
        }

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);

            Vector2 resultPosition;
            float secondsRemaining;
            float cost;

            this.MoveToward(this.TargetPosition, seconds, out resultPosition, out secondsRemaining, out cost);
            cost = cost + this.IdleCost(secondsRemaining);

            if (this.ResupplyPoint == null ||
                this.Materiel - cost < this.MoveCost(Vector2.Distance(this.Position, this.TargetPosition)) + this.MoveCost(Vector2.Distance(this.ResupplyPoint.Position, this.TargetPosition)))
            {
                this.TargetFightingPosition = -1;
            }

            this.MoveTowardAndIdle(this.TargetPosition, seconds);
        }

        public override void ResupplyComplete()
        {
            base.ResupplyComplete();
            if (this.Company != null)
            {
                this.TargetFightingPosition = this.Company.NextFightingPosition(this);
            }
        }

        public float TimeUntilFightingPositionAbondoned()
        {
            return TimeUntilFightingPositionAbondoned(this.TargetFightingPosition);
        }
        
        public float TimeUntilFightingPositionAbondoned(int index)
        {
            if (this.Company == null ||
                this.Company.ResupplyPoint == null ||
                !(this.Company.FightingPositions.Count > index && index >= 0))
            {
                return 0;
            }

            Vector2 fightingPosition = this.Company.FightingPositions[index];
            float distanceToPosition = Vector2.Distance(this.Position, fightingPosition);
            float distanceFightingPositionToBase = Vector2.Distance(this.Company.ResupplyPoint.Position, fightingPosition);
            float timeToTarget = distanceToPosition / this.MaxSpeed;
            float idleMateriel = this.Materiel - this.MoveCost(distanceToPosition) - this.MoveCost(distanceFightingPositionToBase);

            float time = timeToTarget + this.IdleTime(idleMateriel);
            return time;
        }

        public Base ResupplyPoint
        {
            get
            {
                if (this.Company == null)
                {
                    return null;
                }
                else
                {
                    return this.Company.ResupplyPoint;
                }
            }
        }
    }
}

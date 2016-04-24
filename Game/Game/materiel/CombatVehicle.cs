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
        private GameObjectReferenceField<VehiclePosition> targetFightingPosition;

        public CombatVehicle(GameObjectCollection collection)
            : base(collection)
        {
            targetFightingPos = new IntegerGameObjectMember(this, -1);
            targetFightingPosition = new GameObjectReferenceField<VehiclePosition>(this);
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
                
                if (this.targetFightingPosition.Value != null)
                {
                    this.targetFightingPosition.Value.Remove(this);
                }
                if (this.Company != null)
                {
                    this.Company.RemoveFromLastVic(this);
                    if (value == -1)
                    {
                        this.targetFightingPosition.Value = null;
                    }
                    else
                    {
                        this.targetFightingPosition.Value = this.Company.VehiclePositions[value];
                        this.targetFightingPosition.Value.Add(this);
                    }
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
                if (this.targetFightingPosition.Value != null)
                {
                    return this.targetFightingPosition.Value.Position;
                }
                else if (this.ResupplyPoint != null)
                {
                    return this.ResupplyPoint.Position;
                }
                return this.Position;
            }
        }

        public VehiclePosition VehicleFightingPosition
        {
            get
            {
                return this.targetFightingPosition.Value;
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

            if (this.targetFightingPosition.Value != null)
            {
                this.targetFightingPosition.Value.OccupyPosition(this);
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
                if (this.targetFightingPosition.Value != null)
                {
                    this.targetFightingPosition.Value.Remove(this);
                }
                this.targetFightingPosition.Value = null;
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
            if (this.targetFightingPosition.Value != null)
            {
                return TimeUntilFightingPositionAbondoned(this.targetFightingPosition.Value.Position);
            }
            else
            {
                return 0;
            }
        }
        
        public float TimeUntilFightingPositionAbondoned(int index)
        {
            if (this.Company == null ||
                this.Company.ResupplyPoint == null ||
                !(this.Company.VehiclePositions.Count > index && index >= 0))
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

        public float TimeUntilFightingPositionAbondoned(Vector2 pos)
        {
            if (this.Company == null ||
                this.Company.ResupplyPoint == null)
            {
                return 0;
            }

            float distanceToPosition = Vector2.Distance(this.Position, pos);
            float distanceFightingPositionToBase = Vector2.Distance(this.Company.ResupplyPoint.Position, pos);
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

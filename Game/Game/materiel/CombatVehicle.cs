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
    public class CombatVehicle : Vehicle, IComparable
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
                Company co = this.Company.Dereference();
                if (co != null)
                {
                    co.RemoveFromLastVic(this);
                }
                targetFightingPos.Value = value;
                if (co != null)
                {
                    co.AddToLastVic(this);
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
                Company co = this.Company.Dereference();
                if (co != null)
                {
                    if (this.TargetFightingPosition >= 0 && co.FightingPositions.Count > this.TargetFightingPosition)
                    {
                        return co.FightingPositions[this.TargetFightingPosition];
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

        public bool InResupplyQueue()
        {
            if (this.ResupplyPoint != null)
            {
                return this.ResupplyPoint.InResupplyQueue(this);
            }
            return false;
        }

        public override void ServerOnlyUpdate(float secondsElapsed)
        {
            base.ServerOnlyUpdate(secondsElapsed);
            if (this.TargetFightingPosition == -1 && this.DistanceToResuplyPoint() < 10 && !this.InResupplyQueue() && this.ResupplyPoint != null)
            {
                this.ResupplyPoint.EnqueueTransport(this);
            }
        }

        public override void ResupplyComplete()
        {
            base.ResupplyComplete();
            if (this.Company.Dereference() != null)
            {
                this.TargetFightingPosition = this.Company.Dereference().NextFightingPosition(this);
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

            if (this.Materiel - cost < this.MoveCostToTarget() + this.CostToReturnToBaseFromFightingPosition())
            {
                this.TargetFightingPosition = -1;
            }
            else if (this.TargetFightingPosition != -1 &&
                this.Position != this.TargetPosition &&
                resultPosition == this.TargetPosition &&
                this.Company.Dereference() != null)
            {
                this.Company.Dereference().OccupyFightingPosition(this);
            }

            float secondsLeft = this.MoveToward(this.TargetPosition, seconds);
            this.Idle(secondsLeft);
        }

        public bool InTargetFightingPosition
        {
            get
            {
                return this.TargetPosition == this.Position;
            }
        }

        public float MoveCostToTarget()
        {
            return this.MoveCost(Vector2.Distance(this.Position, this.TargetPosition));
        }

        public float DistanceToTarget()
        {
            return Vector2.Distance(this.Position, this.TargetPosition);
        }

        public float DistanceToResuplyPoint()
        {
            Base rsp = this.ResupplyPoint;
            if (rsp != null)
            {
                return Vector2.Distance(this.Position, this.ResupplyPoint.Position);
            }
            else
            {
                return float.PositiveInfinity;
            }
        }

        public float TimeUntilFightingPositionAbondoned()
        {
            return TimeUntilFightingPositionAbondoned(this.TargetFightingPosition);
        }
        
        public float TimeUntilFightingPositionAbondoned(int index)
        {
            Company co = this.Company.Dereference();
            if (co == null || co.ResupplyPoint == null || !co.FightingPositionExists(index))
            {
                return 0;
            }

            Vector2 fightingPosition = co.FightingPositions[index];
            float distanceToPosition = Vector2.Distance(this.Position, fightingPosition);
            float distanceFightingPositionToBase = Vector2.Distance(co.ResupplyPoint.Position, fightingPosition);
            float timeToTarget = distanceToPosition / this.MaxSpeed;
            float idleMateriel = this.Materiel - this.MoveCost(distanceToPosition) - this.MoveCost(distanceFightingPositionToBase);

            float time = timeToTarget + this.IdleTime(idleMateriel);
            return time;
        }

        public float TimeUntilMaterielEmpty()
        {
            float moveCost = this.MoveCostToTarget();
            if (moveCost >= this.Materiel)
            {
                return this.Range() / this.MaxSpeed;
            }
            else
            {
                float timeToTarget = this.DistanceToTarget() / this.MaxSpeed;
                float idleMateriel = this.Materiel - moveCost;
                float idleTime = this.IdleTime(idleMateriel);
                return timeToTarget + idleTime;
            }
        }

        public Base ResupplyPoint
        {
            get
            {
                Company co = this.Company.Dereference();
                if (co == null)
                {
                    return null;
                }
                else
                {
                    return co.ResupplyPoint;
                }
            }
        }

        public float CostToReturnToBase()
        {
            return this.MoveCost(this.DistanceToResuplyPoint());
        }

        public float CostToReturnToBaseFromFightingPosition()
        {
            if(this.ResupplyPoint == null)
            {
                return float.PositiveInfinity;
            }
            float distance = Vector2.Distance(this.ResupplyPoint.Position, this.TargetPosition);
            return this.MoveCost(distance);
        }

        // -1 = supply this first
        // 1 = supply obj first
        public int CompareTo(object obj)
        {
            CombatVehicle other = (CombatVehicle)obj;
            if (this.TimeUntilMaterielEmpty() != other.TimeUntilMaterielEmpty())
            {
                return Math.Sign(this.TimeUntilMaterielEmpty() - other.TimeUntilMaterielEmpty());
            }

            if (other.DistanceToTarget() != this.DistanceToTarget())
            {
                return Math.Sign(other.DistanceToTarget() - this.DistanceToTarget());
            }

            return Math.Sign(this.Materiel - other.Materiel);
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            base.Draw(gameTime, graphics);
            graphics.DrawDebugFont(this.TimeUntilFightingPositionAbondoned().ToString(), this.Position + new Vector2(-25, 0), 1f);
        }
    }
}

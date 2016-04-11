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
                targetFightingPos.Value = value;
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
                if (this.Company.Dereference() != null)
                {
                    return this.Company.Dereference().FightingPosition(this.targetFightingPos.Value);
                }
                else
                {
                    return this.Position;
                }
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
                this.TargetFightingPosition = this.Company.Dereference().NextPosIndex;
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

        public override void DrawScreen(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics, Camera camera, Color color, float depth)
        {
            base.DrawScreen(gameTime, graphics, camera, color, depth);
            if (this.Position != this.TargetPosition)
            {
                graphics.DrawCircle(camera.WorldToScreenPosition(this.TargetPosition), 15, color, depth);
            }

            Vector2 screenPos1 = camera.WorldToScreenPosition(this.Position);
            Vector2 screenPos2 = camera.WorldToScreenPosition(this.TargetPosition);

            if (Vector2.Distance(screenPos1, screenPos2) > 30)
            {
                Vector2 point1 = Utils.PhysicsUtils.MoveTowardBounded(screenPos1, screenPos2, 15);
                Vector2 point2 = Utils.PhysicsUtils.MoveTowardBounded(screenPos2, screenPos1, 15);
                graphics.DrawLine(point1, point2, color, depth);
            }
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

        public void HandoffExtraMateriel(CombatVehicle other)
        {
            float amount = this.Materiel - this.MoveCost(this.DistanceToResuplyPoint());
            amount = Math.Min(amount, other.ResupplyAmount);
            amount = Math.Max(amount, 0);

            other.Materiel = other.Materiel + amount;
            this.Materiel = this.Materiel - amount;
        }
        
        public float TimeUntilFightingPositionAbondoned()
        {
            Company co = this.Company.Dereference();
            if (co == null || this.TargetFightingPosition == -1)
            {
                return 0;
            }

            float timeToTarget = this.DistanceToTarget() / this.MaxSpeed;
            float idleMateriel = this.Materiel - this.MoveCostToTarget() - this.CostToReturnToBaseFromFightingPosition();

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
    }
}

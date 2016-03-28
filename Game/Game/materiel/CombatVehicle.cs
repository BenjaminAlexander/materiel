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
        private Vector2GameObjectMember targetPosition;

        public CombatVehicle(GameObjectCollection collection)
            : base(collection)
        {
            targetPosition = new Vector2GameObjectMember(this, new Vector2(0));
        }

        public static void ServerInitialize(CombatVehicle vic, PlayerGameObject controllingPlayer, Vector2 position)
        {
            Vehicle.ServerInitialize(vic, controllingPlayer, position, 5);
            vic.targetPosition.Value = position;
        }

        public static CombatVehicle CombatVehicleFactory(GameObjectCollection collection, PlayerGameObject controllingPlayer, Vector2 position)
        {
            CombatVehicle vehicle = new CombatVehicle(collection);
            collection.Add(vehicle);
            CombatVehicle.ServerInitialize(vehicle, controllingPlayer, position);
            return vehicle;
        }

        public Vector2 TargetPosition
        {
            set
            {
                this.targetPosition.Value = value;
            }

            get
            {
                return this.targetPosition;
            }
        }

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);
            this.MoveToward(this.TargetPosition, seconds);
        }

        public override void DrawScreen(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics, Camera camera, Color color, float depth)
        {
            base.DrawScreen(gameTime, graphics, camera, color, depth);
            if (this.Position != this.targetPosition)
            {
                graphics.DrawCircle(camera.WorldToScreenPosition(this.targetPosition.Value), 15, color, depth);
            }

            Vector2 screenPos1 = camera.WorldToScreenPosition(this.Position);
            Vector2 screenPos2 = camera.WorldToScreenPosition(this.targetPosition.Value);

            if (Vector2.Distance(screenPos1, screenPos2) > 30)
            {
                Vector2 point1 = Utils.PhysicsUtils.MoveTowardBounded(screenPos1, screenPos2, 15);
                Vector2 point2 = Utils.PhysicsUtils.MoveTowardBounded(screenPos2, screenPos1, 15);
                graphics.DrawLine(point1, point2, color, depth);
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

        public float TimeUntilMaterielEmpty()
        {
            float moveCost = this.MoveCostToTarget();
            if (moveCost >= this.Materiel)
            {
                return this.Range() / this.MaxSpeed;
            }
            else
            {
                return float.PositiveInfinity;
            }
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
            return Math.Sign(other.DistanceToTarget() - this.DistanceToTarget());
        }
    }
}

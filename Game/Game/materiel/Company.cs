using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects;
using MyGame.Server;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.materiel
{
    public class Company : GameObject, IPlayerControlled
    {
        private Comparison<Vector2> Vector2ClockwiseComparison;

        private GameObjectReferenceField<PlayerGameObject> controllingPlayer;
        private GameObjectReferenceListField<CombatVehicle> combatVehicles;
        private GameObjectReferenceListField<VehiclePosition> fightingPositions;
        private GameObjectReferenceField<Base> supplyPoint;

        public void AssignFightingPosition(CombatVehicle vic)
        {
            if (this.fightingPositions.Value.Count == 0)
            {
                return;
            }

            VehiclePosition bestPosition = null;
            float timeInBest = 0;
            float timeBestAbandoned = float.PositiveInfinity;

            foreach (VehiclePosition position in this.fightingPositions.Value)
            {
                if (position != null)
                {
                    float timeInPos = position.TimeUntilAbandoned(vic);
                    if (position.LastVehicle == null)
                    {
                        if ((timeBestAbandoned != 0 && timeInPos > 0) || timeInPos > timeInBest)
                        {
                            bestPosition = position;
                            timeInBest = timeInPos;
                            timeBestAbandoned = 0;
                        }
                    }
                    else if (timeBestAbandoned != 0)
                    {
                        float currentTime = position.TimeUntilAbandoned();
                        if (timeBestAbandoned > currentTime && timeInPos > currentTime)
                        {
                            bestPosition = position;
                            timeInBest = timeInPos;
                            timeBestAbandoned = currentTime;
                        }
                    }
                }
            }

            if (bestPosition != null)
            {
                bestPosition.Add(vic);
            }
        }


        public Company(GameObjectCollection collection)
            : base(collection)
        {
            Vector2ClockwiseComparison = new Comparison<Vector2>(this.less);
          
            controllingPlayer = new GameObjectReferenceField<PlayerGameObject>(this);
            combatVehicles = new GameObjectReferenceListField<CombatVehicle>(this);
            supplyPoint = new GameObjectReferenceField<Base>(this);
            fightingPositions = new GameObjectReferenceListField<VehiclePosition>(this);
        }

        public static void ServerInitialize(Company obj, PlayerGameObject controllingPlayer)
        {
            obj.controllingPlayer.Value = controllingPlayer;
        }

        public static Company Factory(ServerGame game, PlayerGameObject controllingPlayer)
        {
            Company obj = new Company(game.GameObjectCollection);
            game.GameObjectCollection.Add(obj);
            Company.ServerInitialize(obj, controllingPlayer);
            return obj;
        }

        public void AddVehicle(CombatVehicle vic)
        {
            if (vic.Company != null)
            {
                vic.Company.RemoveVehicle(vic);
            }
            vic.Company = this;
            combatVehicles.Value.Add(vic);
            this.AssignFightingPosition(vic);
        }

        public void RemoveVehicle(Vehicle vic)
        {
            if (vic is CombatVehicle)
            {
                CombatVehicle combatVehicle = (CombatVehicle)vic;
                if (combatVehicle.VehicleFightingPosition != null)
                {
                    combatVehicle.VehicleFightingPosition.Remove(combatVehicle);
                }
                combatVehicles.RemoveAllReferences(combatVehicle);
            }
        }

        public void SetPositions(GameObjectCollection collection, List<Vector2> positions)
        {
            Vector2 center = new Vector2(0);
            if (this.ResupplyPoint == null)
            {
                foreach (Vector2 pos in positions)
                {
                    center = center + pos;
                }
                center = center / (float)positions.Count;
            }
            else
            {
                center = this.ResupplyPoint.Position;
            }

            positions.Sort(Vector2ClockwiseComparison);

            if (positions.Count > 1)
            {
                int largest = 0;
                Vector2 v1 = positions[0] - center;
                Vector2 v2 = positions[1] - center;
                float cos = Vector2.Dot(v1, v2) / (v1.Length() * v2.Length());
                float distance = Vector2.Distance(positions[0], positions[1]);

                for (int i = 1; i < positions.Count; i++)
                {
                    v1 = positions[i] - center;
                    v2 = positions[(i + 1) % positions.Count] - center;
                    float newCos = Vector2.Dot(v1, v2) / (v1.Length() * v2.Length());
                    float newDistance = Vector2.Distance(positions[i], positions[(i + 1) % positions.Count]);

                    if (newCos < cos || (newCos == cos && newDistance > distance))
                    {
                        largest = i;
                        cos = newCos;
                        distance = newDistance;
                    }
                }

                for (; largest < positions.Count-1; largest++)
                {
                    Vector2 pos = positions[positions.Count - 1];
                    positions.RemoveAt(positions.Count - 1);
                    positions.Insert(0, pos);
                }
            }

            List<GameObjectReference<VehiclePosition>> oldPositions = new List<GameObjectReference<VehiclePosition>>(this.fightingPositions.Value.ToArray());
            this.fightingPositions.Value.Clear();

            foreach (Vector2 pos in positions)
            {
                this.fightingPositions.Value.Add(VehiclePosition.Factory(collection, this, pos));
            }

            for (int i = 0; i < oldPositions.Count; i++)
            {
                if(this.fightingPositions.Value.Count > i)
                {
                    this.fightingPositions.Value[i].Dereference().Add(oldPositions[i]);
                }
                oldPositions[i].Dereference().Destroy();
            }
        }

        public string GetHudText()
        {
            return this.ID.ToString() + " AR CO - " + this.combatVehicles.Value.Count.ToString();// +"/" + this.transports.Value.Count.ToString();
        }

        public void DrawScreen(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics, Camera camera, Color color, float depth)
        {
            
            foreach (Vehicle vic in this.combatVehicles.Value)
            {
                vic.DrawScreen(gameTime, graphics, camera, color, depth);
            }

            VehiclePosition last = null;
            foreach (VehiclePosition pos in this.fightingPositions.Value)
            {
                if (pos != null)
                {
                    pos.DrawScreen(gameTime, graphics, camera, color, depth);
                    if (last != null)
                    {
                        Vector2 screenPos1 = camera.WorldToScreenPosition(pos.Position);
                        Vector2 screenPos2 = camera.WorldToScreenPosition(last.Position);

                        Vector2 point1 = Utils.PhysicsUtils.MoveTowardBounded(screenPos1, screenPos2, 15);
                        Vector2 point2 = Utils.PhysicsUtils.MoveTowardBounded(screenPos2, screenPos1, 15);

                        if (Vector2.Distance(screenPos1, screenPos2) > 30)
                        {
                            graphics.DrawLine(point1, point2, color, depth);
                        }
                    }
                    last = pos;
                }
            }

            if (supplyPoint.Value != null)
            {
                Vector2 point = camera.WorldToScreenPosition(supplyPoint.Value.Position);
                graphics.DrawCircle(point, 15, color, depth);
            }
        }

        public GameObjectReference<PlayerGameObject> ControllingPlayer
        {
            get
            {
                return controllingPlayer.Value;
            }
        }

        public override void Destroy()
        {
            this.controllingPlayer.Value.RemoveCompany(this);
            foreach (Vehicle vic in this.combatVehicles.Value.ToArray())
            {
                vic.Company = null;
            }

            foreach (VehiclePosition pos in this.fightingPositions.Value.ToArray())
            {
                pos.Destroy();
            }

            base.Destroy();
        }

        public Base ResupplyPoint
        {
            get
            {
                return this.supplyPoint.Value;
            }

            set
            {
                this.supplyPoint.Value = value;
            }
        }

        private Vector2 AveragePosition
        {
            get
            {
                Vector2 center = new Vector2(0);
                float count = 0;
                foreach (VehiclePosition position in this.fightingPositions.Value)
                {
                    if (position != null)
                    {
                        center = center + position.Position;
                        count = count + 1;
                    }
                }
                return center / count;
            }
        }

        private int less(Vector2 a, Vector2 b)
        {
            Vector2 center;
            if (this.ResupplyPoint == null)
            {
                center = this.AveragePosition;
            }
            else
            {
                center = this.ResupplyPoint.Position;
            }

            if (a.X - center.X >= 0 && b.X - center.X < 0)
                return 1;
            if (a.X - center.X < 0 && b.X - center.X >= 0)
                return -1;
            if (a.X - center.X == 0 && b.X - center.X == 0)
            {
                if (a.Y - center.Y >= 0 || b.Y - center.Y >= 0)
                    return Math.Sign(a.Y - b.Y);
                return Math.Sign(b.Y - a.Y);
            }

            // compute the cross product of vectors (center -> a) x (center -> b)
            float det = (a.X - center.X) * (b.Y - center.Y) - (b.X - center.X) * (a.Y - center.Y);
            if (det < 0)
                return 1;
            if (det > 0)
                return -1;

            // points a and b are on the same line from the center
            // check which point is closer to the center
            float d1 = (a.X - center.X) * (a.X - center.X) + (a.Y - center.Y) * (a.Y - center.Y);
            float d2 = (b.X - center.X) * (b.X - center.X) + (b.Y - center.Y) * (b.Y - center.Y);
            return Math.Sign(d1 - d2);
        }
    }
}


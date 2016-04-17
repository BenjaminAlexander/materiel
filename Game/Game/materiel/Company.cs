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
        private Vector2ListMember fightingPositions;
        private GameObjectReferenceField<Base> supplyPoint;

        private Dictionary<int, CombatVehicle> lastVic = new Dictionary<int, CombatVehicle>();

        public int NextFightingPosition(CombatVehicle vic)
        {
            if (this.FightingPositions.Count == 0)
            {
                return -1;
            }

            int bestPosition = -1;
            float timeInBest = 0;
            float timeBestAbandoned = float.PositiveInfinity;

            for (int i = 0; i < this.FightingPositions.Count; i++)
            {
                if (!lastVic.ContainsKey(i))
                {
                    float timeInPos = vic.TimeUntilFightingPositionAbondoned(i);
                    if ((timeBestAbandoned != 0 && timeInPos > 0) || timeInPos > timeInBest)
                    {
                        bestPosition = i;
                        timeInBest = timeInPos;
                        timeBestAbandoned = 0;
                    }
                }
                else if (timeBestAbandoned != 0)
                {
                    float currentTime = lastVic[i].TimeUntilFightingPositionAbondoned();
                    float timeInPos = vic.TimeUntilFightingPositionAbondoned(i);
                    if (timeBestAbandoned > currentTime && timeInPos > currentTime)
                    {
                        bestPosition = i;
                        timeInBest = timeInPos;
                        timeBestAbandoned = currentTime;
                    }
                }
            }

            return bestPosition;
        }


        public Company(GameObjectCollection collection)
            : base(collection)
        {
            Vector2ClockwiseComparison = new Comparison<Vector2>(this.less);
          
            controllingPlayer = new GameObjectReferenceField<PlayerGameObject>(this);
            combatVehicles = new GameObjectReferenceListField<CombatVehicle>(this);
            supplyPoint = new GameObjectReferenceField<Base>(this);
            fightingPositions = new Vector2ListMember(this);
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
            if (vic.Company.Dereference() != null)
            {
                vic.Company.Dereference().RemoveVehicle(vic);
            }
            vic.Company = this;
            combatVehicles.Value.Add(vic);
            vic.TargetFightingPosition = this.NextFightingPosition(vic);
        }

        public void RemoveVehicle(Vehicle vic)
        {
            if (vic is CombatVehicle)
            {
                combatVehicles.RemoveAllReferences((CombatVehicle)vic);
            }
        }

        public void SetPositions(List<Vector2> positions)
        {
            Vector2 center = new Vector2(0);
            if (this.ResupplyPoint == null)
            {
                foreach (Vector2 pos in this.FightingPositions)
                {
                    center = center + pos;
                }
                center = center / (float)this.FightingPositions.Count;
            }
            else
            {
                center = this.ResupplyPoint.Position;
            }

            this.fightingPositions.Value = positions;
            this.fightingPositions.Value.Sort(Vector2ClockwiseComparison);

            if (this.fightingPositions.Value.Count > 1)
            {
                int largest = 0;
                Vector2 v1 = this.FightingPositions[0] - center;
                Vector2 v2 = this.FightingPositions[1] - center;
                float cos = Vector2.Dot(v1, v2) / (v1.Length() * v2.Length());
                float distance = Vector2.Distance(this.FightingPositions[0], this.FightingPositions[1]);

                for (int i = 1; i < this.fightingPositions.Value.Count; i++)
                {
                    v1 = this.FightingPositions[i] - center;
                    v2 = this.FightingPositions[(i + 1) % this.FightingPositions.Count] - center;
                    float newCos = Vector2.Dot(v1, v2) / (v1.Length() * v2.Length());
                    float newDistance = Vector2.Distance(this.FightingPositions[i], this.FightingPositions[(i + 1) % this.FightingPositions.Count]);

                    if (newCos < cos || (newCos == cos && newDistance > distance))
                    {
                        largest = i;
                        cos = newCos;
                        distance = newDistance;
                    }
                }

                for (; largest < this.FightingPositions.Count-1; largest++)
                {
                    Vector2 pos = this.fightingPositions.Value[this.FightingPositions.Count - 1];
                    this.fightingPositions.Value.RemoveAt(this.FightingPositions.Count - 1);
                    this.fightingPositions.Value.Insert(0, pos);
                }
            }
        }
        
        public void RemoveFromLastVic(CombatVehicle vic)
        {
            if (lastVic.ContainsKey(vic.TargetFightingPosition) && lastVic[vic.TargetFightingPosition] != null && lastVic[vic.TargetFightingPosition] == vic)
            {
                lastVic.Remove(vic.TargetFightingPosition);
            }
        }

        public void AddToLastVic(CombatVehicle vic)
        {
            if (vic.TargetFightingPosition != -1 && (
                !lastVic.ContainsKey(vic.TargetFightingPosition) ||
                lastVic[vic.TargetFightingPosition] == null || 
                lastVic[vic.TargetFightingPosition] == vic || 
                lastVic[vic.TargetFightingPosition].TimeUntilFightingPositionAbondoned() < vic.TimeUntilFightingPositionAbondoned()))
            {
                lastVic[vic.TargetFightingPosition] = vic;
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

            if (this.FightingPositions.Count > 0)
            {
                Vector2 screenPos = camera.WorldToScreenPosition(this.FightingPositions[0]);
                graphics.DrawCircle(screenPos, 15, color, depth);
            }

            for (int i =1; i < this.FightingPositions.Count; i++)
            {
                Vector2 screenPos1 = camera.WorldToScreenPosition(this.FightingPositions[i - 1]);
                Vector2 screenPos2 = camera.WorldToScreenPosition(this.FightingPositions[i]);

                Vector2 point1 = Utils.PhysicsUtils.MoveTowardBounded(screenPos1, screenPos2, 15);
                Vector2 point2 = Utils.PhysicsUtils.MoveTowardBounded(screenPos2, screenPos1, 15);

                graphics.DrawCircle(screenPos2, 15, color, depth);
                if (Vector2.Distance(screenPos1, screenPos2) > 30)
                {
                    graphics.DrawLine(point1, point2, color, depth);
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

        public List<Vector2> FightingPositions
        {
            get
            {
                return this.fightingPositions.Value;
            }
        }

        public void OccupyFightingPosition(CombatVehicle vic)
        {
            foreach (CombatVehicle otherVic in combatVehicles.Value)
            {
                if (otherVic != vic && 
                    otherVic.TargetFightingPosition == vic.TargetFightingPosition
                    && otherVic.TargetPosition == otherVic.Position)
                {
                    if (otherVic.TimeUntilFightingPositionAbondoned() > vic.TimeUntilFightingPositionAbondoned())
                    {
                        vic.TargetFightingPosition = this.NextFightingPosition(vic);
                        vic = otherVic;
                    }
                    else
                    {
                        otherVic.TargetFightingPosition = this.NextFightingPosition(otherVic);
                    }
                }
            }
        }

        private int less(Vector2 a, Vector2 b)
        {
            Vector2 center = new Vector2(0);
            if (this.ResupplyPoint == null)
            {
                foreach (Vector2 pos in this.FightingPositions)
                {
                    center = center + pos;
                }
                center = center / (float)this.FightingPositions.Count;
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


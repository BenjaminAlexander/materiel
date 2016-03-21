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
        private GameObjectReferenceField<PlayerGameObject> controllingPlayer;
        private GameObjectReferenceListField<CombatVehicle> combatVehicles;
        private GameObjectReferenceListField<Transport> transports;
        private Vector2GameObjectMember position1;
        private Vector2GameObjectMember position2;
        private GameObjectReferenceField<Base> supplyPoint;

        public Company(GameObjectCollection collection)
            : base(collection)
        {
            controllingPlayer = new GameObjectReferenceField<PlayerGameObject>(this);
            combatVehicles = new GameObjectReferenceListField<CombatVehicle>(this);
            transports = new GameObjectReferenceListField<Transport>(this);
            supplyPoint = new GameObjectReferenceField<Base>(this);
            position1 = new Vector2GameObjectMember(this, new Vector2(0));
            position2 = new Vector2GameObjectMember(this, new Vector2(0));
        }

        public static void ServerInitialize(Company obj, PlayerGameObject controllingPlayer)
        {
            obj.controllingPlayer.Value = controllingPlayer;
        }

        public static Company Factory(ServerGame game, PlayerGameObject controllingPlayer)
        {
            Company obj = new Company(game.GameObjectCollection);
            Company.ServerInitialize(obj, controllingPlayer);
            game.GameObjectCollection.Add(obj);
            return obj;
        }

        public void AddVehicle(Vehicle vic)
        {
            if (vic.Company.Dereference() != null)
            {
                vic.Company.Dereference().RemoveVehicle(vic);
            }
            vic.Company = this;
            if (vic is Transport)
            {
                this.transports.Value.Add((Transport)vic);
            }
            else
            {
                combatVehicles.Value.Add((CombatVehicle)vic);

                if (combatVehicles.Value.Count == 1)
                {
                    this.Move(vic.Position, vic.Position);
                }
                else
                {
                    this.Move(position1.Value, position2.Value);
                }
            }
        }

        public void RemoveVehicle(Vehicle vic)
        {
            if (vic is Transport)
            {
                this.transports.RemoveAllReferences((Transport)vic);
            }
            else
            {
                combatVehicles.RemoveAllReferences((CombatVehicle)vic);
                this.Move(position1.Value, position2.Value);
            }
        }

        public void Move(Vector2 position1, Vector2 position2)
        {
            float distance1 = Vector2.Distance(position1, this.position1.Value) + Vector2.Distance(position2, this.position2.Value);
            float distance2 = Vector2.Distance(position1, this.position2.Value) + Vector2.Distance(position2, this.position1.Value);

            float angle1 = Utils.Vector2Utils.Vector2Angle(this.position1.Value - this.position2.Value);
            float angle2 = Utils.Vector2Utils.Vector2Angle(position1 - position2);
            float angle3 = Utils.Vector2Utils.Vector2Angle(position2 - position1);

            float angleDistance1 = Utils.Vector2Utils.ShortestAngleDistance(angle1, angle2);
            float angleDistance2 = Utils.Vector2Utils.ShortestAngleDistance(angle1, angle3);

            if (angleDistance2 < angleDistance1)
            {
                Vector2 swap = position1;
                position1 = position2;
                position2 = swap;
            }

            if (this.combatVehicles.Value.Count == 1)
            {
                CombatVehicle vic = this.combatVehicles.Value[0];
                vic.TargetPosition = Vector2.Lerp(position1, position2, 0.5f);
            }
            else
            {
                float count = 0;
                foreach (CombatVehicle vic in this.combatVehicles.Value)
                {
                    vic.TargetPosition = Vector2.Lerp(position1, position2, count / ((float)this.combatVehicles.Value.Count - 1f));
                    count++;
                }
            }

            this.position1.Value = position1;
            this.position2.Value = position2;
        }

        public string GetHudText()
        {
            return this.ID.ToString() + " AR CO - " + this.combatVehicles.Value.Count.ToString() + "/" + this.transports.Value.Count.ToString();
        }

        public void DrawScreen(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics, Camera camera, Color color, float depth)
        {
            Vehicle last = null;
            foreach (Vehicle vic in this.combatVehicles.Value)
            {
                vic.DrawScreen(gameTime, graphics, camera, color, depth);
                if(last != null)
                {
                    Vector2 screenPos1 = camera.WorldToScreenPosition(vic.Position);
                    Vector2 screenPos2 = camera.WorldToScreenPosition(last.Position);

                    Vector2 point1 = Utils.PhysicsUtils.MoveTowardBounded(screenPos1, screenPos2, 15);
                    Vector2 point2 = Utils.PhysicsUtils.MoveTowardBounded(screenPos2, screenPos1, 15);
                    if (Vector2.Distance(screenPos1, screenPos2) > 30)
                    {
                        graphics.DrawLine(point1, point2, color, depth);
                    }
                }
                last = vic;
            }

            if (supplyPoint.Value != null && this.combatVehicles.Value.Count != 0)
            {
                Vector2 point1 = camera.WorldToScreenPosition(Vector2.Lerp(this.position1.Value, this.position2.Value, 0.5f));
                Vector2 point2 = camera.WorldToScreenPosition(supplyPoint.Value.Position);
                graphics.DrawLine(point1, point2, color, depth);
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

            foreach (Transport vic in this.transports.Value.ToArray())
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

        public Vehicle NextResupply()
        {
            List<GameObjectReference<CombatVehicle>> vicRefList = new List<GameObjectReference<CombatVehicle>>(combatVehicles.Value);
            if(vicRefList.Count == 0)
            {
                return null;
            }
            Vehicle rtn = null;
            foreach (Vehicle vic in vicRefList)
            {
                bool transportEnRoute = false;
                foreach (Transport transport in transports.Value)
                {
                    if (transport.ResupplyTarget == vic)
                    {
                        transportEnRoute = true;
                        break;
                    }
                }

                if (!transportEnRoute)
                {
                    if(rtn == null || vic.Materiel < rtn.Materiel)
                    {
                        rtn = vic;
                    }
                }
            }
            return rtn;
        }
    }
}

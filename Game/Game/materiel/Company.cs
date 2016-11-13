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
        private GameObjectReferenceListField<Transport> transportVehicles;
        private GameObjectReferenceField<Base> supplyPoint;

        public Company(GameObjectCollection collection)
            : base(collection)
        {          
            controllingPlayer = new GameObjectReferenceField<PlayerGameObject>(this);
            combatVehicles = new GameObjectReferenceListField<CombatVehicle>(this);
            transportVehicles = new GameObjectReferenceListField<Transport>(this);
            supplyPoint = new GameObjectReferenceField<Base>(this);
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
        }

        public void AddTransportVehicle(Transport vic)
        {
            if (vic.Company != null)
            {
                vic.Company.RemoveVehicle(vic);
            }
            vic.Company = this;
            transportVehicles.Value.Add(vic);
        }

        public void RemoveVehicle(Vehicle vic)
        {
            if (vic is CombatVehicle)
            {
                CombatVehicle combatVehicle = (CombatVehicle)vic;
                combatVehicles.RemoveAllReferences(combatVehicle);
            }
            else if (vic is Transport)
            {
                Transport transportVehicle = (Transport)vic;
                transportVehicles.RemoveAllReferences(transportVehicle);
            }
        }

        public void SetPositions(GameObjectCollection collection, List<Vector2> positions)
        {
            for (int i = 0; i < positions.Count && i < combatVehicles.Value.Count; i++)
            {
                try
                {
                    combatVehicles.Value[i].Dereference().TargetPosition = positions[i];
                }
                catch (Exception)
                {
                }
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

            foreach (Vehicle vic in this.transportVehicles.Value)
            {
                vic.DrawScreen(gameTime, graphics, camera, color, depth);
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

        public CombatVehicle NextVehicleToResupply()
        {
            try
            {
                List<CombatVehicle> combatVics = this.combatVehicles.DereferenceAll();
                CombatVehicle bestVic = null;

                foreach (CombatVehicle vic in combatVics)
                {
                    if(bestVic == null || bestVic.Materiel > vic.Materiel)
                    {
                        bestVic = vic;
                    }
                }
                return bestVic;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}


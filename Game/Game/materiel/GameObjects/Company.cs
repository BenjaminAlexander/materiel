using System;
using System.Collections.Generic;
using MyGame.GameStateObjects;
using MyGame.Server;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.DrawingUtils;

namespace MyGame.materiel.GameObjects
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
                catch (FailedDereferenceException)
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

            foreach (GameObjectReference<CombatVehicle> vicRef in this.combatVehicles.Value)
            {
                if (vicRef.CanDereference)
                {
                    vicRef.Dereference().DrawScreen(gameTime, graphics, camera, color, depth);
                }
            }

            foreach (GameObjectReference<Transport> vicRef in this.transportVehicles.Value)
            {
                if (vicRef.CanDereference)
                {
                    vicRef.Dereference().DrawScreen(gameTime, graphics, camera, color, depth);
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

        public CombatVehicle NextVehicleToResupply()
        {
            try
            {
                List<CombatVehicle> combatVics = this.combatVehicles.DereferenceAllPossible();
                List<Transport> transportVics = this.transportVehicles.DereferenceAllPossible();
                Dictionary<CombatVehicle, float> combatVicsMateriel = new Dictionary<CombatVehicle, float>();

                CombatVehicle bestVic = null;

                foreach (CombatVehicle vic in combatVics)
                {
                    combatVicsMateriel[vic] = vic.Materiel;
                }

                foreach (Transport vic in transportVics)
                {
                    if(vic.VicToResupply != null && combatVicsMateriel.ContainsKey(vic.VicToResupply))
                    {
                        combatVicsMateriel[vic.VicToResupply] = combatVicsMateriel[vic.VicToResupply] + vic.EstimatedVehicleMaterielDelivery();
                    }
                }

                foreach (CombatVehicle vic in combatVics)
                {
                    if (combatVicsMateriel[vic] < vic.MaxMateriel && (bestVic == null || combatVicsMateriel[bestVic] > combatVicsMateriel[vic]))
                    {
                        bestVic = vic;
                    }
                }

                return bestVic;
            }
            catch (FailedDereferenceException)
            {
                return null;
            }
        }
    }
}


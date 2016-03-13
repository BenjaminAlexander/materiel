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
    public class Company : GameObject
    {
        private GameObjectReferenceField<PlayerGameObject> controllingPlayer;
        private GameObjectReferenceListField<Vehicle> vehicles;

        public Company(GameObjectCollection collection)
            : base(collection)
        {
            controllingPlayer = new GameObjectReferenceField<PlayerGameObject>(this);
            vehicles = new GameObjectReferenceListField<Vehicle>(this);
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
            vic.SetCompany(this);
            vehicles.Value.Add(vic);
        }

        public void RemoveVehicle(Vehicle vic)
        {
            vehicles.RemoveAllReferences(vic);
        }

        public void Move(Vector2 position)
        {
            int count = 0;
            foreach (Vehicle vic in this.vehicles.Value)
            {
                vic.TargetPosition = position + new Vector2(count * 50, 0);
                count++;
            }
        }

        public string GetHudText()
        {
            return this.ID.ToString() + " AR CO - " + this.vehicles.Value.Count.ToString();
        }
    }
}

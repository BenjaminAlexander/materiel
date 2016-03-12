using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects;
using MyGame.GameServer;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;

namespace MyGame.materiel
{
    public class Company : GameObject
    {
        private GameObjectReferenceField<PlayerGameObject> controllingPlayer;
        private GameObjectReferenceListField<Vehicle> vehicles;

        public Company(Game1 game)
            : base(game)
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
            Company obj = new Company(game);
            Company.ServerInitialize(obj, controllingPlayer);
            game.GameObjectCollection.Add(obj);
            return obj;
        }

        public void AddVehicle(Vehicle vic)
        {
            vehicles.Value.Add(vic);
        }
        
        public string GetHudText()
        {
            return this.ID.ToString() + " AR CO - " + this.vehicles.Value.Count.ToString();
        }
    }
}

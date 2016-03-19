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

        public void Move(Vector2 position1, Vector2 position2)
        {
            if (this.vehicles.Value.Count == 1)
            {
                Vehicle vic = this.vehicles.Value[0];
                vic.TargetPosition = Vector2.Lerp(position1, position2, 0.5f);
            }
            else
            {
                float count = 0;
                foreach (Vehicle vic in this.vehicles.Value)
                {
                    vic.TargetPosition = Vector2.Lerp(position1, position2, count / ((float)this.vehicles.Value.Count - 1f));
                    count++;
                }
            }
        }

        public string GetHudText()
        {
            return this.ID.ToString() + " AR CO - " + this.vehicles.Value.Count.ToString();
        }

        public void DrawScreen(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics, Camera camera, Color color, float depth)
        {
            Vehicle last = null;
            foreach (Vehicle vic in this.vehicles.Value)
            {
                graphics.DrawCircle(camera.WorldToScreenPosition(vic.Position), 15, color, depth);
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
        }

        public GameObjectReference<PlayerGameObject> ControllingPlayer
        {
            get
            {
                return controllingPlayer.Value;
            }
        }
    }
}

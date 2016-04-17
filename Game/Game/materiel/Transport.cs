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
    public class Transport : Vehicle
    {

        public Transport(GameObjectCollection collection)
            : base(collection)
        {
        }

        public static void ServerInitialize(Transport vic, PlayerGameObject controllingPlayer, Vector2 position)
        {
            Vehicle.ServerInitialize(vic, controllingPlayer, position, 20);
        }

        public static Transport TransportFactory(GameObjectCollection collection, PlayerGameObject controllingPlayer, Vector2 position)
        {
            Transport vehicle = new Transport(collection);
            collection.Add(vehicle);
            Transport.ServerInitialize(vehicle, controllingPlayer, position);
            return vehicle;
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            base.Draw(gameTime, graphics);
            graphics.DrawDebugFont("T", this.Position + new Vector2(25, 15), 1f);
        }

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);
            

        }

        public Base ResupplyPoint()
        {
            if (this.Company.Dereference() != null)
            {
                return this.Company.Dereference().ResupplyPoint;
            }
            return null;
        }

        public float DistanceToResupplyPoint()
        {
            if (this.ResupplyPoint() != null)
            {
                return Vector2.Distance(this.Position, this.ResupplyPoint().Position);
            }
            return float.MaxValue;
        }

        public override void ResupplyComplete()
        {
            if (this.Company.Dereference() != null)
            {
            }
        }
    }
}

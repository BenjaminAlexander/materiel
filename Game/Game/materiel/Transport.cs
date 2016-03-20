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
    class Transport : Vehicle
    {
        public Transport(GameObjectCollection collection)
            : base(collection)
        {
        }

        public static void ServerInitialize(Transport vic, PlayerGameObject controllingPlayer, Vector2 position)
        {
            Vehicle.ServerInitialize(vic, controllingPlayer, position);
        }

        public static Transport TransportFactory(GameObjectCollection collection, PlayerGameObject controllingPlayer, Vector2 position)
        {
            Transport vehicle = new Transport(collection);
            Transport.ServerInitialize(vehicle, controllingPlayer, position);
            collection.Add(vehicle);
            return vehicle;
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            base.Draw(gameTime, graphics);
            graphics.DrawDebugFont("T", this.Position + new Vector2(25, 0), 1f);
        }
    }
}

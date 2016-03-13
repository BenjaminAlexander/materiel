using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.PhysicalObjects.MemberPhysicalObjects;
using MyGame.Server;
using MyGame.Client;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    public class SmallShip : Ship
    {
        public static void ServerInitialize(SmallShip smallShip, Vector2 position, Vector2 velocity)
        {
            Ship.ServerInitialize(smallShip, position, velocity, 0, 40, 100, 1800, 1f);
            Turret t3 = new Turret(smallShip.Game);
            Turret.ServerInitialize(t3, smallShip, new Vector2(25, 25) - TextureLoader.GetTexture("Enemy").CenterOfMass, (float)(0), (float)(Math.PI * 3));

            smallShip.Game.GameObjectCollection.Add(t3);
        }

        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Enemy"), Color.White, TextureLoader.GetTexture("Enemy").CenterOfMass, .9f);
        public override Collidable Collidable
        {
            get { return collidable; }
        }

        public SmallShip(Game1 game)
            : base(game)
        {
        }

    }
}
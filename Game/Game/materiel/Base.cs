﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects.PhysicalObjects;
using MyGame.DrawingUtils;
using Microsoft.Xna.Framework;
using MyGame.GameServer;

namespace MyGame.materiel
{
    class Base : CompositePhysicalObject
    {
        static Collidable collidable = new Collidable(TextureLoader.GetTexture("Star"), Color.Black, new Vector2(25), .1f);
        public override Collidable Collidable
        {
            get
            {
                return collidable;
            }
        }

        public static Base BaseFactory(ServerGame game, Vector2 position)
        {
            Base baseObj = new Base(game);
            Base.ServerInitialize(baseObj, position);
            game.GameObjectCollection.Add(baseObj);
            return baseObj;
        }

        public static void ServerInitialize(Base obj, Vector2 position)
        {
            CompositePhysicalObject.ServerInitialize(obj, position, 0);
        }

        public Base(Game1 game)
            : base(game)
        {
        }
    
        public override void MoveOutsideWorld(Microsoft.Xna.Framework.Vector2 position, Microsoft.Xna.Framework.Vector2 movePosition)
        {
 	        throw new NotImplementedException();
        }
    }
}

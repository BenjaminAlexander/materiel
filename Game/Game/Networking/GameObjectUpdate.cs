﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.GameClient;

namespace MyGame.Networking
{
    public class GameObjectUpdate : ClientUpdate
    {
        Type type;
        int id;
        public Type GameObjectType
        {
            get { return type; }
        }

        public int ID
        {
            get { return id; }
        }

        public GameObjectUpdate(GameTime currentGameTime, GameObject obj)
            : base(currentGameTime)
        {
            type = obj.GetType();
            id = obj.ID;
            this.Append(obj.TypeID);
            this.Append(obj.ID);
        }

        public GameObjectUpdate(byte[] b, int lenght)
            : base(b, lenght)
        {
            this.ResetReader();
            type = GameObjectTypes.GetType(this.ReadInt());
            id = this.ReadInt();
        }

        public override void Apply(ClientGame game, GameTime gameTime)
        {
            GameObjectCollection collection = game.GameObjectCollection;
            if (collection.Contains(this.id))
            {
                collection.Get(this.id).UpdateMemberFields(this, gameTime);
            }
            else
            {

                Type[] constuctorParamsTypes = new Type[0];
                //constuctorParamsTypes[0] = typeof(ClientGame);
                //constuctorParamsTypes[1] = typeof(GameObjectUpdate);

                System.Reflection.ConstructorInfo constructor = this.GameObjectType.GetConstructor(constuctorParamsTypes);
                if (constructor == null)
                {
                    throw new Exception("Game object must have constructor GameObject(int)");
                }
                object[] constuctorParams = new object[0];
                //constuctorParams[0] = game;
                //constuctorParams[1] = this;
                GameObject obj = (GameObject)constructor.Invoke(constuctorParams);
                obj.ClientInitialize(game, this);
                //obj.UpdateMemberFields(this);
                collection.Add(obj);

            }
        }
    }
}

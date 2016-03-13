using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.Client;
using System.Net.Sockets;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public class GameObjectUpdate : UdpMessage
    {
        public GameObjectUpdate(GameTime currentGameTime, GameObject obj)
            : base(currentGameTime)
        {
            int typeID = GameObjectTypes.GetTypeID(obj.GetType());
            this.Append(typeID);
            this.Append(obj.ID);
            this.Append(obj.IsDestroyed);

            foreach (GameObjectField field in obj.Fields)
            {
                field.ConstructMessage(this);
            }
        }

        public GameObjectUpdate(UdpTcpPair pair)
            : base(pair)
        {

        }

        public void Apply(ClientGame game, GameTime gameTime)
        {
            GameObjectField.SetModeSimulation();

            this.ResetReader();
            Type typeFromMessage = GameObjectTypes.GetType(this.ReadInt());
            int idFromMessage = this.ReadInt();
            bool isDesroyedFromMessage = this.ReadBoolean();

            GameObjectCollection collection = game.GameObjectCollection;

            GameObject obj;
            if (collection.Contains(idFromMessage))
            {
                obj = collection.Get<GameObject>(idFromMessage);
                if (obj.LastMessageTimeStamp > this.TimeStamp)
                {
                    return;
                }

                if (!(obj.GetType() == typeFromMessage && obj.ID == idFromMessage))
                {
                    throw new Exception("this message does not belong to this object");
                }

                obj.IsDestroyed = isDesroyedFromMessage;
                foreach (GameObjectField field in obj.Fields)
                {
                    field.ApplyMessage(this);
                }
                this.AssertMessageEnd();
            }
            else
            {
                if (isDesroyedFromMessage)
                {
                    return;
                }
                obj = GameObject.Construct(typeFromMessage, game.GameObjectCollection, idFromMessage);

                if (!(obj.GetType() == typeFromMessage && obj.ID == idFromMessage))
                {
                    throw new Exception("this message does not belong to this object");
                }

                obj.IsDestroyed = isDesroyedFromMessage;
                foreach (GameObjectField field in obj.Fields)
                {
                    field.ApplyMessage(this);
                }
                this.AssertMessageEnd();

                obj.OnClientInitialization(game);
            }

            obj.LatencyAdjustment(gameTime, this.TimeStamp);
            GameObjectField.SetModeDraw();
        }
    }
}

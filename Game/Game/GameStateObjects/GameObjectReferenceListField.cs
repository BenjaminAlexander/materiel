using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects
{
    class GameObjectReferenceListField<T> : GameObjectField where T : GameObject
    {
        private GameObjectCollection collection;
        private List<GameObjectReference<T>> value;

        public GameObjectReferenceListField(GameObject obj) : base(obj)
        {
            this.collection = obj.Collection;
            this.value = new List<GameObjectReference<T>>();
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            var rtn = new List<GameObjectReference<T>>();
            int count = message.ReadInt();
            for (int i = 0; i < count; i++)
            {
                GameObjectReference<T> rf = new GameObjectReference<T>(message, collection);
                rtn.Add(rf);
            }
            this.value = rtn;
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.value.Count);
            foreach (GameObjectReference<T> obj in this.value)
            {
                message = obj.ConstructMessage(message);
            }
            return message;
        }

        public List<GameObjectReference<T>> Value
        {
            get
            {
                return this.value;
            }
        }

        public void RemoveAllReferences(T obj)
        {
            int index = 0;
            while (index < value.Count)
            {
                if (value[index].ID == obj.ID)
                {
                    value.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }

        public void RemoveAllReferences(GameObjectReference<T> objRef)
        {
            int index = 0;
            while (index < value.Count)
            {
                if (value[index].ID == objRef.ID)
                {
                    value.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }

        public List<T> DereferenceAll()
        {
            List<T> rtn = new List<T>();
            foreach(GameObjectReference<T> reference in this.value)
            {
                rtn.Add(reference.Dereference());
            }
            return rtn;
        }
    }
}

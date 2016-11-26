using System;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects
{
    public struct GameObjectReference<T> where T : GameObject
    {
        private int id;
        private GameObjectCollection collection;
        private T obj;
        private Boolean hasDereferenced;

        private GameObjectReference(T obj)
        {
            this.obj = obj;
            this.hasDereferenced = true;
            this.collection = null;
            if (obj == null)
            {
                this.id = 0;
                
            }
            else
            {
                this.id = obj.ID;
            }
        }

        public GameObjectReference(GameObjectUpdate message, GameObjectCollection collection)
        {
            this.obj = null;
            this.hasDereferenced = false;
            this.id = message.ReadInt();
            this.collection = collection;
            if (id == 0)
            {
                hasDereferenced = true;
            }
            else
            {
                try
                {
                    Dereference();
                }
                catch (FailedDereferenceException)
                {
                }
            }
        }

        public T Dereference() 
        {
            if(hasDereferenced)
            {
                return obj;
            }

            if (this.collection.Contains(id))
            {
                obj = (T)this.collection.Get<T>(id);
                hasDereferenced = true;
                return obj;
            }
            else
            {
                throw new FailedDereferenceException();
            }
        }

        public GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.id);
            return message;
        }

        public int ID
        {
            get
            {
                return id;
            }
        }

        public static implicit operator T(GameObjectReference<T> reference)
        {
            return reference.Dereference();
        }

        public static implicit operator GameObjectReference<T>(T obj)
        {
            return new GameObjectReference<T>(obj);
        }
    }

    public class FailedDereferenceException : Exception
    {
    }
}

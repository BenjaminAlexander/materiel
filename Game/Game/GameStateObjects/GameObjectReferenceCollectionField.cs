using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects
{
    abstract class GameObjectReferenceCollectionField<T> : GameObjectField where T : GameObject
    {
        private GameObjectCollection collection;

        protected abstract void SetValue(List<GameObjectReference<T>> list);

        public GameObjectReferenceCollectionField(GameObject obj) : base(obj)
        {
            this.collection = obj.Collection;
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
            this.SetValue(rtn);
        }

        public GameObjectUpdate ConstructMessage(GameObjectUpdate message, List<GameObjectReference<T>> list)
        {
            message.Append(list.Count);
            foreach (GameObjectReference<T> obj in list)
            {
                message = obj.ConstructMessage(message);
            }
            return message;
        }
    }
}
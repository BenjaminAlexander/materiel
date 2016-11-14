using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects
{
    class GameObjectReferenceQueueField<T> : GameObjectReferenceCollectionField<T> where T : GameObject
    {
        private Queue<GameObjectReference<T>> value;

        public GameObjectReferenceQueueField(GameObject obj) : base(obj)
        {
            this.value = new Queue<GameObjectReference<T>>();
        }

        protected override void SetValue(List<GameObjectReference<T>> list)
        {
            this.value = new Queue<GameObjectReference<T>>(list);
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            return this.ConstructMessage(message, Enumerable.ToList<GameObjectReference<T>>(this.value));
        }

        public Queue<GameObjectReference<T>> Value
        {
            get
            {
                return this.value;
            }
        }
    }
}

using System.Collections.Generic;

namespace MyGame.GameStateObjects
{
    class GameObjectReferenceListField<T> : GameObjectReferenceCollectionField<T> where T : GameObject
    {
        private List<GameObjectReference<T>> value;

        public GameObjectReferenceListField(GameObject obj) : base(obj)
        {
            this.value = new List<GameObjectReference<T>>();
        }

        protected override void SetValue(List<GameObjectReference<T>> list)
        {
            this.value = list;
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            return this.ConstructMessage(message, this.value);
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

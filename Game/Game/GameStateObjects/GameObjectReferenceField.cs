using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects
{
    class GameObjectReferenceField<T> : GameObjectField where T : GameObject
    {
        private GameObjectCollection collection;
        private GameObjectReference<T> value;

        public GameObjectReferenceField(GameObject obj)
            : base(obj)
        {
            this.collection = obj.Collection;
            this.value = null;
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            this.value = new GameObjectReference<T>(message, this.collection);
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            return this.value.ConstructMessage(message);
        }

        public T Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
            }
        }
    }
}

namespace MyGame.GameStateObjects
{
    abstract class GenericGameObjectField<T> : GameObjectField
    {
        private T simulationValue;
        internal T previousValue;
        internal T drawValue;

        //TODO: is this method the best?
        private bool initialized = false;
        
        //TODO: make this clean
        public T SimulationValue
        {
            get
            {
                return simulationValue;
            }

            protected set
            {
                simulationValue = value;
                if (!initialized)
                {
                    previousValue = value;
                    drawValue = value;
                    initialized = true;
                }
            }
        }
            
        //TODO: if we don't actuall put the inital values we want in here, should the v argument be removed?
        public GenericGameObjectField(GameObject obj, T v) : base(obj)
        {
            simulationValue = v;
            drawValue = v;
            previousValue = v;
        }

        public static implicit operator T(GenericGameObjectField<T> m)
        {
            return m.Value;
        }
        
        public T Value
        {
            get 
            {
                if (GameObjectField.IsModeSimulation())
                {
                    return simulationValue;
                }
                else if (GameObjectField.IsModePrevious())
                {
                    return previousValue;
                }
                else
                {
                    return drawValue;
                }
            }
            set 
            {
                if (GameObjectField.IsModeSimulation())
                {
                    simulationValue = value;
                }
                else if (GameObjectField.IsModePrevious())
                {
                    previousValue = value;
                }
                else
                {
                    drawValue = value;
                }
            }
        }

        public T GetValue(Modes mode)
        {
            if (mode == Modes.Simulation)
            {
                return simulationValue;
            }
            else if (mode == Modes.Previous)
            {
                return previousValue;
            }
            else
            {
                return drawValue;
            }
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            this.previousValue = this.drawValue;
        }
    }
}

using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    class TreeVector2Member : GameObjectField
    {
        private Vector2 simulationValue;
        internal Vector2 previousValue;
        internal Vector2 drawValue;

        //TODO: is this method the best?
        private bool initialized = false;

        private PhysicalObject obj;
        private GameObjectCollection collection;
        
        //TODO: make this clean
        public Vector2 SimulationValue
        {
            get
            {
                return simulationValue;
            }
        }
            
        //TODO: if we don't actuall put the inital values we want in here, should the v argument be removed?
        public TreeVector2Member(PhysicalObject obj, Vector2 v)
            : base(obj)
        {
            simulationValue = v;
            drawValue = v;
            previousValue = v;

            this.obj = obj;
            this.collection = obj.Collection;
        }
        
        public Vector2 Value
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

        public Vector2 DrawValue
        {
            get
            {
                return this.drawValue;
            }
        }

        public Vector2 PreviousValue
        {
            get
            {
                return this.previousValue;
            }
        }

        public Vector2 GetValue(Modes mode)
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

        public override void Interpolate(float smoothing)
        {
            this.drawValue = Vector2.Lerp(this.SimulationValue, this.previousValue, smoothing);
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            this.previousValue = this.drawValue;
            this.simulationValue = message.ReadVector2();

            if (!initialized)
            {
                this.previousValue = this.simulationValue;
                this.drawValue = this.simulationValue;
                this.initialized = true;
            }
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.SimulationValue);
            return message;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.GameStateObjects
{
    class IntegerQueueGameObjectField : GenericGameObjectField<Queue<int>>
    {
        public IntegerQueueGameObjectField(GameObject obj)
            : base(obj, new Queue<int>())
        {
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            base.ApplyMessage(message);
            int count = message.ReadInt();
            this.SimulationValue.Clear();
            for (int i = 0; i < count; i++)
            {
                this.SimulationValue.Enqueue(message.ReadInt());
            }
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.SimulationValue.Count);
            foreach (int i in this.SimulationValue)
            {
                message.Append(this.SimulationValue.Count);
            }
            return message;
        }
    }
}

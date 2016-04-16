using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects
{
    class Vector2ListMember: NonInterpolatedGameObjectMember<List<Vector2>>
    {
        public Vector2ListMember(GameObject obj)
            : base(obj, new List<Vector2>())
        {
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            base.ApplyMessage(message);
            int count = message.ReadInt();

            this.SimulationValue = new List<Vector2>();
            for (int i = 0; i < count; i++)
            {
                this.SimulationValue.Add(message.ReadVector2());
            }
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.SimulationValue.Count);
            foreach (Vector2 pos in this.SimulationValue)
            {
                message.Append(pos);
            }
            return message;
        }
    }
}

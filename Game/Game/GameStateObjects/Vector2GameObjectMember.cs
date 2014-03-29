﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects
{
    class Vector2GameObjectMember : NonInterpolatedGameObjectMember<Vector2>
    {
        public Vector2GameObjectMember(GameObject obj, Vector2 v) : base(obj, v)
        {
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            this.simulationValue = message.ReadVector2();
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.simulationValue);
            return message;
        }
    }
}

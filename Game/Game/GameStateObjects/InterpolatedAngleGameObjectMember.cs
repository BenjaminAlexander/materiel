﻿namespace MyGame.GameStateObjects
{
    class InterpolatedAngleGameObjectMember : GenericGameObjectField<float>
    {
        public InterpolatedAngleGameObjectMember(GameObject obj, float v)
            : base(obj, v)
        {
        }

        public override void Interpolate(float smoothing)
        {
            this.drawValue = Utils.Vector2Utils.Lerp(this.SimulationValue, this.previousValue, smoothing);
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            base.ApplyMessage(message);
            this.SimulationValue = message.ReadFloat();
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.SimulationValue);
            return message;
        }
    }
}

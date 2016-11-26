namespace MyGame.GameStateObjects
{
    class BoolGameObjectMember : NonInterpolatedGameObjectMember<bool>
    {
        public BoolGameObjectMember(GameObject obj, bool v)
            : base(obj, v)
        {
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            base.ApplyMessage(message);
            this.SimulationValue = message.ReadBoolean();
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            message.Append(this.SimulationValue);
            return message;
        }
    }
}

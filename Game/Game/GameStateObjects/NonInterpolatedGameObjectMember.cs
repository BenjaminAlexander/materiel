namespace MyGame.GameStateObjects
{
    abstract class NonInterpolatedGameObjectMember<T> : GenericGameObjectField<T>
    {
        public NonInterpolatedGameObjectMember(GameObject obj, T v)
            : base(obj, v)
        {
        }

        public override void Interpolate(float smoothing) 
        {
            this.drawValue = this.SimulationValue;
        }
    }
}

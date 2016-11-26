namespace MyGame.IO.Events
{
    class LeftMouseReleased : IOEvent
    {
        public override bool hasOccured()
        {
            return IOState.leftButtonReleased();
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == this.GetType();
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}

namespace MyGame.IO.Events
{
    class LeftMouseDown : IOEvent
    {
        public override bool hasOccured()
        {
            return IOState.leftButtonDown();
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

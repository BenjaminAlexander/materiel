namespace MyGame.IO
{
    public abstract class IOEvent
    {
        public abstract bool hasOccured();

        public override bool Equals(object obj)
        {
            return base.Equals(obj) || obj.GetType() == this.GetType();
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}

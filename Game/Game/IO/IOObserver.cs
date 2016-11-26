namespace MyGame.IO
{
    public interface IOObserver
    {
        void UpdateWithIOEvent(IOEvent ioEvent);
    }
}

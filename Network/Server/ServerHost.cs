using Core;

namespace Server
{
    public class ServerHost
    {
        public Room room;
        public Listener listener = new Listener();

        public void Init(int searchPort, int listenPort)
        {
            room = new Room();
            listener.Init(searchPort, listenPort, room);
        }
    }
}
using Core;

namespace Client
{
    public class ClientHost
    {
        public MyClientSession session;
        public Connector connector = new Connector();

        public void Init(int searchPort)
        {
            session = new MyClientSession();
            connector.Init(searchPort, session);
        }
    }
}
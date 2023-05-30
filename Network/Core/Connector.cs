using System;
using System.Net.Sockets;
using System.Net;
using Client;

namespace Core
{
    public class Connector
    {
        Socket _searchSocket;
        Socket _connectSocket;

        byte[] _searchBuffer = new byte[64];
        MyClientSession _session;

        bool _isSearching = false;

        public void Init(int searchPort, MyClientSession session)
        {
            //Init Search Socket
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, searchPort);
            _searchSocket = new Socket(ep.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            _searchSocket.Bind(ep);

            //Init Search Async Event
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnSearchComplete);

            //Init Session
            _session = session;

            Search(args);
        }

        public void StopSearch()
        {
            if (_isSearching)
            {
                _searchSocket.Close();
                _isSearching = false;
            }
        }

        void Search(SocketAsyncEventArgs args)
        {
            args.SetBuffer(_searchBuffer, 0, _searchBuffer.Length);

            try
            {
                _isSearching = true;
                bool pending = _searchSocket.ReceiveFromAsync(args);

                if (!pending)
                    OnSearchComplete(null, args);
            }
            catch (Exception e)
            {
                StopSearch();
                //Need log
            }
        }

        void OnSearchComplete(Object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                try
                {
                    if ((PacketID)Deserializer.DeserializeUshort(new ArraySegment<byte>(_searchBuffer)) == PacketID.Search)
                    {
                        SearchPacket searchPacket = new SearchPacket();
                        searchPacket.Deserialize(new ArraySegment<byte>(_searchBuffer, 0, args.BytesTransferred));

                        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(searchPacket.ip), searchPacket.port);
                        Connect(ep);
                    }
                    else
                    {
                        Search(args);
                    }
                }
                catch (Exception e)
                {
                    //Need Log
                }
            }

            StopSearch();
        }

        void Connect(IPEndPoint ep)
        {
            //Init Connect Socket
            _connectSocket = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //Init Connect Async Event
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = ep;
            args.Completed += OnConnectComplete;

            try
            {
                bool pending = _connectSocket.ConnectAsync(args);

                if (!pending)
                    OnConnectComplete(null, args);
            }
            catch (Exception e)
            {
                //Need log
            }
        }

        void OnConnectComplete(Object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                args.ConnectSocket.LingerState = new LingerOption(true, 0);
                _session.Init(args.ConnectSocket);
                _session.MyConnectMethod(args.RemoteEndPoint);
            }
            else
            {
                //Need Log
            }
        }
    }
}
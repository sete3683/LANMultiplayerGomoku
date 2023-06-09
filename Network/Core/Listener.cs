﻿using Server;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Core
{
    public class Listener
    {
        Socket _searchSocket;
        Socket _listenSocket;

        byte[] _searchBuffer = new byte[32];
        ArraySegment<byte> _searchSegment;

        Thread _searchThread;
        Room _sessionRoom;

        bool _isSearching = false;
        bool _isListening = false;

        public void Init(int searchPort, int listenPort, Room room)
        {
            //Init Search Socket
            _searchSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _searchSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

            //Init Listen Socket
            IPEndPoint ep = new IPEndPoint(GetIPv4(), listenPort);
            _listenSocket = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.LingerState = new LingerOption(true, 0);
            _listenSocket.Bind(ep);
            _listenSocket.Listen(4);
            _isListening = true;

            //Init Search Buffer
            SearchPacket searchPacket = new SearchPacket() { ip = ep.Address.ToString(), port = (ushort)ep.Port };
            _searchSegment = searchPacket.Serialize(_searchBuffer);

            //Init Accept Async Event
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptComplete);

            //Init Search Thread
            _searchThread = new Thread(() => Search(searchPort));
            _searchThread.Start();

            //Init Session Room
            _sessionRoom = room;

            Accept(args);
        }
        
        IPAddress GetIPv4()
        {
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;
            }

            return null;
        }

        void Search(int searchPort)
        {
            _isSearching = true;
            
            IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, searchPort);

            while (_isSearching)
            {
                _searchSocket.SendTo(_searchSegment.Array, ep);
                Thread.Sleep(1500);
            }
        }

        public void StopSearch()
        {
            if (_isSearching)
            {
                _isSearching = false;
                _searchThread.Join();
            }
        }

        void Accept(SocketAsyncEventArgs args)
        {
            try
            {
                bool pending = _listenSocket.AcceptAsync(args);

                if (!pending)
                    OnAcceptComplete(null, args);
            }
            catch (Exception e)
            {
                StopListen();
                //Need log
            }
        }

        void OnAcceptComplete(object sender, SocketAsyncEventArgs args)
        {
            MyServerSession session = new MyServerSession();
            args.AcceptSocket.LingerState = new LingerOption(true, 0);
            session.Init(args.AcceptSocket);
            session.room = _sessionRoom;
            _sessionRoom.Enter(session);
            
            session.MyConnectMethod(args.AcceptSocket.RemoteEndPoint);

            args.AcceptSocket = null;
            Accept(args);
        }

        public void StopListen()
        {
            if (_isListening)
            {
                _isListening = false;
                _listenSocket.Close();
            }
        }
    }
}
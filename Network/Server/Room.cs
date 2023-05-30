using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Room
    {
        List<MyServerSession> _sessions = new List<MyServerSession>();
        ushort _sessionID = 0;

        object _lock = new object();

        public void Enter(MyServerSession session)
        {
            lock (_lock)
            {
                _sessions.Add(session);
                session.id = _sessionID++;
                session.room = this;
            }
        }

        public void Leave(MyServerSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session);
            }
        }

        public void Clear()
        {
            DisconnectAll();
            _sessions.Clear();
        }

        void DisconnectAll()
        {
            lock (_lock)
            {
                foreach (MyServerSession session in _sessions)
                    session.Disconnect();
            }
        }

        public void Broadcast(Packet packet)
        {
            lock (_lock)
            {
                foreach (MyServerSession session in _sessions)
                {
                    session.Send(packet.Serialize(session.SendBuffer));
                }
            }
        }

        public void Broadcast(ArraySegment<byte> segment)
        {
            lock (_lock)
            {
                foreach (MyServerSession session in _sessions)
                {
                    session.Send(segment);
                }
            }
        }

        public void Unicast(int id, Packet packet)
        {
            _sessions[id].Send(packet.Serialize(_sessions[id].SendBuffer));
        }
    }
}

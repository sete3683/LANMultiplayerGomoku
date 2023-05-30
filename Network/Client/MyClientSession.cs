using Core;
using Server;
using System;
using System.Net;
using UnityEngine;

namespace Client
{
    public class MyClientSession : Session
    {
        public bool isStarted = false;

        public override void MyConnectMethod(EndPoint ep)
        {
            ReadyPacket readyPacket = new ReadyPacket();
            Send(readyPacket.Serialize(_sendBuffer));
        }

        public override void MySendMethod() { }

        public override void MyRecvMethod(ArraySegment<byte> segment)
        {
            PacketID packetID = (PacketID)Deserializer.DeserializeID(segment);

            switch (packetID)
            {
                case PacketID.Ready:
                    GameManager.Instance.JobQueue.Enqueue(GameManager.Instance.OnReady);
                    break;

                case PacketID.Start:
                    if (!isStarted)
                    {
                        isStarted = true;
                        GameManager.Instance.JobQueue.Enqueue(GameManager.Instance.OnStart);
                        Send(segment);
                    }
                    break;

                case PacketID.Process:
                    ProcessPacket processPacket = new ProcessPacket();
                    processPacket.Deserialize(segment);
                    string[] rc = processPacket.data.Split(',');
                    (int r, int y) pos = (int.Parse(rc[0]), int.Parse(rc[1]));
                    GameManager.Instance.JobQueue.Enqueue(() => GameManager.Instance.OnProcess(pos));
                    break;

                case PacketID.End:
                    break;
            }
        }

        public override void MyDisconnectMethod(EndPoint ep)
        {
            GameManager.Instance.JobQueue.Enqueue(GameManager.Instance.OnDisconnect);
        }

        public void SendPacket(Packet packet)
        {
            Send(packet.Serialize(_sendBuffer));
        }
    }
}
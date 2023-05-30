using Core;
using System;
using System.Net;
using UnityEngine;

namespace Server
{
    public class MyServerSession : Session
    {
        public ushort id;
        public Room room;

        public bool isStarted = false;

        public override void MyConnectMethod(EndPoint ep)
        {
            NetworkManager.Instance.JobQueue.Enqueue(NetworkManager.Instance.StopSearchServer);
            NetworkManager.Instance.JobQueue.Enqueue(NetworkManager.Instance.StopListenServer);

            ReadyPacket readyPacket = new ReadyPacket();
            room.Broadcast(readyPacket);
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
                        room.Broadcast(segment);
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
    }
}
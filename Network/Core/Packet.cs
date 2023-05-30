using System;

namespace Core
{
    public enum PacketID : ushort
    {
        Search = 1,
        Ready,
        Start,
        Process,
        End
    }

    public enum EndID : ushort
    {
        Win = 1,
        Lose
    }

    public class SearchPacket : Packet
    {
        public string ip;
        public ushort port;

        public SearchPacket() : base(PacketID.Search) { }

        public ArraySegment<byte> Serialize(byte[] sendBuffer)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(sendBuffer);
            ushort cursor = 0;

            Serializer.SerializeUshort(segment, (ushort)packetID, ref cursor);
            Serializer.SerializeString(segment, ip, ref cursor);
            Serializer.SerializeUshort(segment, port, ref cursor);

            return new ArraySegment<byte>(segment.Array, segment.Offset, cursor);
        }

        public override ArraySegment<byte> Serialize(SendBuffer sendBuffer)
        {
            ArraySegment<byte> segment = sendBuffer.Open(128);
            ushort cursor = 0;

            Serializer.SerializeUshort(segment, (ushort)packetID, ref cursor);
            Serializer.SerializeString(segment, ip, ref cursor);
            Serializer.SerializeUshort(segment, port, ref cursor);

            return sendBuffer.Close(cursor);
        }

        public override void Deserialize(ArraySegment<byte> segment)
        {
            ushort cursor = sizeof(ushort);

            ushort ipLen = Deserializer.DeserializeUshort(segment, ref cursor);
            ip = Deserializer.DeserializeString(segment, ipLen, ref cursor);
            port = Deserializer.DeserializeUshort(segment, ref cursor);
        }
    }

    public class ReadyPacket : Packet
    {
        public ReadyPacket() : base(PacketID.Ready) { }

        public override ArraySegment<byte> Serialize(SendBuffer sendBuffer)
        {
            ArraySegment<byte> segment = sendBuffer.Open(32);
            ushort cursor = sizeof(ushort);

            Serializer.SerializeUshort(segment, (ushort)packetID, ref cursor);
            Serializer.SerializeUshort(segment, cursor);

            return sendBuffer.Close(cursor);
        }

        public override void Deserialize(ArraySegment<byte> segment) { }
    }

    public class StartPacket : Packet
    {
        public StartPacket() : base(PacketID.Start) { }

        public override ArraySegment<byte> Serialize(SendBuffer sendBuffer)
        {
            ArraySegment<byte> segment = sendBuffer.Open(32);
            ushort cursor = sizeof(ushort);

            Serializer.SerializeUshort(segment, (ushort)packetID, ref cursor);
            Serializer.SerializeUshort(segment, cursor);

            return sendBuffer.Close(cursor);
        }

        public override void Deserialize(ArraySegment<byte> segment) { }
    }

    public class ProcessPacket : Packet
    {
        public ushort userID;
        public string data;

        public ProcessPacket() : base(PacketID.Process) { }

        public override ArraySegment<byte> Serialize(SendBuffer sendBuffer)
        {
            ArraySegment<byte> segment = sendBuffer.Open(128);
            ushort cursor = sizeof(ushort);

            Serializer.SerializeUshort(segment, (ushort)packetID, ref cursor);
            Serializer.SerializeUshort(segment, userID, ref cursor);
            Serializer.SerializeString(segment, data, ref cursor);
            Serializer.SerializeUshort(segment, cursor);

            return sendBuffer.Close(cursor);
        }

        public override void Deserialize(ArraySegment<byte> segment)
        {
            ushort cursor = sizeof(ushort) * 2;

            userID = Deserializer.DeserializeUshort(segment, ref cursor);
            ushort dataLen = Deserializer.DeserializeUshort(segment, ref cursor);
            data = Deserializer.DeserializeString(segment, dataLen, ref cursor);
        }
    }

    public class EndPacket : Packet
    {
        public ushort userID;
        public EndID endID;

        public EndPacket() : base(PacketID.End) { }


        public override ArraySegment<byte> Serialize(SendBuffer sendBuffer)
        {
            ArraySegment<byte> segment = sendBuffer.Open(64);
            ushort cursor = sizeof(ushort);

            Serializer.SerializeUshort(segment, (ushort)packetID, ref cursor);
            Serializer.SerializeUshort(segment, userID, ref cursor);
            Serializer.SerializeUshort(segment, (ushort)endID, ref cursor);
            Serializer.SerializeUshort(segment, cursor);

            return sendBuffer.Close(cursor);
        }

        public override void Deserialize(ArraySegment<byte> segment)
        {
            ushort cursor = sizeof(ushort) * 2;

            userID = Deserializer.DeserializeUshort(segment, ref cursor);
            endID = (EndID)Deserializer.DeserializeUshort(segment, ref cursor);
        }
    }

    public abstract class Packet
    {
        public PacketID packetID;

        public Packet(PacketID packetID)
        {
            this.packetID = packetID;
        }

        public abstract ArraySegment<byte> Serialize(SendBuffer sendBuffer);
        public abstract void Deserialize(ArraySegment<byte> segment);
    }
}
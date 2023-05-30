using System;
using System.Text;

namespace Core
{
    public static class Serializer
    {
        public static void SerializeUshort(ArraySegment<byte> segment, ushort data, ref ushort cursor)
        {
            byte[] dataBytes = BitConverter.GetBytes(data);
            Array.Copy(dataBytes, 0, segment.Array, segment.Offset + cursor, sizeof(ushort));
            cursor += sizeof(ushort);
        }

        public static void SerializeString(ArraySegment<byte> segment, string data, ref ushort cursor)
        {
            ushort dataLen = (ushort)Encoding.UTF8.GetBytes(data, 0, data.Length, segment.Array, segment.Offset + cursor + sizeof(ushort));
            SerializeUshort(segment, dataLen, ref cursor);
            cursor += dataLen;
        }

        public static void SerializeUshort(ArraySegment<byte> segment, ushort data)
        {
            byte[] dataBytes = BitConverter.GetBytes(data);
            Array.Copy(dataBytes, 0, segment.Array, segment.Offset, sizeof(ushort));
        }
    }

    public static class Deserializer
    {
        public static ushort DeserializeUshort(ArraySegment<byte> segment, ref ushort cursor)
        {
            ushort data = BitConverter.ToUInt16(segment.Array, segment.Offset + cursor);
            cursor += sizeof(ushort);

            return data;
        }
        
        public static ushort DeserializeUshort(ArraySegment<byte> segment)
        {
            return BitConverter.ToUInt16(segment.Array, segment.Offset);
        }

        public static ushort DeserializeID(ArraySegment<byte> segment)
        {
            return BitConverter.ToUInt16(segment.Array, segment.Offset + sizeof(ushort));
        }

        public static string DeserializeString(ArraySegment<byte> segment, ushort dataLen, ref ushort cursor)
        {
            string data = Encoding.UTF8.GetString(segment.Array, segment.Offset + cursor, dataLen);
            cursor += dataLen;

            return data;
        }
    }
}
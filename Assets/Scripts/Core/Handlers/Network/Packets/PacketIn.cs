using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class PacketIn : BinaryReader
{
    private byte[] packetData;
    public Opcodes Opcode { get; set; }
    public ushort Size { get; set; }

    public PacketIn(byte[] data)
        : base(new MemoryStream(data))
    {
        ushort Size = (ushort)((this.ReadUInt16() / 0x100) - 4);
        Opcode = (Opcodes)this.ReadUInt16();
    }

    public PacketIn(byte[] data, int i)
        : base(new MemoryStream(data))
    {
        packetData = data;
    }

    public override string ReadString()
    {
        StringBuilder sb = new StringBuilder();
        while (true)
        {
            byte b;
            //if (Remaining > 0)
            b = ReadByte();
            //else
            //   b = 0;

            if (b == 0) break;
            sb.Append((char)b);
        }
        return sb.ToString();
    }

    public byte[] ReadRemaining()
    {
        MemoryStream ms = (MemoryStream)BaseStream;
        int Remaining = (int)(ms.Length - ms.Position);
        return ReadBytes(Remaining);
    }

    public int Remaining
    {
        get
        {
            MemoryStream ms = (MemoryStream)BaseStream;
            return (int)(ms.Length - ms.Position);
        }
        set
        {
            MemoryStream ms = (MemoryStream)BaseStream;
            if (value <= (ms.Length - ms.Position))
                ms.Position = value;
        }
    }
    public float ReadFloat()
    {
        return System.BitConverter.ToSingle(ReadBytes(4), 0);
    }


    public byte[] ToArray()
    {
        return ((MemoryStream)BaseStream).ToArray();
    }
}
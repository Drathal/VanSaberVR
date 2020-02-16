using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class PacketWriter : BinaryWriter
{
    public Opcodes Opcode { get; set; }
    public ushort Size { get; set; }
    public int Length { get { return (int)BaseStream.Length; } }

    public PacketWriter() : base(new MemoryStream()) { }
    public PacketWriter(Opcodes opcode)
        : base(new MemoryStream())
    {

        Opcode = (Opcodes)opcode;
    }


    public byte[] ReadDataToSend(bool isAuthPacket = false)
    {
        byte[] data = new byte[BaseStream.Length];
        Seek(0, SeekOrigin.Begin);

        for (int i = 0; i < BaseStream.Length; i++)
            data[i] = (byte)BaseStream.ReadByte();

        Size = (ushort)(data.Length - 2);
        if (!isAuthPacket)
        {
            data[0] = (byte)(Size / 0x100);
            data[1] = (byte)(Size % 0x100);
        }

        return data;
    }

    public void WriteCString(string input)
    {
        byte[] data = Encoding.UTF8.GetBytes(input + '\0');
        Write(data);
    }

    public byte[] PacketData
    {
        get
        {
            return (this.BaseStream as MemoryStream).ToArray();
        }
    }

    public void Seek(int offset)
    {
        base.Seek(offset, SeekOrigin.Begin);
    }

    public void WriteInt8(sbyte data)
    {
        base.Write(data);
    }

    public void WriteInt16(short data)
    {
        base.Write(data);
    }

    public void WriteInt32(int data)
    {
        base.Write(data);
    }

    public void WriteInt64(long data)
    {
        base.Write(data);
    }

    public void WriteUInt8(byte data)
    {
        base.Write(data);
    }

    public void WriteUInt16(ushort data)
    {
        base.Write(data);
    }

    public void WriteUInt32(uint data)
    {
        base.Write(data);
    }

    public void WriteUInt64(ulong data)
    {
        base.Write(data);
    }

    public void WriteFloat(float data)
    {
        base.Write(data);
    }

    public void WriteDouble(double data)
    {
        base.Write(data);
    }

    public void WriteString(string data)
    {
        byte[] sBytes = Encoding.ASCII.GetBytes(data);
        this.WriteBytes(sBytes);
        base.Write((byte)0);    // String null terminated
    }

    public void WriteBytes(byte[] data)
    {
        base.Write(data);
    }
}
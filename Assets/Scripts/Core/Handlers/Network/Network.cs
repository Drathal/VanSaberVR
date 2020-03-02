using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using static HelperClass;

public class NetSocket
{
    public Socket mSocket = null;
    byte[] DataBuffer;
    
    public NetSocket()
    {

    }

    public bool ConnectToServer(string addr)
    {
        IPAddress ASAddr;

        try
        {
            ASAddr = System.Net.IPAddress.Parse(addr);
            IPEndPoint ASDest = new IPEndPoint(ASAddr, 3724);
            mSocket = new Socket(ASDest.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            mSocket.SendTimeout = 1000;
            mSocket.Connect(ASDest);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    } 

    public void ReceivePacket()
    {
        try
        {
            if (mSocket != null)
            {
                if (mSocket.Connected)
                {
                    while (mSocket.Available > 0)
                    {

                        DataBuffer = new byte[mSocket.Available];
                        mSocket.Receive(DataBuffer, DataBuffer.Length, SocketFlags.None);

                        HandleData(DataBuffer);
                    }
                }
                else
                {
                    mSocket.Close();
                    ConnectToServer("127.0.0.1");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error Reading Data." + e.ToString());
        }
    }

    public void HandleData(byte[] data)
    {
        try
        {
            for (int index = 0; index < data.Length; index++)
            {
                byte[] headerData = new byte[6];
                Array.Copy(data, index, headerData, 0, 6);
                DecodeOpcode(headerData);
                Array.Copy(headerData, 0, data, index, 6);

                ushort opcode = BitConverter.ToUInt16(headerData, 0);
                int length = BitConverter.ToInt16(headerData, 2);

                Opcodes code = (Opcodes)opcode;

                byte[] packetData = new byte[length + 2];

                Array.Copy(data, index, packetData, 0, length + 2);

                index += 2 + (length - 1);

                switch ((Opcodes)opcode)
                {
                    case Opcodes.SETTINGS_WRITE:
                        SettingRead(packetData);
                        break;
                    default:
                        break;

                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error Reading Data." + e.ToString());
        }

    }

    void SettingRead(byte[] data)
    {
        try
        {
            PacketIn packet = new PacketIn(data);
            _currentSettings = new Settings()
            {
                UserName = Decrypt(packet.ReadString()),
                LeftColor = ColourFromInt(packet.ReadInt32()),
                RightColor = ColourFromInt(packet.ReadInt32()),
                WallFrameColor = ColourFromInt(packet.ReadInt32()),
                LeftLightColor = ColourFromInt(packet.ReadInt32()),
                RightLightColor = ColourFromInt(packet.ReadInt32()),
                ShowDebris = packet.ReadInt32(),
                ShowSparks = packet.ReadInt32(),
                CutSoundLevel = packet.ReadInt32(),
                ArmDistance = packet.ReadFloat(),
                PlayerHeight = packet.ReadFloat(),
                LastKnownMenu = Decrypt(packet.ReadString()),
                LastKnownPlatform = Decrypt(packet.ReadString()),
                LastKnownSaberSet = Decrypt(packet.ReadString()),
                LastKnownNoteSet = Decrypt(packet.ReadString())
            };
        }
        catch (Exception)
        {
            if (File.Exists(@Application.dataPath + "/Data.mjd"))
            {
                File.Delete(@Application.dataPath + "/Data.mjd");
            }

            Settings newSettings = new Settings()
            {
                UserName = "CloneSaberTester",
                LeftColor = Color.red,
                RightColor = Color.blue,
                WallFrameColor = Color.white,
                LeftLightColor = Color.red,
                RightLightColor = Color.blue,
                ShowDebris = 1,
                ShowSparks = 1,
                CutSoundLevel = 30,
                ArmDistance = 1.5f,
                PlayerHeight = 1,
                LastKnownMenu = "",
                LastKnownPlatform = "",
                LastKnownSaberSet = "",
                LastKnownNoteSet = ""
            };

            SettingsHandler.Instance.WriteSettings(newSettings);
            SettingsHandler.Instance.ReadSettings();
        }
    }
    
    public byte[] EncodeOpcode(int size, int opcode)
    {
        var index = 0;
        var newSize = size + 2;
        var header = new byte[4];
        if (newSize > 0x7FFF)
        {
            header[index++] = (byte)(0x80 | (0xFF & (newSize >> 16)));
        }

        header[index++] = (byte)(0xFF & (newSize >> 8));
        header[index++] = (byte)(0xFF & newSize);
        header[index++] = (byte)(0xFF & opcode);
        header[index] = (byte)(0xFF & (opcode >> 8));

        return header;
    }

    public void DecodeOpcode(byte[] header)
    {
        ushort length;
        short opcode;

        length = BitConverter.ToUInt16(new byte[] { header[1], header[0] }, 0);
        opcode = BitConverter.ToInt16(header, 2);

        header[0] = BitConverter.GetBytes(opcode)[0];
        header[1] = BitConverter.GetBytes(opcode)[1];

        header[2] = BitConverter.GetBytes(length)[0];
        header[3] = BitConverter.GetBytes(length)[1];
    }

    public void SendPacket(PacketWriter packet)
    {        
        byte[] endData = FinalisePacket(packet);

        SendData(endData);
    }

    public byte[] FinalisePacket(PacketWriter packet)
    {

        BinaryWriter endPacket = new BinaryWriter(new MemoryStream());
        byte[] header = this.EncodeOpcode(packet.PacketData.Length, (short)packet.Opcode);

        endPacket.Write(header);
        endPacket.Write(packet.PacketData);

        var data = (endPacket.BaseStream as MemoryStream).ToArray();

        return data;
    }

    private const int Keysize = 256;    
    private const int DerivationIterations = 1000;

    public string Encrypt(string plainText)
    {
        var saltStringBytes = Generate256BitsOfRandomEntropy();
        var ivStringBytes = Generate256BitsOfRandomEntropy();
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        using (var password = new Rfc2898DeriveBytes("CloneSaber", saltStringBytes, DerivationIterations))
        {
            var keyBytes = password.GetBytes(Keysize / 8);
            using (var symmetricKey = new RijndaelManaged())
            {
                symmetricKey.BlockSize = 256;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;
                using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            var cipherTextBytes = saltStringBytes;
                            cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                            cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                            memoryStream.Close();
                            cryptoStream.Close();
                            return Convert.ToBase64String(cipherTextBytes);
                        }
                    }
                }
            }
        }
    }

    public string Decrypt(string cipherText)
    {
        // Get the complete stream of bytes that represent:
        // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
        var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
        // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
        var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
        // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
        var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
        // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
        var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

        using (var password = new Rfc2898DeriveBytes("CloneSaber", saltStringBytes, DerivationIterations))
        {
            var keyBytes = password.GetBytes(Keysize / 8);
            using (var symmetricKey = new RijndaelManaged())
            {
                symmetricKey.BlockSize = 256;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;
                using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                {
                    using (var memoryStream = new MemoryStream(cipherTextBytes))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            var plainTextBytes = new byte[cipherTextBytes.Length];
                            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            memoryStream.Close();
                            cryptoStream.Close();
                            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                        }
                    }
                }
            }
        }
    }

    private static byte[] Generate256BitsOfRandomEntropy()
    {
        var randomBytes = new byte[32];
        using (var rngCsp = new RNGCryptoServiceProvider())
        {
            rngCsp.GetBytes(randomBytes);
        }
        return randomBytes;
    }


    public void SendData(byte[] send)
    {

        var buffer = new byte[send.Length];
        Buffer.BlockCopy(send, 0, buffer, 0, send.Length);

        try
        {
            mSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, delegate { }, null);
        }
        catch (Exception e)
        {
            mSocket.Close();
            this.mSocket = null;
            
            Console.WriteLine("Error sending Packet.");
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NS_Utils;
using NS_Struct;
using NS_ByteArray;

namespace NS_Tftp
{
    public enum TftpStatus
    {
        Success,
        Err_Net,
        Err_WrReq,
        Err_WrData,
        Err_Retry,
        WriteProgress,
    };

    public delegate void TftpMsg(TftpStatus status, string text);

    public class Tftp
    {
        private UdpClient client = null;
        private const int TFTP_MAX_DATE_SIZE = 512;
        private TftpMsg msg_CallBack = null;
        private string remoteIP = null;
        private int remotePort = 69;
        private int retryCnt = 20;

        private enum TftpCode
        {
            Rrq = 1,
            Wrq,
            Data,
            Ack,
            Err,
        };

        /*
        *   2 bytes     2 bytes      
        *   -----------------------
        *   | Opcode |   Block #  |   
        *   -----------------------
        */
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct Reply
        {
            public UInt16 opCode;
            public UInt16 block;
        }

        /*
        *   2 bytes     string    1 byte     string   1 byte
        *   ------------------------------------------------
        *   | Opcode |  Filename  |   0  |    Mode    | 0  |
        *   ------------------------------------------------
        */
        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi, Pack=1)]
        public struct WriteReqHead
        {
            public UInt16 opCode;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string fileName;
            public byte rsv1;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct WriteReqTail
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string mode;
            public byte rsv2;
        }

        /*
        *    2 bytes     2 bytes      n bytes
        *    ----------------------------------
        *    | Opcode |   Block #  |   Data   |
        *    ----------------------------------
        */
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct WriteData
        {
            public UInt16 opCode;
            public UInt16 block;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = TFTP_MAX_DATE_SIZE)]
            public byte[] data;
        }

        public Tftp(int localPort, string remoteIP, TftpMsg msg_CallBack)
        {
            this.remoteIP = remoteIP;
            this.msg_CallBack = msg_CallBack;
            client = new UdpClient(localPort);
        }

        public bool upload(string filePath, byte xor)
        {          
            try
            {                
                client.Client.ReceiveTimeout = 20000;
                if (writeReq(client, filePath) == false)
                {
                    msg_CallBack?.Invoke(TftpStatus.Err_WrReq, null);
                    return false;
                }
                client.Client.ReceiveTimeout = 1000;
                if (writeData(client, filePath, xor) == false)
                {
                    msg_CallBack?.Invoke(TftpStatus.Err_WrData, null);
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                msg_CallBack?.Invoke(TftpStatus.Err_Net, null);
                return false;
            }
            finally
            {
                client.Close();
            }            
            return true;
        }

        private bool writeReq(UdpClient client, string filePath)
        {
            byte[] byteArray = BitConverter.GetBytes(Utils.htons((UInt16)TftpCode.Wrq));
            byte[] byteArray1 = Encoding.Default.GetBytes(Path.GetFileName(filePath));
            byteArray = ByteArray.CombomByteArray(byteArray, byteArray1);

            byteArray1 = new byte[1];
            byteArray1[0] = 0;
            byteArray = ByteArray.CombomByteArray(byteArray, byteArray1);

            byteArray1 = Encoding.Default.GetBytes("octet");            
            byteArray = ByteArray.CombomByteArray(byteArray, byteArray1);

            byteArray1 = new byte[1];
            byteArray1[0] = 0;
            byteArray = ByteArray.CombomByteArray(byteArray, byteArray1);

            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Parse(remoteIP), remotePort);
            client.Send(byteArray, byteArray.Length, remoteIpEndPoint);

            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] receiveBytes = client.Receive(ref remoteEP);
            Reply reply = (Reply)Struct.BytesToStuct(receiveBytes, typeof(Reply));
            reply.opCode = Utils.htons(reply.opCode);
            reply.block = Utils.htons(reply.block);
            if ((reply.opCode != (UInt16)TftpCode.Ack) || (reply.block != 0))
            {
                return false;
            }
            remoteIP = remoteEP.Address.ToString();
            remotePort = remoteEP.Port;
            Console.WriteLine("IP: {0}, Port: {1}", remoteIP, remotePort);
            return true;
        }
        
        public bool writeData(UdpClient client, string filePath, byte xor)
        {
            bool ret = true;
            FileInfo fileInfo = new FileInfo(filePath);
            long fileTotalLength = fileInfo.Length;
            long fileWriteLenght = 0;
            try
            {
                FileStream fileStreamIn = File.OpenRead(filePath);
                byte[] buffer = new byte[TFTP_MAX_DATE_SIZE];
                int readLenth = 0;
                UInt16 nextBlock = 0;
                do
                {
                    readLenth = fileStreamIn.Read(buffer, 0, buffer.Length);
                    for (int i = 0; i < readLenth; ++i)
                    {
                        buffer[i] ^= xor;
                    }
                    nextBlock++;

                    WriteData outData;
                    outData.opCode = Utils.htons((UInt16)TftpCode.Data);
                    outData.block = Utils.htons(nextBlock);
                    outData.data = buffer;
                    byte[] bytes = Struct.StructToBytes(outData);

                    int count = 0;
                    bool replyOK = false;
                    do
                    {
                        try
                        {
                            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Parse(remoteIP), remotePort);
                            client.Send(bytes, Marshal.SizeOf(outData.opCode)
                                + Marshal.SizeOf(outData.block) + readLenth, remoteIpEndPoint);

                            Reply reply;
                            do
                            {
                                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                                byte[] receiveBytes = client.Receive(ref remoteEP);
                                reply = (Reply)Struct.BytesToStuct(receiveBytes, typeof(Reply));
                                reply.opCode = Utils.htons(reply.opCode);
                                reply.block = Utils.htons(reply.block);
                            } while ((reply.opCode != (UInt16)TftpCode.Ack) || (reply.block != nextBlock));
                            replyOK = true;
                        }
                        catch
                        {
                            count++;
                            msg_CallBack?.Invoke(TftpStatus.Err_Retry, count.ToString());
                        }
                        finally
                        {
                            if (count >= retryCnt)
                            {
                                ret = false;
                            }
                        }
                    } while ((replyOK == false) && (ret == true));
                    /* 发送进度 */
                    if (replyOK == true)
                    {
                        fileWriteLenght += readLenth;
                        int progress = (int)((float)fileWriteLenght / fileTotalLength * 100);
                        msg_CallBack?.Invoke(TftpStatus.WriteProgress, progress.ToString());
                    }  
                } while ((ret == true) && (readLenth >= TFTP_MAX_DATE_SIZE)); /* 最后的数据包长度为0时，仍要发送给服务器 */
                if (ret == true)
                {
                    msg_CallBack?.Invoke(TftpStatus.Success, "发送 [ " + fileWriteLenght.ToString() + "/" +
                                    fileTotalLength.ToString() + " ] Bytes!");
                }              
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return ret;
        }
    }
}

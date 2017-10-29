using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NS_Communication;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using NS_Struct;
using NS_Utils;
using NS_ByteArray;
using NS_FirewallHelp;

namespace NS_Communication
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ProtocalHead
    {
        public byte start;
        public byte addr;
        public byte index;
        public byte rsv1;
        public UInt16 size;
        public byte rsv2;
        public byte token;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ProtocalTail
    {
        public UInt16 crc16;
        public byte end;
    }

    public class UdpCommu : Communication
    {
        private UdpClient client = null;
        public IPEndPoint remoteEP { get; set; }

        public UdpCommu(int localPort, IndexOutputType indexOutputType)
        {
            this.indexOutputType = indexOutputType;
            client = new UdpClient(localPort);           
            
            UdpCommuReceive commuReceive = new UdpCommuReceive(indexOutputType, this, notifyReceiveDoneEventHandler);
        }

        public void close()
        {
            if (client != null)
            {
                client.Close();
                client = null;
            }
        }

        public bool connect(IPEndPoint remoteEP)
        {
            bool ret = true;
            try
            {
                client.Connect(remoteEP);
            }
            catch
            {
                ret = false;
            }
            return ret;
        }

        protected override byte[] packageHead(int bodySize)
        {
            ProtocalHead head;
            head.start = Protocal.PROTOCAL_START;
            head.addr = (byte)Communication.ADDR_TYPE.UDP;
            generalNextOutIndex(indexOutputType);
            head.index = outIndex;
            head.rsv1 = Protocal.PROTOCAL_RSV;
            head.size = (UInt16)bodySize;
            head.rsv2 = Protocal.PROTOCAL_RSV;
            head.token = Protocal.PROTOCAL_TOKEN;
            return Struct.StructToBytes(head);
        }

        protected override byte[] packageBody(byte[] buffer, int startIndex, int length)
        {
            byte[] body = new byte[length];
            Array.Copy(buffer, startIndex, body, 0, length);
            return body;
        }

        protected override byte[] packageTail(byte[] head, byte[] body)
        {
            byte[] crcData = ByteArray.CombomByteArray(head, body);
            ProtocalTail tail;
            tail.crc16 = Utils.getCRC16(0, crcData, crcData.Length);
            tail.end = Protocal.PROTOCAL_END;
            return Struct.StructToBytes(tail);
        }

        protected override byte[] packageFrame(byte[] head, byte[] body, byte[] tail)
        {
            byte[] temp = ByteArray.CombomByteArray(head, body);
            return ByteArray.CombomByteArray(temp, tail);
        }

        protected override bool send(byte[] frame)
        {
            bool ret = true;
            try
            {
                client.Send(frame, frame.Length, remoteEP);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ret = false;
            }
            return ret;
        }

        public override bool receive(out byte[] frame)
        {
            bool ret = true;
            frame = null;
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                frame = client.Receive(ref ep);
                if (remoteEP.Address == IPAddress.Broadcast)
                {
                    remoteEP = ep;
                }
            }
            catch
            {
                ret = false;
            }
            return ret;
        }

        protected override Object getReplyHead(byte[] package)
        {
            byte[] headBytes = ByteArray.CopyTo(package, 0, Marshal.SizeOf(typeof(ProtocalHead)));
            return Struct.BytesToStuct(headBytes, typeof(ProtocalHead));
        }

        protected override Object getReplyTail(byte[] package)
        {
            ProtocalHead head = (ProtocalHead)getReplyHead(package);
            byte[] tailBytes = ByteArray.CopyTo(package, Marshal.SizeOf(typeof(ProtocalHead)) + head.size, Marshal.SizeOf(typeof(ProtocalTail)));
            return Struct.BytesToStuct(tailBytes, typeof(ProtocalTail));
        }

        protected override byte getReplyStatus(byte[] package)
        {
            byte[] bodyBytes = ByteArray.CopyTo(package, Marshal.SizeOf(typeof(ProtocalHead)), Marshal.SizeOf(typeof(Protocal.CmdReplyVoid)));
            Protocal.CmdReplyVoid reply = (Protocal.CmdReplyVoid)Struct.BytesToStuct(bodyBytes, typeof(Protocal.CmdReplyVoid));
            return reply.status;
        }

        protected override ushort getReplyCmd(byte[] package)
        {
            byte[] bodyBytes = ByteArray.CopyTo(package, Marshal.SizeOf(typeof(ProtocalHead)), Marshal.SizeOf(typeof(Protocal.CmdReplyVoid)));
            Protocal.CmdReplyVoid reply = (Protocal.CmdReplyVoid)Struct.BytesToStuct(bodyBytes, typeof(Protocal.CmdReplyVoid));
            return reply.cmd;
        }

        protected override byte[] getReplyData(byte[] package)
        {
            ProtocalHead head = (ProtocalHead)getReplyHead(package);
            return ByteArray.CopyTo(package, Marshal.SizeOf(typeof(ProtocalHead)), head.size);
        }

        protected override byte getReplyIndex(byte[] package)
        {
            ProtocalHead head = (ProtocalHead)getReplyHead(package);
            return head.index;
        }
    }

    class UdpCommuReceive
    {
        enum ReceiveStatus
        {
            Start = 0,
            Addr,
            Index,
            Size,
            Token,
            Data,
            Crc16,
            End,
        }

        private ReceiveStatus receiveStatus = ReceiveStatus.Start;
        private int rxIndex = 0;
        private int dataSize = 0;
        private notifyReplyEventHandler notifyDone;
        private UdpCommu udpCommu;

        private void CommuReceiveThread()
        {
            byte[] receivePackage;
            byte[] framePakcage; 
            while (true)
            {
                if (udpCommu.receive(out receivePackage) == false)
                {
                    continue;
                }
                if (pullPackage(receivePackage, out framePakcage) == false)
                {
                    continue;
                }
                if (checkout(framePakcage) == false)
                {
                    continue;
                }
                notifyDone?.Invoke(framePakcage);
            }
        }

        public UdpCommuReceive(Communication.IndexOutputType indexOutputType, 
                UdpCommu udpCommu,
                notifyReplyEventHandler notifyDone)
        {
            this.udpCommu = udpCommu;
            this.notifyDone = notifyDone;
            Thread childThread = new Thread(new ThreadStart(CommuReceiveThread));
            childThread.IsBackground = true;
            childThread.Start();
        }

        private bool pullPackage(byte[] buffer, out byte[] package)
        {
            package = new byte[0];

            bool ret = false;
            foreach (byte data in buffer)
            {
                switch (receiveStatus)
                {
                    case ReceiveStatus.Start:
                        if (data == Protocal.PROTOCAL_START)
                        {
                            package = ByteArray.CombomByteArray(package, new byte[] { data });
                            receiveStatus = ReceiveStatus.Addr;
                        }
                        break;
                    case ReceiveStatus.Addr:
                        if (data != (byte)Communication.ADDR_TYPE.UDP)
                        {
                            receiveStatus = ReceiveStatus.Start;
                        }
                        else
                        {
                            package = ByteArray.CombomByteArray(package, new byte[] { data });
                            receiveStatus = ReceiveStatus.Index;
                            rxIndex = 0;
                        }
                        break;
                    case ReceiveStatus.Index:
                        package = ByteArray.CombomByteArray(package, new byte[] { data });
                        if (++rxIndex >= 2)
                        {
                            rxIndex = 0;
                            receiveStatus = ReceiveStatus.Size;
                        }
                        break;
                    case ReceiveStatus.Size:
                        package = ByteArray.CombomByteArray(package, new byte[] { data });
                        rxIndex++;
                        if (rxIndex == 1)
                        {
                            dataSize = data;
                        }
                        else if (rxIndex == 2)
                        {                           
                            dataSize |= data << 8;
                        }
                        else
                        {
                            receiveStatus = ReceiveStatus.Token;
                        }
                        break;
                    case ReceiveStatus.Token:
                        if (data != (byte)Protocal.PROTOCAL_TOKEN)
                        {
                            receiveStatus = ReceiveStatus.Start;
                        }
                        else
                        {
                            package = ByteArray.CombomByteArray(package, new byte[] { data });
                            receiveStatus = ReceiveStatus.Data;
                        }
                        break;
                    case ReceiveStatus.Data:
                        if ((dataSize--) != 0)
                        {
                            package = ByteArray.CombomByteArray(package, new byte[] { data });
                        }
                        if (dataSize == 0)
                        {
                            receiveStatus = ReceiveStatus.Crc16;
                            rxIndex = 0;
                        }
                        break;
                    case ReceiveStatus.Crc16:
                        package = ByteArray.CombomByteArray(package, new byte[] { data });
                        if (++rxIndex >= 2)
                        {
                            receiveStatus = ReceiveStatus.End;
                        }
                        break;
                    case ReceiveStatus.End:
                        if (data == (byte)Protocal.PROTOCAL_END)
                        {
                            package = ByteArray.CombomByteArray(package, new byte[] { data });
                            ret = true;
                        }
                        receiveStatus = ReceiveStatus.Start;
                        break;
                }
            }
            return ret;
        }

        private bool checkout(byte[] buffer)
        {
            byte[] headBytes = ByteArray.CopyTo(buffer, 0, Marshal.SizeOf(typeof(ProtocalHead)));
            ProtocalHead head = (ProtocalHead)Struct.BytesToStuct(headBytes, typeof(ProtocalHead));
            byte[] bodyBytes = ByteArray.CopyTo(buffer, Marshal.SizeOf(typeof(ProtocalHead)), head.size);
            byte[] tailBytes = ByteArray.CopyTo(buffer, Marshal.SizeOf(typeof(ProtocalHead)) + head.size, Marshal.SizeOf(typeof(ProtocalTail)));
            ProtocalTail tail = (ProtocalTail)Struct.BytesToStuct(tailBytes, typeof(ProtocalTail));
            UInt16 crc = Utils.getCRC16(0, ByteArray.CombomByteArray(headBytes, bodyBytes), Marshal.SizeOf(typeof(ProtocalHead)) + head.size);
            return (crc == tail.crc16);
        }
    }
}

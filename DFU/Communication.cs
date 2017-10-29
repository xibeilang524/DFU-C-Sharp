using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NS_Communication
{
    public delegate void notifyReplyEventHandler(byte[] package);

    public enum ErrorStatus
    {
        TimeOutError = 0,
        CommuError,
        ReceivePackageLengthError,
        SendPackageLengthOverflow,
    }

    public class SendCmdException : ApplicationException
    {
        public ErrorStatus errorStatus;

        public SendCmdException(ErrorStatus errorStatus)
        {
            this.errorStatus = errorStatus;
        }
    }

    public abstract class Communication
    {
        protected int maxDataFrameLenth = 100;
        protected int maxRetryCnt = 3;
        protected int sendPackageTimeout = 300;
        private List<byte[]> replyCmdList;
        private notifyReplyEventHandler sampleEvent = null;
        private bool timerTimeout = false;

        public enum ADDR_TYPE
        {
            UDP = 0,
            COM,
        }

        public enum IndexOutputType
        {
            UnusualMasterSendSlaveReply = 0,
            UnusualMasterSendSlaveNoReply,
            UnusualSlaveSendMasterReply,
            UsualMasterSendSlaveReply,
            UsualMasterSendSlaveNoReply,
            UsualSlaveSendMasterReply,
        }

        public enum IndexInputType
        {
            UnusualMasterSendSlaveReply = 0,
            UnusualSlaveSendMasterNoReply,
            UnusualSlaveSendMasterReply,
            UsualMasterSendSlaveReply,
            UsualSlaveSendMasterNoReply,
            UsualSlaveSendMasterReply,
        }

        public Communication()
        {
            replyCmdList = new List<byte[]>();
        }

        protected byte outIndex { get; set; }
        protected IndexOutputType indexOutputType{ get; set; }
        protected void generalNextOutIndex(IndexOutputType indexType)
        {
            byte index = outIndex;
            index++;
            switch (indexType)
            {
                case IndexOutputType.UnusualMasterSendSlaveReply:
                    if (index > 0x0F)
                    {
                        index = 0;
                    }
                    break;
                case IndexOutputType.UnusualMasterSendSlaveNoReply:
                    if ((index < 0x10) || (index > 0x17))
                    {
                        index = 0x10;
                    } 
                    break;
                case IndexOutputType.UnusualSlaveSendMasterReply:
                    if ((index < 0x18) || (index > 0x1F))
                    {
                        index = 0x18;
                    }
                    break;
                case IndexOutputType.UsualMasterSendSlaveReply:
                    if ((index < 0x20) || (index > 0x7F))
                    {
                        index = 0x20;
                    }
                    break;
                case IndexOutputType.UsualMasterSendSlaveNoReply:
                    if ((index < 0x80) || (index > 0xBF))
                    {
                        index = 0x80;
                    }
                    break;
                case IndexOutputType.UsualSlaveSendMasterReply:
                    if (index < 0xC0)
                    {
                        index = 0xC0;
                    }
                    break;
            }
            outIndex = index;
        }

        protected IndexInputType getIndexInputType(byte index)
        {
            IndexInputType indexType;
            if (index <= 0x0F)
            {
                indexType = IndexInputType.UnusualMasterSendSlaveReply;
            }
            else if (index <= 0x17)
            {
                indexType = IndexInputType.UnusualSlaveSendMasterNoReply;
            }
            else if (index <= 0x1F)
            {
                indexType = IndexInputType.UnusualSlaveSendMasterReply;
            }
            else if (index <= 0x7F)
            {
                indexType = IndexInputType.UsualMasterSendSlaveReply;
            }
            else if (index <= 0xBF)
            {
                indexType = IndexInputType.UsualSlaveSendMasterNoReply;
            }
            else
            {
                indexType = IndexInputType.UsualSlaveSendMasterReply;
            }
            return indexType;
        }

        protected bool slaveNeedReply()
        {
            bool ret = false;
            switch (indexOutputType)
            {
                case IndexOutputType.UnusualMasterSendSlaveReply:
                case IndexOutputType.UsualMasterSendSlaveReply:
                    ret = true;
                    break;
            }
            return ret;
        }

        protected void RegisterSampleEvent(notifyReplyEventHandler callBack)
        {
            sampleEvent = callBack;
        }

        abstract protected Object getReplyHead(byte[] package);
        abstract protected Object getReplyTail(byte[] package);
        abstract protected byte getReplyStatus(byte[] package);
        abstract protected UInt16 getReplyCmd(byte[] package);
        abstract protected byte[] getReplyData(byte[] package);
        abstract protected byte getReplyIndex(byte[] package);           

        abstract protected byte[] packageHead(int bodySize);
        abstract protected byte[] packageBody(byte[] buffer, int startIndex, int length);
        abstract protected byte[] packageTail(byte[] head, byte[] body);
        abstract protected byte[] packageFrame(byte[] head, byte[] body, byte[] tail);
        abstract protected bool send(byte[] frame);
        abstract public bool receive(out byte[] frame);       

        private void TimerTimeoutCalBack(Object o)
        {
            timerTimeout = true;
        }

        public void sendCmd(byte[] outBuffer, int outLength, out byte[] inBuffer, int inLength)
        {
            inBuffer = null;
            if (outLength > maxDataFrameLenth)
            {
                throw (new SendCmdException(ErrorStatus.SendPackageLengthOverflow));
            }

            byte[] head = packageHead(outLength);
            byte[] body = packageBody(outBuffer, 0, outLength);
            byte[] tail = packageTail(head, body);
            byte[] outFrame = packageFrame(head, body, tail);

            if (slaveNeedReply() == false)
            {
                if (send(outFrame) == false)
                {
                    throw (new SendCmdException(ErrorStatus.CommuError));
                }
            }
            else
            {
                int retryCount = 0;
                while (true)
                {
                    if (send(outFrame) == false)
                    {
                        throw (new SendCmdException(ErrorStatus.CommuError));
                    }

                    timerTimeout = false;
                    replyCmdList.Clear();
                    System.Threading.Timer timer = new System.Threading.Timer(TimerTimeoutCalBack, null, sendPackageTimeout, 0);
                    try
                    {
                        while (timerTimeout == false)
                        {
                            lock (replyCmdList)
                            {
                                if (replyCmdList.Count != 0)
                                {
                                    foreach (byte[] replyPackage in replyCmdList)
                                    {
                                        if (getReplyCmd(outFrame) != getReplyCmd(replyPackage))
                                        {
                                            continue;
                                        }
                                        byte[] replyBody = getReplyData(replyPackage);
                                        if (replyBody.Length != inLength)
                                        {
                                            throw (new SendCmdException(ErrorStatus.ReceivePackageLengthError));
                                        }
                                        inBuffer = replyBody;
                                        return;                                      
                                    }
                                    replyCmdList.Clear();
                                }
                            }
                        }

                        if (timerTimeout)
                        {
                            Console.WriteLine("RetryCount.........." + (retryCount + 1));
                            retryCount++;
                            if (retryCount >= maxRetryCnt)
                            {
                                throw (new SendCmdException(ErrorStatus.TimeOutError));
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    finally
                    {
                        timer.Dispose();
                    }
                }
            }
            
        }

        protected void notifyReceiveDoneEventHandler(byte[] package)
        {
            byte rxIndex = getReplyIndex(package);
            IndexInputType indexType = getIndexInputType(rxIndex);
            switch (indexType)
            {
                case IndexInputType.UnusualMasterSendSlaveReply:
                case IndexInputType.UsualMasterSendSlaveReply:
                    if (rxIndex == outIndex)
                    {
                        lock(replyCmdList)
                        {
                            replyCmdList.Add(package);
                        }
                    }
                    break;
                case IndexInputType.UnusualSlaveSendMasterNoReply:
                case IndexInputType.UsualSlaveSendMasterNoReply:
                    sampleEvent?.Invoke(package);
                    break;
            }
        }

    }
    
    

   
}

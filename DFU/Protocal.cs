using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NS_Communication
{
    class Protocal
    {
        public const byte PROTOCAL_START = 0x1B;
        public const byte PROTOCAL_RSV = 0x00;
        public const byte PROTOCAL_TOKEN = 0x0E;
        public const byte PROTOCAL_END = 0x16;

        /* 命令 */
        //从 0x0000-0x01FF 公共命令，所有控制器所用到的命令
        public const UInt16 CMD_SIGN_ON = 0x0001;               //发回设备名称			
        public const UInt16 CMD_DEVICE_VERSION = 0x0002;        //返回PCB信息及固件版本
        public const UInt16 CMD_RESET = 0x0003;                 //复位设备 *还没有打算好怎么处理
        public const UInt16 CMD_PRIVATE_ACCESS = 0x0004;        //进入/退出特权访问状态
        public const UInt16 CMD_GET_ERROR = 0x0005;             //发回错误代码
        public const UInt16 CMD_CLEAR_ERROR = 0x0006;           //清除错误
        public const UInt16 CMD_READ_PRM = 0x0007;              //读参数
        public const UInt16 CMD_WRITE_PRM = 0x0008;             //写参数 
        public const UInt16 CMD_READ_PRV = 0x0009;              //读取特权参数	
        public const UInt16 CMD_WRITE_PRV = 0x000A;             //写特权参数
        public const UInt16 CMD_BACKUP_PRM = 0x000B;            //备份参数
        public const UInt16 CMD_RESTORE_PRM = 0x000C;           //还原参数	
        public const UInt16 CMD_OBD_QUERY = 0x000D;             //OBD码查询
        public const UInt16 CMD_OBD_RESET = 0x000E;             //OBD码复位       
        public const UInt16 CMD_SIGN_OFF = 0x00FE;              //断开连接
        public const UInt16 CMD_KEEP_ALIVE = 0x00FF;            //保活数据

        public const UInt16 RTM_SAMPLE_VALUE = 0x01C0;          //发送相对值
        public const UInt16 RTM_SAMPLE_CODE = 0x01C1;           //发送绝对码 
        public const UInt16 RTN_SAMPLE_RCODE = 0x01C2;          //发送相对码  

        public const UInt16 CMD_SEND_START = 0x0101;            //发送开始	
        public const UInt16 CMD_SEND_STOP = 0x0102;             //发送停止
        public const UInt16 CMD_SAMPLE_ONE_C = 0x0103;          //仅采集一个通道一次【码】
        public const UInt16 CMD_SAMPLE_ONE_V = 0x0104;          //仅采集一个通道一次【值】
        public const UInt16 CMD_SET_TARE = 0x0105;              //设置零点 
        public const UInt16 CMD_CTRL_MODE = 0x0106;             //控制模式选择
                
        public const UInt16 CMD_PROTECT = 0x0108;               //设备保护使能
        public const UInt16 CMD_STOP = 0x0109;                  //强制中断
        public const UInt16 CMD_STEP_NORM_V = 0x010A;           //发送一个常规步骤
        public const UInt16 CMD_STEP_NORM_C = 0x010B;           //下发一个试验步骤(码方式)
        public const UInt16 CMD_STEP_SPEC = 0x010C;             //发送一个非常规步骤
        public const UInt16 CMD_OPEN_LOOP = 0x010D;             //开环参数

        public const UInt16 CMD_SWITCH = 0x010E;                //开关量控制命令
        public const UInt16 CMD_KBSPD_SEL = 0x010F;             //选择某小键盘速度组 
        public const UInt16 CMD_FORWARD_DIR = 0x0110;           //0表示进程时控制量往增大方向变化；1表示进程时控制量往减少方向变化 

        public const UInt16 CMD_SET_DEVICE_ID = 0x000F;         //设置设备ID 
        public const UInt16 CMD_GET_DEVICE_ID = 0x0010;         //获取设备ID

        public const UInt16 NTF_PROCESSING = 0x0180;
        public const UInt16 NTF_FINISHED = 0x0181;
        public const UInt16 NTF_FUN_KEY = 0x0182;
        public const UInt16 CMD_SAMPLE_PROTECT = 0x001D;        //式样保护

        public const UInt16 CMD_HANDSET_ON = 0xF001;            //手控盒联机	
        public const UInt16 CMD_HANDSET_COMM = 0xF002;          //手控盒命令

        public const UInt16 CMD_PRINT_LOG = 0x0001;             //日志
        public const UInt16 CMD_FIRMWARE_UPDATE = 0x0002;       //固件升级

        /* 发送命令 */
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct CmdTxVoid
        {
            public UInt16 cmd;
        }

        public const UInt16 UpgradePasswd = 0x5AA5;
        public enum UpgradeCode
        {
            AskAllowUpgrade = 0,
            StartUpgrade,
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct CmdTxUpgrade
        {
            public UInt16 cmd;
            public UInt16 passwd;
            public byte code;           
        }

        /* 接收命令 */
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct CmdReplyVoid
        {
            public UInt16 cmd;
            public byte status;
        }

        public enum UpgradeStatus
        {
            Busy = 0,
            NotAllow,
            Allow,
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct CmdRxUpgrade
        {
            public UInt16 cmd;
            public byte status;
            public byte replyCode;   
        }
    }
}

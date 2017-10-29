using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NS_ConsoleRemap;
using NS_Network;
using NS_Tftp;
using System.Diagnostics;
using System.Threading;
using NS_Communication;
using NS_Struct;
using System.Runtime.InteropServices;
using NS_FirewallHelp;
using NetFwTypeLib;

namespace DFU
{
    public partial class Form1 : Form
    {
        private const string bootIP = "192.168.0.111";
        private const int bootPort = 69;
        private const int prmPort = 5555;
        private IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(bootIP), bootPort);
        private const string remoteSubnetMask = "255.255.255.0";
        private string openFilePath = null;
        private string openFileName = null;
        private UdpCommu udpCommu = null;
        private Stopwatch stopWatch = null;
        System.Threading.Timer timer;   

        public Form1()
        {          
            InitializeComponent();
            //添加防火墙
            FirewallHelp.AuthorizeApplication(Text, Application.ExecutablePath, NET_FW_SCOPE_.NET_FW_SCOPE_ALL, NET_FW_IP_VERSION_.NET_FW_IP_VERSION_ANY);

            new ConsoleRemapTextBox(textBox_Log);
            refreshLocalIP();
            Console.WriteLine("请选择升级固件...");
            SetUI2(false);
            udpCommu = new UdpCommu(0, Communication.IndexOutputType.UsualMasterSendSlaveReply); 
        }

        private void refreshLocalIP()
        {
            comboBox_LocalIP.Items.Clear();

            ArrayList netAddrList;
            Network.getNetAddress(out netAddrList);
            foreach (UnicastIPAddressInformation addr in netAddrList)
            {
                if (Network.isEqualSubnet(addr.Address.ToString(), addr.IPv4Mask.ToString(),
                                                    remoteEP.Address.ToString(), remoteSubnetMask) == true)
                {
                    comboBox_LocalIP.Items.Add(addr.Address.ToString());
                    comboBox_LocalIP.SelectedIndex = 0;
                }
            } 
        }

        private void button_Refresh_Click(object sender, EventArgs e)
        {
            refreshLocalIP();
        }

        private void button_File_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "二进制文件|*.bin";
            fileDialog.Title = "Open Bin File";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine("\r\n手动升级说明：\r\n" +
                    "  1、将控制器关机。\r\n" +
                    "  2、用尖锐物体按住控制器【复位】按钮，开机，等待3秒钟后，松开手。\r\n" +
                    "  3、点击【手动升级】按钮。");
                SetUI2(true);
                openFilePath = fileDialog.FileName;
                openFileName = Path.GetFileName(openFilePath);
                textBox_File.Text = openFileName;
            }
            else
            {
                SetUI2(false);
            }
        }

        private PingReply ping(string remoteIP)
        {
            Ping pingSender = new Ping();
            IPAddress address = IPAddress.Parse(remoteIP);
            return pingSender.Send(address);
        }

        private void PingThread()
        {
            Console.WriteLine("Ping " + bootIP + " ...");
            PingReply reply = ping(bootIP);
            if (reply.Status == IPStatus.Success)
            {
                Console.WriteLine("TO  : {0}", reply.Address.ToString());
                Console.WriteLine("RTT : {0}", reply.RoundtripTime);
                Console.WriteLine("TTL : {0}", reply.Options.Ttl);
                Console.WriteLine("连接成功！");
            }
            else
            {
                Console.WriteLine(reply.Status);
                Console.WriteLine("连接失败！");
            }

            button_Ping.Enabled = true;
        }

        private void button_Ping_Click(object sender, EventArgs e)
        {
            button_Ping.Enabled = false;
            Thread upgradeThread = new Thread(new ThreadStart(PingThread));
            upgradeThread.IsBackground = true;
            upgradeThread.Start();
        }

        private void TftpMsg(TftpStatus status, string text)
        {
            switch (status)
            {
                case TftpStatus.Err_Net:
                    Console.WriteLine("网络错误！");
                    break;
                case TftpStatus.Err_Retry:
                    Console.WriteLine("重试次数.........." + text + "次");
                    break;
                case TftpStatus.Err_WrData:
                    Console.WriteLine("发送文件错误！");
                    break;
                case TftpStatus.Err_WrReq:
                    Console.WriteLine("发送写请求错误！");
                    break;
                case TftpStatus.Success:
                    Console.WriteLine("升级完成," + text);
                    break;
                case TftpStatus.WriteProgress:
                    Console.WriteLine("升级进度.........." + text + "%");
                    progressBar_Upgrade.BeginInvoke(new ThreadStart(() => { progressBar_Upgrade.Value = Convert.ToInt16(text); }));
                    
                    break;
                default:
                    Console.WriteLine("未知错误！");
                    break;
            }
        }         

        private void UpgradeThread()
        {
            progressBar_Upgrade.BeginInvoke(new ThreadStart(() => { progressBar_Upgrade.Value = 0; }));
            Tftp tftpClient = new Tftp(0, remoteEP.Address.ToString(), TftpMsg);
            byte xor = 0x55;
            if (openFileName.Contains("OR") || (openFileName.Contains("or")))
            {
                xor = 0;
            }
            bool ret = tftpClient.upload(openFilePath, xor);
            if (ret == true)
            {
                stopWatch.Stop();

                MessageBox.Show("恭喜你，升级成功！！！\r\n" +
                    "升级耗时：「 " + stopWatch.Elapsed.ToString() + " 」", "提示信息",
                    MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }

            button_AutoUpgrade.BeginInvoke(new ThreadStart(() =>
            {
                SetUI1(true);
                SetUI2(true);
            }));
        }

        private bool autoUpgrade()
        {
            Console.WriteLine("Try auto upgrade...");     
            udpCommu.remoteEP = udpCommu.remoteEPList.ElementAt(comboBox_deviceIP.SelectedIndex);
            byte[] outBytes = buildUpgradeCmd((byte)Protocal.UpgradeCode.AskAllowUpgrade);
            byte[] inBytes;
            try
            {
                Console.WriteLine("Shake hands...");
                udpCommu.sendCmd(outBytes, outBytes.Length, out inBytes, Marshal.SizeOf(typeof(Protocal.CmdRxUpgrade)));
                Protocal.CmdRxUpgrade upgress = (Protocal.CmdRxUpgrade)Struct.BytesToStuct(inBytes, typeof(Protocal.CmdRxUpgrade));
                if (upgress.replyCode != (byte)Protocal.UpgradeStatus.Allow)
                {
                    Console.WriteLine("Quit auto upgrade, because of " + (Protocal.UpgradeStatus)upgress.replyCode);
                    MessageBox.Show("控制器正在忙，拒绝升级！", "提示信息",
                        MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return false;
                }
                Console.WriteLine("OK!");

                Console.WriteLine("Start...");
                outBytes = buildUpgradeCmd((byte)Protocal.UpgradeCode.StartUpgrade);
                udpCommu.sendCmd(outBytes, outBytes.Length, out inBytes, Marshal.SizeOf(typeof(Protocal.CmdRxUpgrade)));
                upgress = (Protocal.CmdRxUpgrade)Struct.BytesToStuct(inBytes, typeof(Protocal.CmdRxUpgrade));
                if (upgress.replyCode != (byte)Protocal.UpgradeStatus.Allow)
                {
                    Console.WriteLine("Quit auto upgrade, because of " + (Protocal.UpgradeStatus)upgress.replyCode);
                    MessageBox.Show("控制器正在忙，拒绝升级！", "提示信息",
                        MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return false;
                }
                Console.WriteLine("OK!");
            }
            catch (SendCmdException e)
            {
                Console.WriteLine("Quit auto upgrade, because of " + e.errorStatus);
                return false;
            }            

            return true;
        }

        private void upgradeHandler()
        {
            Console.WriteLine("Connect " + remoteEP + " ...");
            Thread upgradeThread = new Thread(new ThreadStart(UpgradeThread));
            upgradeThread.IsBackground = true;
            upgradeThread.Start();
        }

        private void TimerTimeoutCalBack(Object o)
        {
            Console.WriteLine("OK!");
            timer.Dispose();
            Console.WriteLine("Ping " + remoteEP + " ...");
            while (true)
            {
                PingReply reply = ping(remoteEP.Address.ToString());
                if (reply.Status == IPStatus.Success)
                {
                    break;
                }
            }
            Console.WriteLine("OK");
            upgradeHandler();
        }

        private void SetUI1(bool status)
        {
            button_AutoUpgrade.Enabled = status;
            button_manualUpgrade.Enabled = status;
            button_Ping.Enabled = status;
            button_File.Enabled = status;
            button_Refresh.Enabled = status;
            button_searchDevice.Enabled = status;
        }

        private void SetUI2(bool status)
        {
            if (status == false)
            {
                button_AutoUpgrade.Enabled = false;
                button_manualUpgrade.Enabled = false;
            }
            else
            {
                if (comboBox_deviceIP.Items.Count == 0)
                {
                    button_AutoUpgrade.Enabled = false;
                    button_manualUpgrade.Enabled = true;
                }
                else
                {
                    button_AutoUpgrade.Enabled = true;
                    button_manualUpgrade.Enabled = false;
                }
            }          
        }

        private void button_AutoUpgrade_Click(object sender, EventArgs e)
        {
            stopWatch = new Stopwatch();
            stopWatch.Start();
            SetUI1(false);
 
            if (autoUpgrade() == true)
            {
                Console.WriteLine("Wait for PRM jump to boot...");
                const int waitPRMJumpTimeout = 1000;
                timer = new System.Threading.Timer(TimerTimeoutCalBack, null, waitPRMJumpTimeout, 0);
            }
            else
            {
                SetUI1(true);
                SetUI2(true);
            }
        }

        [DllImport("user32", EntryPoint = "HideCaret")]
        private static extern bool HideCaret(IntPtr hWnd);
        private void textBox_Log_MouseDown(object sender, MouseEventArgs e)
        {            
            HideCaret((sender as TextBox).Handle);
        }

        private byte[] buildUpgradeCmd(byte code)
        {
            Protocal.CmdTxUpgrade outPackage;
            outPackage.cmd = Protocal.CMD_FIRMWARE_UPDATE;
            outPackage.code = (byte)Protocal.UpgradeCode.AskAllowUpgrade;
            outPackage.passwd = Protocal.UpgradePasswd;
            return Struct.StructToBytes(outPackage);
        }
        
        private void UpdateDeviceIP()
        {
            comboBox_deviceIP.Items.Clear();
            byte[] sendPackage = buildUpgradeCmd((byte)Protocal.UpgradeCode.AskAllowUpgrade);
            if (udpCommu.sendBroadcastCmd(prmPort, sendPackage, sendPackage.Length) == true)
            {
                if (udpCommu.remoteEPList.Count != 0)
                {                    
                    foreach (IPEndPoint ep in udpCommu.remoteEPList)
                    {
                        comboBox_deviceIP.Items.Add(ep.ToString());
                    }
                    comboBox_deviceIP.SelectedIndex = 0;
                    SetUI2(true);                    
                }
                Console.WriteLine("支持自动升级的控制器 .......... " + udpCommu.remoteEPList.Count + "个");
            }
        }

        private void button_searchDevice_Click(object sender, EventArgs e)
        {
            UpdateDeviceIP();
        }

        private void button_manualUpgrade_Click(object sender, EventArgs e)
        {
            stopWatch = new Stopwatch();
            stopWatch.Start();
            SetUI1(false);
            upgradeHandler();
        }

        private void comboBox_deviceIP_SelectedIndexChanged(object sender, EventArgs e)
        {
            remoteEP = udpCommu.remoteEPList.ElementAt(comboBox_deviceIP.SelectedIndex);
            remoteEP.Port = bootPort;
            refreshLocalIP();
        }
    }
}

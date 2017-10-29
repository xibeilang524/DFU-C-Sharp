using System;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;


namespace NS_Network
{
    public class Network
    {
        public static void getNetAddress(out ArrayList netAddr)
        {
            netAddr = new ArrayList();

            NetworkInterface[] networkInterfaceList = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in networkInterfaceList)
            {
                UnicastIPAddressInformationCollection allAddress = adapter.GetIPProperties().UnicastAddresses;
                if (allAddress.Count > 0)
                {
                    if (adapter.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation addr in allAddress)
                        {
                            if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                netAddr.Add(addr);
                            }
                        }
                    }
                }
            }
        }

        public static bool isEqualSubnet(string ip1, string subnetMask1, string ip2, string subnetMask2)
        {
            string[] ip1List = ip1.Split('.');
            string[] subnet1List = subnetMask1.Split('.');
            string[] ip2List = ip2.Split('.');
            string[] subnet2List = subnetMask2.Split('.');
            bool ret = true;
            for (int i=0; i<ip1List.Length; ++i)
            {
                int value1 = int.Parse(ip1List[i]) & int.Parse(subnet1List[i]);
                int value2 = int.Parse(ip2List[i]) & int.Parse(subnet2List[i]);
                if (value1 != value2)
                {
                    ret = false;
                    break;
                }
            }
            return ret;
        }
    }
}


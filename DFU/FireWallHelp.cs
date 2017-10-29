using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFwTypeLib;

namespace NS_FirewallHelp
{
    public static class FirewallHelp
    {
        private const string CLSID_FIREWALL_MANAGER = "{304CE942-6E39-40D8-943A-B913C40C9CD4}";

        private static INetFwMgr GetFirewallManager()
        {
            Type objectType = Type.GetTypeFromCLSID(new Guid(CLSID_FIREWALL_MANAGER));
            return Activator.CreateInstance(objectType) as NetFwTypeLib.INetFwMgr;
        }

        // ProgID for the AuthorizedApplication object
        private const string PROGID_AUTHORIZED_APPLICATION = "HNetCfg.FwAuthorizedApplication";
        public static bool AuthorizeApplication(string title, string applicationPath, NET_FW_SCOPE_ scope, NET_FW_IP_VERSION_ ipVersion)
        {
            // Create the type from prog id
            Type type = Type.GetTypeFromProgID(PROGID_AUTHORIZED_APPLICATION);
            INetFwAuthorizedApplication auth = Activator.CreateInstance(type) as INetFwAuthorizedApplication;
            auth.Name = title;
            auth.ProcessImageFileName = applicationPath;
            auth.Scope = scope;
            auth.IpVersion = ipVersion;
            auth.Enabled = true;

            INetFwMgr manager = GetFirewallManager();
            try
            {
                manager.LocalPolicy.CurrentProfile.AuthorizedApplications.Add(auth);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }
    }
}

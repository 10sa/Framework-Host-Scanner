using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Module.Base;
using Framework.Enum;
using System.Net;

namespace Modules.Host.TCP
{
    public class DNSAnalysor : IVulnerableModuleBase
    {
		public string IVulnerableInfo { get; private set; }
		public string ModuleName { get { return "DNSAnalysor"; } }
        public string ModuleVer { get { return "1.0v"; } }


		public CallResult IVulnerableCheck(string address)
		{
			IPHostEntry Data = Dns.GetHostEntry(address);
			IVulnerableInfo = "DNS Analysor Result" + Environment.NewLine;
			IVulnerableInfo += "---------------------" + Environment.NewLine;

			foreach(var Address in Data.AddressList)
			{
				IVulnerableInfo += string.Format("| Address Protocol : {0} | IP : {1} | DNS Hostname : {2} |\n", Address.AddressFamily, Address.ToString(), Data.HostName) + Environment.NewLine;
			}

			return CallResult.Status;
		}
	}
}

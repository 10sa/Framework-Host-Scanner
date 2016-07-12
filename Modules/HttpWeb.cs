using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Module.Base;
using Framework.Enum;
using HtmlAgilityPack;

namespace Modules.HttpWeb
{
    /// <summary>
    /// CVE-2014-6271 :: ShellShock (Apapche HTTP Server User-Agent Attack) Patch : <http://ftp.gnu.org/gnu/bash/bash-4.3-patches/bash43-025>
    /// </summary>
    public class ShellShock : WebBase, IVulnerableModuleBase
    {
		/// <summary>
		/// ShellShock 취약점을 탐색하기 위한 헤더입니다.
		/// </summary>
		private const string Agent = "ShellShock: Vulnerable!";
		public string IVulnerableInfo { get; private set; }

		public string ModuleName { get { return "ShellShock Module"; } }
        public string ModuleVer { get { return "0.1v"; } }

		public CallResult IVulnerableCheck(string address)
        {
            // 초기 서버 주소를 설정합니다.
			try
			{
				ServerAddress = MakeUrl(address);
			}
			catch(Exception)
			{
				throw new UriFormatException("주소를 URI로 변경할수 없습니다!");
			}

            return ShellShockCheck();
        }

        /// <summary>
        /// 쉘쇼크 취약점을 검색하는 메소드입니다.
        /// </summary>
        /// <returns>보안 취약점이 있을시 true, 없을시 false를 반환합니다.</returns>
        private CallResult ShellShockCheck()
        {
            // 깊이 탐색 알고리즘 구현하기.
			// HTML 파싱이 필요함.
            RequestEx(true, true, true, GetRequestHeaders());
            if (ResponseHeader.Contains(Agent))
            {
                IVulnerableInfo = "서버에 쉘 쇼크 보안 취약점이 존재합니다.\n";
				return CallResult.Unsafe;
            }

			return CallResult.Safe;
        }

        /// <summary>
        /// 쉘쇼크 취약점에 필요한 헤더를 만들어 반환하는 메소드입니다.
        /// </summary>
        /// <returns>헤더 키/값 으로 이루어진 Headers 구조체입니다.</returns>
        private Headers GetRequestHeaders()
        {
            Headers Packet = new Headers();
            Packet.Key.Add("User-Agent");
            Packet.Value.Add("echo \"" + Agent + "\"");

            return Packet;
        }
	}

	public class VulnerableFileFinder : WebBase, IVulnerableModuleBase
	{
		public string IVulnerableInfo { get; private set; }
		public string ModuleName { get { return "Vulnerable File Finder"; } }
		public string ModuleVer { get { return "1.0v"; } }

		List<string> Links = new List<string>();

		public CallResult IVulnerableCheck(string address)
		{
			ServerAddress = MakeUrl(address);
			Request(true);

			if(Checking())
				return CallResult.Unsafe;

			return CallResult.Safe;
		}

		private bool Checking()
		{
			HtmlDocument Parsing = new HtmlDocument();
			Parsing.LoadHtml(ResponseEntity);

			// 깊이 탐색 알고리즘 필요
			foreach(var Tag in Parsing.DocumentNode.SelectNodes("//a[@href]"))
			{
				// 리다이렉트 링크가 원래 서버 주소와 일치하는지 검사. (만약 검사되지 않을시 무한반복할수 있음.)
				string temp = Tag.GetAttributeValue("href", string.Empty);
				if(temp.Contains(ServerAddress.AbsoluteUri))
				{
					Links.Add(temp);
				}

				foreach(var Link in Links)
				{
					Uri ParsingUri = new Uri(Link);

					if(ParsingUri.AbsolutePath.Contains("phpinfo.php"))
					{
						if(Parsing.DocumentNode.SelectSingleNode("//title").InnerText.Contains("phpinfo()"))
						{
							IVulnerableInfo = string.Format("Find a PHP Info File : {0}", Link);
							return true;
						}
					}

				}
			}

			return true;
		}
	}

	public class AllowsMethodChecker : WebBase, IVulnerableModuleBase
	{
		public string IVulnerableInfo { get; private set; }
		public string ModuleName { get { return "Allows Methods Checker"; } }
		public string ModuleVer { get { return "1.0v"; } }

		public CallResult IVulnerableCheck(string address)
		{
			throw new NotImplementedException();
		}
	}
}

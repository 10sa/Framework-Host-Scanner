using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Module.Base;

namespace Modules.HttpWeb
{
    /// <summary>
    /// CVE-2014-6271 :: ShellShock (Apapche HTTP Server User-Agent Attack) Patch : <http://ftp.gnu.org/gnu/bash/bash-4.3-patches/bash43-025>
    /// </summary>
    public class ShellShock : WebBase, IVulnerableModuleBase
    {
        /// <summary>
        /// 보안 취약점의 정보입니다.
        /// </summary>
        private string VPInfo;

        /// <summary>
        /// ShellShock 취약점을 탐색하기 위한 헤더입니다.
        /// </summary>
        private const string Agent = "ShellShock: Vulnerable!";


        public string ModuleName
        {
            get
            {
                return "ShellShock Module";
            }
        }


        public string ModuleVer
        {
            get
            {
                return "0.1v";
            }
        }

        public bool IVulnerableCheck(string address)
        {
            // 초기 서버 주소를 설정합니다.
			try
			{
				Server_Address = MakeUrl(address);
			}
			catch(Exception)
			{
				throw new UriFormatException("주소를 URI로 변경할수 없습니다!");
			}

            return ShellShockCheck();
        }

        public string IVulnerableInfo()
        {
            return VPInfo;
        }

        /// <summary>
        /// 쉘쇼크 취약점을 검색하는 메소드입니다.
        /// </summary>
        /// <returns>보안 취약점이 있을시 true, 없을시 false를 반환합니다.</returns>
        private bool ShellShockCheck()
        {
            // 깊이 탐색 알고리즘 구현하기.
			// HTML 파싱이 필요함.
            RequestEx(true, true, true, GetRequestHeaders());
            if (Response_Header.Contains(Agent))
            {
                VPInfo = "서버에 쉘 쇼크 보안 취약점이 존재합니다.\n";
                return true;
            }

            return false;
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
}

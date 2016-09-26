using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Module.Base;
using Framework.Enum;
using System.Net;

using HtmlAgilityPack;

namespace Modules.HttpWeb
{
    /// <summary>
    /// ShellShock 취약점을 탐색하기 위한 모듈 클래스입니다.
    /// </summary>
    public class ShellShock : WebBase, IVulnerableModuleBase
    {
        /// <summary>
        /// ShellShock 취약점을 탐색하기 위한 헤더입니다.
        /// </summary>
        private const string Agent = "ShellShock: Vulnerable";
        public string IVulnerableInfo { get; private set; }

        public string ModuleName { get { return "ShellShock Module"; } }
        public string ModuleVer { get { return "0.1v"; } }

        public IVulnerableOptions ModuleOptions { get { return IVulnerableOptions.AllPage; } }
        public object IOptionsAddData { get; set; }
        

        public CallResult IVulnerableCheck(string address)
        {
            List<Uri> ServerAllPages = (List<Uri>)IOptionsAddData;
            bool IsVulnerable = false;

            foreach(var Data in ServerAllPages)
            {
                try
                {
                    SetAddress(Data.AbsoluteUri);
                    RequestEx(true, true, true, GetRequestHeaders());

                    if(ResponseHeader.Contains(Agent))
                    {
                        IVulnerableInfo += string.Format("취약 페이지 : {0}\n", Data.AbsoluteUri);
                        IsVulnerable = true;
                    }
                }
                catch(WebException)
                {
                    continue;
                }
                catch(UriFormatException)
                {
                    continue;
                }
                catch(Exception)
                {
                    throw;
                }
            }

            if(IsVulnerable)
                return CallResult.Unsafe;
            else
                return CallResult.Safe;
                
        }

        /// <summary>
        /// 쉘쇼크 취약점에 필요한 헤더를 만들어 반환하는 메소드입니다.
        /// </summary>
        /// <returns>헤더 키/값 으로 이루어진 Headers 구조체입니다.</returns>
        private Headers GetRequestHeaders()
        {
            return new Headers("User-Agnet", "() {:;}; echo" + Agent);
        }

        protected sealed override void SetAddress(string Address)
        {
            HttpRequest = (HttpWebRequest)WebRequest.Create(Address);
            HttpRequest.UserAgent = Agent;
            HttpRequest.Method = "GET";
        }
    }

	/// <summary>
	/// 웹 서버 상에서 취약한 파일을 찾기 위해 만들어진 모듈입니다.
	/// </summary>
	public class VulnerableFileFinder : WebBase, IVulnerableModuleBase
	{
		public string IVulnerableInfo { get; private set; }
		public string ModuleName { get { return "Vulnerable File Finder"; } }
		public string ModuleVer { get { return "1.0v"; } }

        public IVulnerableOptions ModuleOptions { get { return IVulnerableOptions.Dictionary; } }
        public object IOptionsAddData { get; set; }
        private List<string> DefaultWords;

        private const int MaximumHope = 3;
        private int Hope = 0;

        public CallResult IVulnerableCheck(string address)
        {
            DefaultWords = (List<string>)IOptionsAddData;

            try
            {
                Requests(address);
            }
            catch(Exception)
            {
                throw;
            }

            return CallResult.Safe;
        }

        private void Requests(string address)
        {
            if(Hope >= MaximumHope)
            {
                Hope--;
                return;
            }
            else
                Hope++;

            try
            {
                foreach(var Word in DefaultWords)
                {
                    try
                    {
                        SetAddress(address + Word);
                        Request(true);

                        if(ResponseEntity != string.Empty && ResponseEntity != Environment.NewLine)
                        {
                            HtmlDocument Parsing = new HtmlDocument();
                            Parsing.Load(ResponseEntity);
                            string Title = Parsing.DocumentNode.SelectSingleNode("//title").InnerText;

                            if(Title.Contains("Index Of"))
                                IVulnerableInfo += string.Format("Index File : {0}", address);
                            else if(Title.Contains("phpinfo()"))
                            {
                                IVulnerableInfo += string.Format("phpinfo() File : {0}", address);
                            }
                        }

                        Requests(address);
                    }
                    catch(NullReferenceException) { continue; }
                    catch(ArgumentException) { continue; }
                    catch(WebException) { continue; }
                    catch(Exception)
                    {
                        throw;
                    }
                    
                }
            }
            catch(NullReferenceException)
            {
                return;
            }
            catch(Exception)
            {
                throw;
            }
            
            return;
        }

        protected sealed override void SetAddress(string Address)
        {
            HttpRequest = (HttpWebRequest)WebRequest.Create(MakeUri(Address));
            HttpRequest.Method = "GET";
        }
    } 

	/// <summary>
	/// 웹 서버가 허용한 메소드를 확인하는 모듈 클래스 입니다.
	/// </summary>
	public class AllowsMethodChecker : WebBase, IVulnerableModuleBase
	{
		public string IVulnerableInfo { get; private set; }

		public string ModuleName { get { return "Allows Methods Checker"; } }
        public string ModuleVer { get { return "1.0v"; } }

        public IVulnerableOptions ModuleOptions { get { return IVulnerableOptions.ServerOnly; } }
        public object IOptionsAddData { get; set; }


        public CallResult IVulnerableCheck(string address)
		{
			SetAddress(address);
			HttpRequest.Method = "OPTIONS";

			try
			{
				Request(false);
                foreach(var Allows in HttpRequest.Headers.GetValues("Allow"))
                {
                    if(Allows.Equals("PUT"))
                        IVulnerableInfo += "서버가 PUT 메소드를 사용 허가하고 있습니다.";
                    else if(Allows.Equals("DELETE"))
                        IVulnerableInfo += "서버가 DELETE 메소드를 사용 허가하고 있습니다.";
                    else if(Allows.Equals("TRACE"))
                        IVulnerableInfo += "서버가 TRACE 메소드를 사용 허가하고 있습니다.";
                }
            }
			catch (WebException exp)
			{
				IVulnerableInfo = "웹 서버에서 200번 상태코드 이외의 상태코드를 반환하였습니다." + Environment.NewLine;
				IVulnerableInfo += "Status : " + exp.Status.ToString();

                return CallResult.Exception;
			}
			catch (Exception exp)
			{
				IVulnerableInfo = exp.Message;
				return CallResult.Exception;
			}

			return CallResult.Status;
		}
	}
}

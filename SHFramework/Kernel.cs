using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

using HtmlAgilityPack;

using SHFramework.Module;
using SHFramework.Module.Base;
using SHFramework.Struct;
using SHFramework.Enum;

namespace SHFramework
{
	/// <summary>
	/// 서버의 취약점을 점검하는 프레임워크 클래스입니다.
	/// </summary>
	public class ScannerFramework
	{
		/// <summary>
		/// 모듈을 제어합니다.
		/// </summary>
		public moduleController moduleControll = new moduleController(true);


        /// <summary>
        /// VulnerablePointCheck 메소드 호출 후 모듈의 보고서가 저장되는 리스트입니다.
        /// </summary>
        public List<ModuleCallResult> Info { get; private set; } = new List<ModuleCallResult>();

        /// <summary>
        /// 목표 서버에 대한 취약점을 점검합니다.
        /// </summary>
        /// <param name="Address">목표 서버의 주소입니다.</param>
        public void VulnerablePointCheck(string Address)
		{
            WebLinkDFSearchClass dfSearcher = new WebLinkDFSearchClass();
            DictionaryCrafter dCrafter = null;
            
            Info.Clear();

            for (int i = 0; i < moduleControll.Lenght; i++)
			{
				try
				{
                    if ((moduleControll.Data[i].Status == ModuleStatus.Error) || (moduleControll.Data[i].Status == ModuleStatus.DontCall))
                        Info.Add(new ModuleCallResult(moduleControll.Data[i], CallResult.Exception, string.Empty));
                    else
                    {
                        if(moduleControll.Data[i].Module.ModuleOptions == IVulnerableOptions.AllPage)
                        {
                            if(dfSearcher.Address.Count == 0)
                                dfSearcher.GetAllEdgeLinks(Address);

                            moduleControll.Data[i].Module.IOptionsAddData = dfSearcher.Address;
                        }
                        else if (moduleControll.Data[i].Module.ModuleOptions == IVulnerableOptions.Dictionary)
                        {
                            if(dCrafter == null)
                                dCrafter = new DictionaryCrafter();

                            moduleControll.Data[i].Module.IOptionsAddData = dCrafter.DefaultWords;
                        }

                        Info.Add(new ModuleCallResult(moduleControll.Data[i], moduleControll.Data[i].Module.IVulnerableCheck(Address), moduleControll.Data[i].Module.IVulnerableInfo));
                    }
                }
                catch(ArgumentNullException)
                {
					throw;
                }
                catch (Exception)
				{
                    // 모듈의 예외 반환을 처리하기 위한 에러처리.
					Info.Add(new ModuleCallResult(moduleControll.Data[i], CallResult.Exception, moduleControll.Data[i].Module.IVulnerableInfo));
               }
			}

            return;
		}

	}

    /// <summary>
    /// 웹 페이지의 모든 페이지를 가져올때 사용되는 클래스입니다.
    /// </summary>
    public class WebLinkDFSearchClass : WebBase
    {
        /// <summary>
        /// 모든 리다이렉트 링크입니다.
        /// </summary>
        public List<Uri> Address = new List<Uri>();
        private int hope;
        private const int maximunHope = 8;

		/// <summary>
		/// DFS 검색에 맞는 HTTP 메소드 및 검색할 서버를 설정합니다.
		/// </summary>
		/// <param name="Address">검색할 서버 입니다.</param>
        protected sealed override void SetAddress(string Address)
        {
            HttpRequest = (HttpWebRequest)WebRequest.Create(MakeUri(Address));
            HttpRequest.UserAgent = "DFSPageScanner";
            HttpRequest.Method = "GET";
        }

        /// <summary>
        /// 웹 주소에 연결된 모든 리다이렉트 링크를 가져옵니다.
        /// </summary>
        /// <param name="Address">타겟의 웹 주소입니다.</param>
        /// <returns>Uri로 이루어진 연결된 모든 리다이렉트 링크입니다.</returns>
        public List<Uri> GetAllEdgeLinks(string Address)
        {
            if(hope >= maximunHope)
            {
                hope--;
                return null;
            }
            else
                hope++;
            
            SetAddress(Address);
            this.Address.Add(MakeUri(Address));
            Request(true);
            // System.Windows.Forms.MessageBox.Show(ResponseEntity);

            HtmlDocument Parsing = new HtmlDocument();
            Parsing.LoadHtml(ResponseEntity);

            try
            {
                foreach(var Tag in Parsing.DocumentNode.SelectNodes("//a[@href]"))
                {
                    try
                    {
                        Uri Link = MakeUri(Tag.GetAttributeValue("href", string.Empty));
                        GetAllEdgeLinks(Tag.GetAttributeValue("href", string.Empty));

                        // 컨피그 요소 고려.
                        if(!this.Address.Contains(Link) && Link.Authority.Contains(ServerAddress.Authority))
                            this.Address.Add(Link);
                    }
                    catch(UriFormatException)
                    {
                        ResponseHeader = null;
                        continue;
                    }
                    catch(NullReferenceException)
                    {
                        ResponseHeader = null;
                        continue;
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                }
            }
            catch(NullReferenceException)
            {
                return null;
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                ResponseHeader = null;
            }

            return this.Address;
        }
    }

    /// <summary>
    /// 사전 파일의 단어를 가져올때 사용되는 클래스입니다.
    /// </summary>
    public class DictionaryCrafter
    {
        /// <summary>
        /// 불러와진 사전 파일의 단어 목록입니다.
        /// </summary>
        public readonly List<string> DefaultWords = new List<string>();

        /// <summary>
        /// 사전 파일의 단어를 가져와 DefaultWords 리스트에 저장합니다.
        /// </summary>
        public DictionaryCrafter()
        {
            using(StreamReader Reader = new StreamReader(Environment.CurrentDirectory + "\\Dictionary.dic"))
            {
                while(true)
                {
                    if(Reader.EndOfStream)
                        break;

                    DefaultWords.Add(Reader.ReadLine());
                }
            }
        }
    }
}

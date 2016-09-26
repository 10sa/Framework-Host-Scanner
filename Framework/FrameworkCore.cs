using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Reflection;
using HtmlAgilityPack;

using Framework.Module;
using Framework.Module.Base;
using Framework.Struct;
using Framework.Enum;

namespace Framework
{
	/// <summary>
	/// 서버의 취약점을 점검하는 프레임워크 클래스입니다.
	/// </summary>
	public class ScannerFramework
	{
		/// <summary>
		/// 모듈을 제어합니다.
		/// </summary>
		public ModuleController ModuleControll = new ModuleController(true);


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
            WebLinkDFSearchClass DFSeacher = new WebLinkDFSearchClass();
            DictionaryCrafter DCrafter = null;
            
            Info.Clear();

            for (int i = 0; i < ModuleControll.Lenght; i++)
			{
				try
				{
                    if ((ModuleControll.Data[i].Status == ModuleStatus.Error) || (ModuleControll.Data[i].Status == ModuleStatus.DontCall))
                        Info.Add(new ModuleCallResult(ModuleControll.Data[i], CallResult.Exception, string.Empty));
                    else
                    {
                        if(ModuleControll.Data[i].Module.ModuleOptions == IVulnerableOptions.AllPage)
                        {
                            if(DFSeacher.Address.Count == 0)
                                DFSeacher.GetAllEdgeLinks(Address);

                            ModuleControll.Data[i].Module.IOptionsAddData = DFSeacher.Address;
                        }
                        else if (ModuleControll.Data[i].Module.ModuleOptions == IVulnerableOptions.Dictionary)
                        {
                            if(DCrafter == null)
                                DCrafter = new DictionaryCrafter();

                            ModuleControll.Data[i].Module.IOptionsAddData = DCrafter.DefaultWords;
                        }

                        Info.Add(new ModuleCallResult(ModuleControll.Data[i], ModuleControll.Data[i].Module.IVulnerableCheck(Address), ModuleControll.Data[i].Module.IVulnerableInfo));
                    }
                }
                catch(ArgumentNullException e)
                {
                    throw e;
                }
                catch (Exception)
				{
                    // 모듈의 예외 반환을 처리하기 위한 에러처리.
					Info.Add(new ModuleCallResult(ModuleControll.Data[i], CallResult.Exception, ModuleControll.Data[i].Module.IVulnerableInfo));
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
        private int Hope;
        private const int MaximunHope = 8;

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
            if(Hope >= MaximunHope)
            {
                Hope--;
                return null;
            }
            else
                Hope++;
            
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

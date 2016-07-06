using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.IO;

namespace Framework
{
	/// <summary>
	/// HTTP 헤더의 키와 값을 저장하기 위한 구조체입니다.
	/// </summary>
	public struct Headers
	{
		public List<string> Key;
		public List<string> Value;
	}


	/// <summary>
	/// HTTP 통신을 위한 상속 클래스입니다.
	/// </summary>
	public abstract class WebBase
	{
		/// <summary>
		/// 웹 서버와의 통신을 위한 상위 클래스입니다.
		/// </summary>
		protected HttpWebRequest Request_var { get; set; }
		/// <summary>
		/// 웹 서버의 URI 입니다.
		/// </summary>
		protected Uri Server_Address { get; set; }
		/// <summary>
		/// 서버에서 송신한 헤더입니다.
		/// </summary>
		public string Response_Header { get; protected set; }
		/// <summary>
		/// 서버에서 송신한 엔티티 본문입니다.
		/// </summary>
		public string Response_Entity { get; protected set; }

		/// <summary>
		/// 생성시 입력된 주소값으로 요청을 보냅니다.
		/// </summary>
		/// <param name="GetEntity">엔티티 본문을 Response_Entity 인스턴스에 저장할지에 대한 여부입니다.</param>
		public void Request(bool GetEntity)
		{
			try
			{
				Request_var.GetRequestStream();
			}
			catch (Exception exp)
			{
				throw exp;
			}
			
			WebResponse Response = Request_var.GetResponse();

			GetHeader(Response);
			if (GetEntity)
				this.GetEntity(Response);
		}

		// 오버로드로 구현할까?...
        /// <summary>
        /// Request 메서드와 동일한 기능을 제공하나 Request 메서드가 사용할 HTTP 헤더를 지정 합니다.
        /// </summary>
        /// <param name="GetEntitiy">엔티티 본문을 Response_Entitiy 인스턴스에 저장할지에 대한 여부입니다.</param>
        /// <param name="CallRequestMethod">인자로 전달받은 헤더의 키/값 쌍을 모두 헤더에 추가한 뒤 Request 메소드를 호출할지에 대한 여부입니다.</param>
        /// <param name="ClearAndAdd">기존에 있던 헤더 데이터를 초기화 한 뒤 추가할지에 대한 여부입니다.</param>
        /// <param name="Headers">헤더의 키와 값을 저장한 구조체입니다.</param>
        public void RequestEx(bool GetEntitiy, bool CallRequestMethod, bool ClearAndAdd, Headers Headers)
        {
			if ( Headers.Key.Count != Headers.Value.Count )
				throw new Exception("키 리스트와 값 리스트의 크기가 맞지 않습니다.");

            if (ClearAndAdd)
                RemoveAllHeaders();

            for(int i=0; i<=Headers.Key.Count; i++)
            {
                Request_var.Headers.Add(Headers.Key[i], Headers.Value[i]);
            }

            if(CallRequestMethod)
                Request(GetEntitiy);

            return;
        }

        /// <summary>
        /// 모든 헤더를 제거합니다.
        /// </summary>
        public void RemoveAllHeaders()
        {
            Request_var.Headers.Clear();
        }


        /// <summary>
        /// 파라매터로 넘겨받은 키 이름을 가진 헤더를 제거합니다.
        /// </summary>
        /// <param name="Name">지울 헤더의 키 이름입니다.</param>
        public void RemoveHeaders(string Name)
        {
            Request_var.Headers.Remove(Name);
        }

        /// <summary>
        /// HTTP 요청 후 서버측에서 반환하는 헤더를 Response_Header 인스턴스에 저장합니다.
        /// </summary>
        /// <param name="Response">서버측에서 반환하는 데이터를 포함한 WebResponse 클래스입니다.</param>
		private void GetHeader(WebResponse Response)
		{
			Response_Header = Response.Headers.ToString();

			return;
		}
        /// <summary>
        /// HTTP 요청 후 서버측에서 반환하는 엔티티 본문을 Response_Entitiy 인스턴스에 저장합니다.
        /// </summary>
        /// <param name="Response">서버측에서 반환하는 데이터를 포함한 WebResponse 클래스입니다.</param>
		private void GetEntity(WebResponse Response)
		{
            using (StreamReader Reader = new StreamReader(Response.GetResponseStream()))
            {
                Response_Entity = Reader.ReadToEnd();
            }

			return;
		}


		/// <summary>
		/// 스킴이 포함되지 않은 주소 문자열에 스킴을 추가하고 URI로 만들어 반환합니다.
		/// </summary>
		/// <param name="address">URI 주소입니다.</param>
		/// <returns>완성된 URI를 반환합니다.</returns>
		protected virtual Uri MakeUrl(string address)
		{
            // 이 부분을 고칠수 있을까?...
			try
			{
				return new Uri(address);
			}
			catch(UriFormatException)
			{
				try
				{
					return new Uri("http://" + address);
				}
				catch(Exception exp)
				{
					throw exp;
				}
			}
			catch(Exception exp)
			{
				throw exp;
			}
		}
    }
}

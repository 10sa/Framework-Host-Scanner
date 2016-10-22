using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace SHFramework.Module.Base
{
	/// <summary>
	/// HTTP 헤더의 키와 값을 저장하기 위한 구조체입니다.
	/// </summary>
	public struct headers
	{
		/// <summary>
		/// 헤더의 키값 입니다.
		/// </summary>
		public List<string> Key;

		/// <summary>
		/// 헤더의 값입니다.
		/// </summary>
		public List<string> Value;

		/// <summary>
		/// HTTP 헤더를 전달받은 인자로 초기화합니다.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
        public headers(string key, string value)
        {
            this.Key = new List<string> { key };
            this.Value = new List<string> { value };
        }
	}


	/// <summary>
	/// HTTP 통신을 위한 상속 클래스입니다.
	/// </summary>
	public abstract class WebBase
	{
		/// <summary>
		/// 웹 서버와의 통신을 위한 상위 클래스입니다.
		/// </summary>
		protected HttpWebRequest httpRequest { get; set; }
		/// <summary>
		/// 웹 서버의 URI 입니다.
		/// </summary>
		protected Uri ServerAddress { get { return httpRequest.Address; } }
		/// <summary>
		/// 서버에서 송신한 헤더입니다.
		/// </summary>
		public string ResponseHeader { get; protected set; }
		/// <summary>
		/// 서버에서 송신한 엔티티 본문입니다.
		/// </summary>
		public string ResponseEntity { get; protected set; }

		/// <summary>
		/// 주소를 설정하는 메소드입니다.
		/// </summary>
		/// <param name="Address">서버에 대한 주소입니다.</param>
		protected virtual void SetAddress(string address)
		{
            httpRequest = (HttpWebRequest)WebRequest.Create(MakeUri(address));
			httpRequest.Method = "HEAD";
            httpRequest.UserAgent = "Scanner Framework(0.1v);";
		}

		/// <summary>
		/// 생성시 입력된 주소값으로 요청을 보냅니다.
		/// </summary>
		/// <param name="getEntity">엔티티 본문을 Response_Entity 인스턴스에 저장할지에 대한 여부입니다.</param>
		public void Request(bool getEntity)
		{
            try
            {
                WebResponse response = httpRequest.GetResponse();
                GetHeader(response);
                if(getEntity)
                    this.GetEntity(response);
            }
            catch(Exception)
            {
                throw;
            }
		}


        /// <summary>
        /// Request 메서드와 동일한 기능을 제공하나 Request 메서드가 사용할 HTTP 헤더를 지정 합니다.
        /// </summary>
        /// <param name="getEntitiy">엔티티 본문을 Response_Entitiy 인스턴스에 저장할지에 대한 여부입니다.</param>
        /// <param name="callRequestMethod">인자로 전달받은 헤더의 키/값 쌍을 모두 헤더에 추가한 뒤 Request 메소드를 호출할지에 대한 여부입니다.</param>
        /// <param name="clearAndAdd">기존에 있던 헤더 데이터를 초기화 한 뒤 추가할지에 대한 여부입니다.</param>
        /// <param name="headers">헤더의 키와 값을 저장한 구조체입니다.</param>
        public void RequestEx(bool getEntitiy, bool callRequestMethod, bool clearAndAdd, headers headers)
        {
			if ( headers.Key.Count != headers.Value.Count )
				throw new Exception("키 리스트와 값 리스트의 크기가 맞지 않습니다.");

            if (clearAndAdd)
                RemoveAllheaders();

            for(int i=0; i<headers.Key.Count; i++)
            {
				httpRequest.Headers.Add(headers.Key[i], headers.Value[i]);
            }

            if(callRequestMethod)
                Request(getEntitiy);

            return;
        }

        /// <summary>
        /// 모든 헤더를 제거합니다.
        /// </summary>
        public void RemoveAllHeaders()
        {
			httpRequest.Headers.Clear();
        }


        /// <summary>
        /// 파라매터로 넘겨받은 키 이름을 가진 헤더를 제거합니다.
        /// </summary>
        /// <param name="Name">지울 헤더의 키 이름입니다.</param>
        public void RemoveHeaders(string name)
        {
			httpRequest.headers.Remove(name);
        }

        /// <summary>
        /// HTTP 요청 후 서버측에서 반환하는 헤더를 ResponseHeader 인스턴스에 저장합니다.
        /// </summary>
        /// <param name="Response">서버측에서 반환하는 데이터를 포함한 WebResponse 클래스입니다.</param>
		private void GetHeader(WebResponse response)
		{
			// MessageBox.Show(Response.headers.ToString());
			ResponseHeader = response.headers.ToString();

			return;
		}
        /// <summary>
        /// HTTP 요청 후 서버측에서 반환하는 엔티티 본문을 Response_Entitiy 인스턴스에 저장합니다.
        /// </summary>
        /// <param name="Response">서버측에서 반환하는 데이터를 포함한 WebResponse 클래스입니다.</param>
		private void GetEntity(WebResponse response)
		{
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                ResponseEntity = reader.ReadToEnd();
                // MessageBox.Show(ResponseEntity);
            }

			return;
		}


		/// <summary>
		/// 스킴이 포함되지 않은 주소 문자열에 스킴을 추가하고 URI로 만들어 반환합니다.
		/// </summary>
		/// <param name="address">URI 주소입니다.</param>
		/// <returns>완성된 URI를 반환합니다.</returns>
		protected virtual Uri MakeUri(string address)
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
				catch(Exception)
				{
					throw;
				}
			}
			catch(Exception)
			{
				throw;
			}
		}
    }
}

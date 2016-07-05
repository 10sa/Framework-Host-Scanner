using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScannerFramework
{
	/// <summary>
	/// 모듈을 실행시켰을때 반환하는 정보입니다.
	/// </summary>
	public struct VulnerablePointModuleInfo
	{
		/// <summary>
		/// 취약점 여부입니다.
		/// </summary>
		public List<bool> VulnerablePointStatus;


		/// <summary>
		/// 취약점의 정보입니다.
		/// </summary>
		public List<string> VulnerablePointInfo;


		/// <summary>
		/// 취약점 모듈의 이름입니다.
		/// </summary>
		public List<string> VulnerablePointName;


		/// <summary>
		/// 모듈의 버전입니다.
		/// </summary>
		public List<string> VulnerablePointModuleVer;
	}


	/// <summary>
	/// 모듈 로드시 발생한 에러 모듈의 에러 정보를 담는 구조체입니다.
	/// </summary>
	public struct ModuleLoadErrorList
	{
		/// <summary>
		/// 에러를 일으킨 모듈의 이름입니다.
		/// </summary>
		public List<string> ErrorModuleName;


		/// <summary>
		/// 에러의 정보입니다.
		/// </summary>
		public List<string> ErrorModuleInfo;
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Framework.Module.Base;
using Framework.Enum;

namespace Framework.Struct
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

	
	/// <summary>
	/// 불러온 모듈의 정보를 저장합니다.
	/// </summary>
	public struct ModuleData
	{
        /// <summary>
        /// 파라매터로 전달된 값으로 초기화합니다.
        /// </summary>
        /// <param name="Module">모듈의 인스턴스입니다.</param>
        /// <param name="Name">모듈의 이름입니다.</param>
        /// <param name="Info">모듈의 호출 정보입니다.</param>
		public ModuleData(IVulnerableModuleBase Module, string Name, ModuleStatus Info)
		{
            try
            {
                this.Module = Module;
                this.Name = Name;
                Status = Info;
            }
			catch(NullReferenceException)
            {
                throw new NullReferenceException("잘못된 값 입니다!");
            }
		}

        /// <summary>
		/// 호출할 모듈의 정보입니다.
		/// </summary>
		public IVulnerableModuleBase Module { get; private set; }

        /// <summary>
        /// 호출할 모듈의 이름입니다.
        /// </summary>
        public string Name;

        /// <summary>
        /// 모듈의 상태입니다.
        /// </summary>
        public ModuleStatus Status;
    }

    /// <summary>
    /// 모듈의 호출 결과와 정보를 저장하는 구조체입니다.
    /// </summary>
    public struct ModuleCallResult
    {
		/// <summary>
		/// 모듈의 정보입니다.
		/// </summary>
        public ModuleData Module { get; set; }

		/// <summary>
		/// 모듈의 호출 결과입니다.
		/// </summary>
        public CallResult Result { get; set; }

		/// <summary>
		/// 모듈의 호출후 정보입니다.
		/// </summary>
		public string Info { get; set; }

		/// <summary>
		/// 인스턴스를 호출 결과로 전달받은 파라매터로 초기화합니다.
		/// </summary>
		/// <param name="Module">호출한 모듈의 ModuleData 구조체입니다.</param>
		/// <param name="Result">모듈 호출의 결과입니다.</param>
		/// <param name="Info">모듈 호출의 정보입니다.</param>
        public ModuleCallResult(ModuleData Module, CallResult Result, string Info)
        {
            try
            {
				this.Module = Module;
				this.Result = Result;

				if (Info != null)
					this.Info = Info;
				else
					this.Info = null;
			}
            catch(NullReferenceException)
            {
                throw new NullReferenceException();
            }
        }
    }
}

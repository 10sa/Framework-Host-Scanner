using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Enum;

namespace Framework.Module.Base
{
    /// <summary>
    /// VulnerablePointChecker 프레임워크에 추가될 모듈의 인터페이스 입니다.
    /// 이 인터페이스를 상속한 클래스는 인자를 0개 사용하는 생성자를 포함하여야 합니다.
    /// </summary>
    public interface IVulnerableModuleBase
    {
        /// <summary>
        /// 모듈의 이름입니다.
        /// </summary>
		/// <returns>모듈의 이름입니다.</returns>
        string ModuleName { get; }

        /// <summary>
        /// 모듈의 버전입니다.
        /// </summary>
		/// <returns>모듈의 버전입니다.</returns>
        string ModuleVer { get; }

        /// <summary>
        /// 모듈의 옵션입니다.
        /// </summary>
        IVulnerableOptions ModuleOptions { get; }

        /// <summary>
        /// 옵션에 따라 추가로 설정되는 속성입니다.
        /// </summary>
        object IOptionsAddData { get; set; }

        /// <summary>
        /// 취약점이 있을시 추가되는 설명입니다.
        /// </summary>
        /// <returns>표시되는 설명입니다.</returns>
        string IVulnerableInfo { get; }

		/// <summary>
		/// 취약점 점검을 시작하는 인터페이스입니다. 프레임워크에서 이 메소드를 호출합니다.
		/// </summary>
		/// <param name="address">타겟 URI 주소입니다.</param>
		/// <returns>모듈의 반환 정보입니다.</returns>
		CallResult IVulnerableCheck(string address);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Reflection;

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
		/// 목표 서버에 대한 취약점을 점검합니다.
		/// </summary>
		/// <param name="Address">목표 서버의 주소입니다.</param>
		/// <returns>호출 결과를 저장한 리스트를 반환합니다.</returns>
		public List<CallResult> VulnerablePointCheck(string Address)
		{
			List<CallResult> Info = new List<CallResult>();

			for (int i = 0; i <= ModuleControll.Modules.Lenght; i++)
			{
				// 모듈 호출 여부에 따른 작동방식 (UI에서 쓰임)
				if (!(ModuleControll.Modules.IndexOf(i).Status == ModuleStatus.Call))
					continue;
                
				try
				{
					if (ModuleControll.Modules.IndexOf(i).Module.IVulnerableCheck(Address))
						Info.Add(CallResult.Unsafe);
				}
				catch (Exception)
				{
					// 모듈의 예외 반환을 처리하기 위한 에러처리.
					Info.Add(CallResult.Exception);
				}
			}

			return Info;
		}

	}
}

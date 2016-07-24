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
        /// VulnerablePointCheck 메소드 호출 후 모듈의 보고서가 저장되는 리스트입니다.
        /// </summary>
        public List<ModuleCallResult> Info { get; private set; } = new List<ModuleCallResult>();

        /// <summary>
        /// 목표 서버에 대한 취약점을 점검합니다.
        /// </summary>
        /// <param name="Address">목표 서버의 주소입니다.</param>
        public void VulnerablePointCheck(string Address)
		{
            Info.Clear();
			for (int i = 0; i <= ModuleControll.Lenght-1; i++)
			{
				// 호출구조 개선할것. (복잡함)
				try
				{
                    // DontCall이 구현되지 않은 관계로 생략.
                    if (ModuleControll.Data[i].Status != ModuleStatus.Error || ModuleControll.Data[i].Status == ModuleStatus.DontCall)
                        Info.Add(new ModuleCallResult(ModuleControll.Data[i], ModuleControll.Data[i].Module.IVulnerableCheck(Address), ModuleControll.Data[i].Module.IVulnerableInfo));
                    else
                    {
                        Info.Add(new ModuleCallResult(ModuleControll.Data[i], CallResult.Exception, "모듈에 에러가 있습니다."));
                    }
                        
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
}

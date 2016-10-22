using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Net;

using SHFramework.Struct;
using SHFramework.Enum;
using SHFramework.Module.Base;

namespace SHFramework.Module
{
	/// <summary>
	/// 모듈의 데이터를 저장하고 관리하는 클래스입니다.
	/// </summary>
	public class ModuleDataController
	{
		/// <summary>
		/// 모듈 데이터를 저장하는 리스트입니다.
		/// </summary>
		public List<ModuleData> Data { get; private set; } = new List<ModuleData>();

		/// <summary>
		/// 모듈 데이터를 저장하는 리스트의 길이입니다.
		/// </summary>
		public int Lenght { get { return Data.Count; } }

		/// <summary>
		/// 모듈 데이터를 저장한 리스트를 초기화합니다.
		/// </summary>
		public void Clear()
		{
			Data.Clear();
		}

		/// <summary>
		/// 모듈 데이터를 저장하는 리스트에 모듈 데이터를 추가합니다.
		/// </summary>
		/// <param name="Module">IVulnerableModuleBase 인터페이스를 상속한 모듈입니다.</param>
		/// <param name="Name">모듈의 이름입니다.</param>
		/// <param name="Flag">모듈의 상태입니다.</param>
		public void Add(IVulnerableModuleBase Module, string Name, ModuleStatus Flag)
		{
            Data.Add(new ModuleData(Module, Name, Flag));
		}

        /// <summary>
        /// 모듈의 상태를 설정합니다.
        /// </summary>
        /// <param name="index">상태를 설정할 모듈의 인덱스입니다.</param>
        /// <param name="Status">설정할 상태입니다.</param>
        public void SetModuleStatus(int index, ModuleStatus status)
        {
            Data.Insert(index, new ModuleData(Data[index].Module, Data[index].Name, status));
            Data.RemoveAt(index + 1);

            return;
        }
	}


	/// <summary>
	/// 모듈의 로드 및 관리를 담당하는 모듈 컨트롤러 클래스입니다.
	/// </summary>
	public class ModuleController : ModuleDataController
	{
        /// <summary>
        /// 모듈 컨트롤러 클래스를 초기화합니다.
        /// </summary>
        /// <param name="ModuleLoad">초기화에 동시에 모듈을 로드할지에 대한 여부입니다.</param>
        public ModuleController(bool moduleLoad)
        {
            if (moduleLoad)
                this.ModuleLoad();
        }

		/// <summary>
		/// 모듈을 다시 로드합니다.
		/// </summary>
		public void Reload()
		{
			Data.Clear();
			ModuleLoad();
		}

		/// <summary>
		/// 모듈을 추가합니다.
		/// </summary>
		/// <param name="Module">추가될 모듈입니다.</param>
		protected void AddVulnerablePointCheckModule(IVulnerableModuleBase module)
		{
			Add(module, module.ModuleName, ModuleStatus.Call);
		}

		/// <summary>
		/// 모듈이 비정상적일시 모듈 로드를 에러로 처리하여 추가하는 메소드입니다.
		/// </summary>
		/// <param name="Module">추가될 모듈입니다.</param>
		/// <param name="Name">모듈의 이름입니다.</param>
		public void AddVulnerablePointCheckModule(IVulnerableModuleBase module, string name)
		{
			Add(module, name, ModuleStatus.Error);
		}

		/// <summary>
		/// 외부 Dll를 로드합니다.
		/// </summary>
		/// <returns>로드된 정보를 자동으로 저장합니다.</returns>
		public void ModuleLoad()
		{
			// 폴더가 있는지 확인.
			if (!Directory.Exists(Environment.CurrentDirectory + @"\Module"))
			{
				Directory.CreateDirectory(Environment.CurrentDirectory + @"\Module");
				return;
			}

			// 폴더가 있을시 폴더에 있는 dll 확장자를 가진 모든 파일을 로드함.
			DirectoryInfo moduleFoler = new DirectoryInfo(Environment.CurrentDirectory + @"\Module");
			FileInfo[] modulesInfo = moduleFoler.GetFiles("*.dll", SearchOption.TopDirectoryOnly);

			// Dll 파일 로드.
			foreach (var module in modulesInfo)
			{
				// LoadFile 메소드는 절대 경로를 파라메터로 넘겨야 함. (상대경로 X)
				Assembly dllLoader = Assembly.LoadFile(module.FullName);
				Type[] moduleClasses = dllLoader.GetExportedTypes();

				foreach (var moduleClass in moduleClasses)
				{
					// 클래스 여부, 인스턴스 생성 가능여부 점검.
					if (Class.IsClass && !Class.IsAbstract)
					{
						// IVulnerableModuleBase의 여부 점검.
						if (typeof(IVulnerableModuleBase).IsAssignableFrom(moduleClass))
						{
							IVulnerableModuleBase temp = (IVulnerableModuleBase)Activator.CreateInstance(moduleClass);

							try
							{
								// 모듈 이름의 정상 호출 여부 확인.
								string name = temp.ModuleName;
								string ver = temp.ModuleVer;

                                // 오버헤드를 감수하고 모듈의 정상 여부를 검사함.
                                // 만약 모듈이 정상적으로 구현되지 않았다면 NotImplementedException 예외를 반환할 것임.
                                if(temp.ModuleOptions == IVulnerableOptions.AllPage)
                                    temp.IOptionsAddData = new List<Uri> { new Uri("http://localhost") };
                                else if(temp.ModuleOptions == IVulnerableOptions.Dictionary)
                                    temp.IOptionsAddData = new List<string> { "Test" };

								temp.IVulnerableCheck("localhost");
                                temp.IOptionsAddData = null;

								// 정상적으로 로드되었을 경우, 추가.
								AddVulnerablePointCheckModule(temp);
							}
							catch(WebException)
							{
								// 주소 자체가 localhost 이기 때문에 충분히 예외 발생이 가능함.
								AddVulnerablePointCheckModule(temp);
							}
							catch(Exception)
							{
								// 인터페이스를 상속하는 클래스를 찾았으나 인터페이스 메소드가 제대로 정의되지 않았을 경우 예외처리함.
								// Overload 된 메소드 자체가 에러 처리를 위한 메소드임.
								AddVulnerablePointCheckModule(temp, temp.ModuleName);
							}
						}

					}

				}

			}

		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;
using System.Net;

using Framework.Struct;
using Framework.Enum;
using Framework.Module.Base;

namespace Framework.Module
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
        public void SetModuleStatus(int index, ModuleStatus Status)
        {
            Data.Insert(index, new ModuleData(Data[index].Module, Data[index].Name, Status));
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
        public ModuleController(bool ModuleLoad)
        {
            if (ModuleLoad)
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
		protected void AddVulnerablePointCheckModule(IVulnerableModuleBase Module)
		{
			Add(Module, Module.ModuleName, ModuleStatus.Call);
		}

		/// <summary>
		/// 모듈이 비정상적일시 모듈 로드를 에러로 처리하여 추가하는 메소드입니다.
		/// </summary>
		/// <param name="Module">추가될 모듈입니다.</param>
		/// <param name="Name">모듈의 이름입니다.</param>
		public void AddVulnerablePointCheckModule(IVulnerableModuleBase Module, string Name)
		{
			Add(Module, Name, ModuleStatus.Error);
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
			DirectoryInfo ModuleFoler = new DirectoryInfo(Environment.CurrentDirectory + @"\Module");
			FileInfo[] ModulesInfo = ModuleFoler.GetFiles("*.dll", SearchOption.TopDirectoryOnly);

			// Dll 파일 로드.
			foreach (var Module in ModulesInfo)
			{
				// LoadFile 메소드는 절대 경로를 파라메터로 넘겨야 함. (상대경로 X)
				Assembly Dll_Loader = Assembly.LoadFile(Module.FullName);
				Type[] ModuleClass = Dll_Loader.GetExportedTypes();

				foreach (var Class in ModuleClass)
				{
					// 클래스 여부, 인스턴스 생성 가능여부 점검.
					if (Class.IsClass && !Class.IsAbstract)
					{
						// IVulnerableModuleBase의 여부 점검.
						if (typeof(IVulnerableModuleBase).IsAssignableFrom(Class))
						{
							IVulnerableModuleBase Temp = (IVulnerableModuleBase)Activator.CreateInstance(Class);

							try
							{
								// 모듈 이름의 정상 호출 여부 확인.
								string name = Temp.ModuleName;
								string ver = Temp.ModuleVer;

                                // 오버헤드를 감수하고 모듈의 정상 여부를 검사함.
                                // 만약 모듈이 정상적으로 구현되지 않았다면 NotImplementedException 예외를 반환할 것임.
                                if(Temp.ModuleOptions == IVulnerableOptions.AllPage)
                                    Temp.IOptionsAddData = new List<Uri> { new Uri("http://localhost") };
                                else if(Temp.ModuleOptions == IVulnerableOptions.Dictionary)
                                    Temp.IOptionsAddData = new List<string> { "Test" };

								Temp.IVulnerableCheck("localhost");
                                Temp.IOptionsAddData = null;

								// 정상적으로 로드되었을 경우, 추가.
								AddVulnerablePointCheckModule(Temp);
							}
							catch (WebException)
							{
								// 주소 자체가 localhost 이기 때문에 충분히 예외 발생이 가능함.
								AddVulnerablePointCheckModule(Temp);
							}
							catch(Exception)
							{
								// 인터페이스를 상속하는 클래스를 찾았으나 인터페이스 메소드가 제대로 정의되지 않았을 경우 예외처리함.
								// Overload 된 메소드 자체가 에러 처리를 위한 메소드임.
								AddVulnerablePointCheckModule(Temp, Temp.ModuleName);
							}
						}

					}

				}

			}

		}
	}
}

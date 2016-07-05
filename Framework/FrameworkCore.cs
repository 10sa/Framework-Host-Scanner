using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Reflection;

namespace ScannerFramework
{
    
    // Framework To Do List
    // 1. 완성하기.
    // 2. Dll 동적 로드 구현. (완료)
    // 2-1. Dll 동적 로드 후 유효성 검사. (완료됨)
    // 3. 캡슐화. (진행중)
    // 4. 디버깅 (진행중)
    // 모듈 추가.


    /// <summary>
    /// 서버의 취약점을 점검하는 프레임워크 클래스입니다.
    /// </summary>
    public class ScannerFramework
    {
		/// <summary>
		/// 호출할 모듈의 리스트입니다.
		/// </summary>
		private List<IVulnerableModuleBase> Modules = new List<IVulnerableModuleBase>();

        /// <summary>
        /// 호출할 모듈의 이름입니다.
        /// </summary>
        private List<string> ModulesName = new List<string>();

		/// <summary>
		/// 취약점을 점검할 서버의 URI 입니다.
		/// </summary>
		public string TargetServerURI { get; private set; }

        /// <summary>
        /// 모듈 로드시 발생한 에러를 저장하는 VulnerablePointModuleLoadErrorList 입니다.
        /// </summary>
        public ModuleLoadErrorList ErrorModuleList { get; private set; }


        /// <summary>
        /// 프레임워크를 초기화합니다.
        /// </summary>
        public ScannerFramework()
        {
            ErrorModuleList = ModuleLoad();
        }

		/// <summary>
		/// 모듈을 다시 로드합니다.
		/// </summary>
		public void Reload()
		{
			Modules.Clear();
			ErrorModuleList = ModuleLoad();
		}

		/// <summary>
		/// 모듈을 추가합니다.
		/// </summary>
		/// <param name="Module">추가될 모듈입니다.</param>
        public void AddVulnerablePointCheckModule(IVulnerableModuleBase Module)
        {
			Modules.Add(Module);
            ModulesName.Add(Module.ModuleName);
        }

        /// <summary>
        /// 모듈 이름 메소드가 정상적으로 구현되지 않았을 경우, 파라매터로 넘겨받은 이름을 모듈의 이름으로 설정합니다.
        /// </summary>
        /// <param name="Module">추가될 모듈입니다.</param>
        /// <param name="Name">모듈의 이름입니다.</param>
        public void AddVulnerablePointCheckModule(IVulnerableModuleBase Module, string Name)
        {
            Modules.Add(Module);
            ModulesName.Add(Name);
        }
        
        /// <summary>
        /// 목표 서버에 대한 취약점을 점검합니다.
        /// </summary>
		/// <param name="Address">목표 서버의 주소입니다.</param>
        /// <returns>취약점의 여부와 상태, 모듈의 정보를 포함한 리스트를 반환합니다.</returns>
        public VulnerablePointModuleInfo VulnerablePointCheck(string Address)
        {
            VulnerablePointModuleInfo Info = new VulnerablePointModuleInfo();
			TargetServerURI = Address;

            for (int i = 0; i <= Modules.Count; i++)
			{
                try
                {
                    Info.VulnerablePointName.Add(ModulesName[i]);
                    Info.VulnerablePointModuleVer.Add(Modules[i].ModuleVer);
                    Info.VulnerablePointStatus.Add(Modules[i].IVulnerableCheck(TargetServerURI));
                    if (Info.VulnerablePointStatus[i])
                        Info.VulnerablePointInfo.Add(Modules[i].IVulnerableInfo());
                }
                catch(NotImplementedException)
                {
                    // 이 부분을 줄일수 있을듯 함.
                    // 모듈의 비정상적인 구현을 점검하기 위한 에러처리.
                    Info.VulnerablePointModuleVer[i] = "";
                    Info.VulnerablePointStatus[i] = false;
                    Info.VulnerablePointInfo[i] = "모듈이 정상적으로 구현되지 않았습니다.";
                }
                catch(Exception exp)
                {
                    // 모듈의 예외 반환을 처리하기 위한 에러처리.
                    Info.VulnerablePointModuleVer[i] = "";
                    Info.VulnerablePointStatus[i] = false;
                    Info.VulnerablePointInfo[i] = "모듈이 예외를 반환했습니다.\n" + exp.Message;
                }
			}

            return Info;
        }



        // 로드에 성공한 모듈의 메소드 유효성을 검사하는 코드를 추가해야 함. (완료)

        /// <summary>
        /// 외부 Dll를 로드합니다.
        /// </summary>
        /// <returns>ModuleLoadErrorList 구조체입니다.</returns>
		public ModuleLoadErrorList ModuleLoad()
		{
            // dll에 인터페이스를 상속하는 클래스가 2개 이상일때 로드되는것을 방지하기 위한 bool 형 변수.
            bool isFinedModuleClass = false;
            Type Temp_Class = null;
            ModuleLoadErrorList Error_List = new ModuleLoadErrorList();

            // 폴더가 있는지 확인.
            if (!Directory.Exists("/Module"))
                Directory.CreateDirectory("/Module");

            // 폴더가 있을시 폴더에 있는 dll 확장자를 가진 모든 파일을 로드함.
			DirectoryInfo ModuleFoler = new DirectoryInfo("/Module");
			FileInfo[] ModulesInfo = ModuleFoler.GetFiles("*.dll", SearchOption.TopDirectoryOnly);

            // Dll 파일 로드.
            foreach(var Module in ModulesInfo)
            {
                // LoadFile 메소드는 절대 경로를 파라메터로 넘겨야 함. (상대경로 X)
                Assembly Dll_Loader = Assembly.LoadFile(Module.FullName);
                Type[] ModuleClass = Dll_Loader.GetExportedTypes();
                
                foreach(var Class in ModuleClass)
                {
                    // 클래스 여부 검사.
                    if(Class.IsClass)
                    {
                        // IVulnerableModuleBase의 여부 점검.
                        // 이 부분은 IVulnerableModuleBase 인터페이스를 상속하는 클래스가 2개 이상인지에 대한 여부를 검색하는 코드가 포함되어 있음.
                        if (typeof(IVulnerableModuleBase).IsAssignableFrom(Class))
                        {
                            if (!isFinedModuleClass)
                            {
                                Temp_Class = Class;
                                isFinedModuleClass = true;
                            }
                            else
                            {
                                Error_List.ErrorModuleName.Add(Module.Name);
                                Error_List.ErrorModuleInfo.Add("인터페이스를 상속하는 클래스가 2개 이상입니다.");
                            }                                
                        }
                    }
                }

                if(!isFinedModuleClass)
                {
                    Error_List.ErrorModuleName.Add(Module.Name);
                    Error_List.ErrorModuleInfo.Add("인터페이스를 상속하는 클래스가 없습니다.");
                }
                else
                {
                    IVulnerableModuleBase temp = (IVulnerableModuleBase)Activator.CreateInstance(Temp_Class);
                    
                    try
                    {
                        // 모듈 이름의 정상 호출 여부 확인.
                        string name = temp.ModuleName;
                        string ver = temp.ModuleVer;

                        // 정상적으로 로드되었을 경우, 추가.
                        AddVulnerablePointCheckModule(temp);
                    }
                    catch(NotImplementedException)
                    {
                        // 모듈이 정상적으로 불러와 졌으나, ModuleName 메소드가 제대로 구현되지 않았을 경우 발생하는 예외처리.
                        AddVulnerablePointCheckModule(temp, Module.Name);
                    }
                }
            }

            return Error_List;
        }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Reflection;

namespace Framework
{

    // Framework To Do List
    // 1. 완성하기.
    // 2. Dll 동적 로드 구현. (완료)
    // 2-1. Dll 동적 로드 후 유효성 검사. (완료됨)
    // 3. 캡슐화. (진행중)
    // 4. 디버깅 (진행중)
    // 모듈 추가.

    
    /// <summary>
    /// 모듈의 데이터를 저장하고 관리하는 클래스입니다.
    /// </summary>
    public class ModuleData
    {
        // 구조체의 이름 변경과 Struct 파일로 이전할것.
        public struct MetaData
        {
            /// <summary>
            /// 호출할 모듈의 리스트입니다.
            /// </summary>
            public IVulnerableModuleBase Module { get; private set; }

            /// <summary>
            /// 호출할 모듈의 이름입니다.
            /// </summary>
            private string Name;

            /// <summary>
            /// 모듈의 상태입니다.
            /// </summary>
            public ModuleStatus Status;

            public MetaData(IVulnerableModuleBase Module, string Name, ModuleStatus Info)
            {
                this.Module = Module;
                this.Name = Name;
                Status = Info;
            }
        }

        private List<MetaData> Data = new List<MetaData>();

        public int Lenght { get { return Data.Count; } }

        public MetaData IndexOf(int index)
        {
            return Data[index];
        }

        public void Clear()
        {
            Data.Clear();
        }

        public void Add(IVulnerableModuleBase Module, string Name, ModuleStatus Flag)
        {
            Data.Add(new MetaData(Module, Name, Flag));
        }
    }


    /// <summary>
    /// 서버의 취약점을 점검하는 프레임워크 클래스입니다.
    /// </summary>
    public class ScannerFramework
    {
		/// <summary>
		/// 취약점을 점검할 서버의 URI 입니다.
		/// </summary>
		public string TargetServerURI { get; private set; }

        /// <summary>
        /// 로드된 모듈의 정보입니다.
        /// </summary>
        public ModuleData Modules { get; private set; } = new ModuleData();

        /// <summary>
        /// 프레임워크를 초기화합니다.
        /// </summary>
        public ScannerFramework()
        {
            ModuleLoad();
        }

		/// <summary>
		/// 모듈을 다시 로드합니다.
		/// </summary>
		public void Reload()
		{
			Modules.Clear();
			ModuleLoad();
		}

		/// <summary>
		/// 모듈을 추가합니다.
		/// </summary>
		/// <param name="Module">추가될 모듈입니다.</param>
        public void AddVulnerablePointCheckModule(IVulnerableModuleBase Module)
        {
			Modules.Add(Module, Module.ModuleName, ModuleStatus.Call);
        }

        /// <summary>
        /// 모듈이 비정상적일시 모듈 로드를 에러로 처리하여 추가하는 메소드입니다.
        /// </summary>
        /// <param name="Module">추가될 모듈입니다.</param>
        /// <param name="Name">모듈의 이름입니다.</param>
        public void AddVulnerablePointCheckModule(IVulnerableModuleBase Module, string Name)
        {
            Modules.Add(Module, Name, ModuleStatus.Error);
        }
        

        /// <summary>
        /// 목표 서버에 대한 취약점을 점검합니다.
        /// </summary>
		/// <param name="Address">목표 서버의 주소입니다.</param>
        /// <returns>호출 결과를 저장한 리스트를 반환합니다.</returns>
        public List<CallResult> VulnerablePointCheck(string Address)
        {
            List<CallResult> Info = new List<CallResult>();
			TargetServerURI = Address;

            for (int i = 0; i <= Modules.Lenght; i++)
			{
                // 모듈 호출 여부에 따른 작동방식 (UI에서 쓰임)
                if (!(Modules.IndexOf(i).Status == ModuleStatus.Call))
                    continue;

                try
                {
                    if(Modules.IndexOf(i).Module.IVulnerableCheck(TargetServerURI))
                        Info.Add(CallResult.Unsafe);
                }
                catch(Exception)
                {
                    // 모듈의 예외 반환을 처리하기 위한 에러처리.
                    Info.Add(CallResult.Exception);
                }
			}

            return Info;
        }


        /// <summary>
        /// 외부 Dll를 로드합니다.
        /// </summary>
        /// <returns>ModuleLoadErrorList 구조체입니다.</returns>
		private void ModuleLoad()
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
            foreach(var Module in ModulesInfo)
            {
                // LoadFile 메소드는 절대 경로를 파라메터로 넘겨야 함. (상대경로 X)
                Assembly Dll_Loader = Assembly.LoadFile(Module.FullName);
                Type[] ModuleClass = Dll_Loader.GetExportedTypes();

                foreach (var Class in ModuleClass)
                {
                    // 클래스 여부 검사.
                    if(Class.IsClass)
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
                                Temp.IVulnerableCheck("TEST_Address");
                                Temp.IVulnerableInfo();

                                // 정상적으로 로드되었을 경우, 추가.
                                AddVulnerablePointCheckModule(Temp);
                            }
                            catch (NotImplementedException)
                            {
                                // 인터페이스를 상속하는 클래스를 찾았으나 인터페이스 메소드가 제대로 정의되지 않았을 경우 예외처리함.
                                // Overload 된 메소드 자체가 에러 처리를 위한 메소드임.
                                AddVulnerablePointCheckModule(Temp, Module.Name);
                            }

                        }

                    }

                }

            }

        }
    }
}

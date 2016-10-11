using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;


using Framework.fw_interfaces;

namespace Framework
{
	/// <summary>
	/// Framework Module Part Class.
	/// </summary>
	public class fw_module
	{
		private readonly string ModuleDirPath;

		/// <summary>
		/// Initlizing Class.
		/// </summary>
		public fw_module()
		{
			ModuleDirPath = Environment.CurrentDirectory + @"\Module";
		}

		/// <summary>
		/// Load Module.
		/// </summary>
		/// <returns>Module Instances.</returns>
		public List<module_interface> Load()
		{
			List<module_interface> Modules = new List<module_interface>();
			DirectoryInfo Dir = new DirectoryInfo(ModuleDirPath);

			foreach(var Data in Dir.GetFiles("*.dll"))
			{
				try
				{
					Assembly ModuleLoad = Assembly.LoadFile(Data.FullName);
					int LoadCount = 0;

					foreach (var ModuleType in ModuleLoad.GetExportedTypes())
					{
						if (ModuleType.IsAssignableFrom(typeof(module_interface)) && ModuleType.IsPublic && ModuleType.IsClass && !ModuleType.IsAbstract)
						{
							try
							{
								Modules.Add((module_interface)Activator.CreateInstance(ModuleType));
								LoadCount++;
							}
							catch(MissingMethodException)
							{
								fw_kernel.WriteErrorReport(string.Format("Module Load Error | Missing Constructor | Dll : {0} | Class : {1}", ModuleType.FullName, ModuleType.Name));
							}
						}
					}

					if (LoadCount == 0)
						fw_kernel.WriteErrorReport("Module Load Error | Not Found Assiganble Method | " + ModuleLoad.FullName);

				}
				catch(BadImageFormatException exc)
				{
					fw_kernel.WriteErrorReport("Module Load Error | Bad Format Dll | " + exc.FileName);
					continue;
				}
			}

			return Modules;
		}
	}
}

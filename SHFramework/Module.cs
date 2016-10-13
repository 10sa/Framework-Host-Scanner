using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

using SHFramework.Interfaces;

namespace SHFramework
{
	/// <summary>
	/// Framework Module Part Class.
	/// </summary>
	public class ModuleLoadControll
	{
		/// <summary>
		/// Module Folder Name.
		/// </summary>
		private const string ModuleFolderName = "Module";

		/// <summary>
		/// Module Load Format.
		/// </summary>
		private const string ModuleLoadFormat = "*.dll";

		private const string ErrorReportFormat = "Module Load Error | {0] | {1} ";
		private const string BadFormatDll = "Bad Format Dll.";
		private const string MissingConstructor = "Missing Constructor.";
		private const string NotFoundAssiganbleMethod = "Not Found Assiganble Method.";

		/// <summary>
		/// Return Module Folder Path.
		/// </summary>
		public string ModuleDirPath
		{
			get
			{
				return Environment.CurrentDirectory + @"\" + ModuleFolderName;
			}
		}

		/// <summary>
		/// The loaded module List.
		/// </summary>
		public List<module_interface> Modules { get; private set; } = new List<module_interface>();

		/// <summary>
		/// Load Module.
		/// </summary>
		/// <returns>Module Instances.</returns>
		public List<module_interface> Load()
		{
			foreach(var ModuleFile in GetModuleFiles())
			{
				try
				{
					int LastCount = Modules.Count;
					AddModules(GetModuleClass(ModuleFile.FullName));

					if (LastCount != Modules.Count)
						Kernel.WriteErrorReport(string.Format(ErrorReportFormat, NotFoundAssiganbleMethod, ModuleFile.FullName));
				}
				// Ignore Exceptions. 
				catch (BadImageFormatException) { }
			}

			return Modules;
		}

		/// <summary>
		/// Add Module Class.
		/// </summary>
		/// <param name="Classes">Module Class.</param>
		private void AddModules(Type[] Classes)
		{
			foreach(var Class in Classes)
			{
				if(CheckClassType(Class))
				{
					try
					{
						Modules.Add((module_interface)Activator.CreateInstance(Class));
					}
					catch (MissingMethodException)
					{
						Kernel.WriteErrorReport(string.Format(ErrorReportFormat, MissingConstructor, Class.FullName));
					}
				}
			}
		}

		/// <summary>
		/// Return Module Files.
		/// </summary>
		/// <returns>Module Files Info.</returns>
		private FileInfo[] GetModuleFiles()
		{
			DirectoryInfo Dir = new DirectoryInfo(ModuleDirPath);
			return Dir.GetFiles(ModuleLoadFormat);
		}

		/// <summary>
		/// Load Module Class.
		/// </summary>
		/// <param name="FileAbsolutePath">Module File Absolute Path</param>
		/// <returns>Classes Extract From the Module.</returns>
		private Type[] GetModuleClass(string FileAbsolutePath)
		{
			try
			{
				return Assembly.LoadFile(FileAbsolutePath).GetExportedTypes();
			}
			catch (BadImageFormatException exc)
			{
				Kernel.WriteErrorReport(string.Format(ErrorReportFormat, BadFormatDll, exc.FileName));
				throw;
			}
			
		}

		/// <summary>
		/// Validity Check Of The Class
		/// </summary>
		/// <param name="Class">Class To Be Checked</param>
		/// <returns>Validity Of The Class.</returns>
		private bool CheckClassType(Type Class)
		{
			if (Class.IsAssignableFrom(typeof(module_interface)) && Class.IsPublic && !Class.IsAbstract)
				return true;
			else
				return false;
		}
	}
}

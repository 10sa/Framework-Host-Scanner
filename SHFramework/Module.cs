using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

using SHFramework.Module.Interfaces;

namespace SHFramework.Module
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
		/// The Last Loaded Module List.
		/// </summary>
		public List<ModuleData> Modules { get; private set; } = new List<ModuleData>();

		/// <summary>
		/// Load Module.
		/// </summary>
		/// <returns>Module Instances.</returns>
		public List<ModuleData> Load()
		{
			foreach(var moduleFile in GetModuleFiles())
			{
				try
				{
					int lastCount = Modules.Count;

					foreach(var _class in CreateModules(GetModuleClass(moduleFile.FullName)))
					{
						Modules.Add(new ModuleData(_class, moduleFile.Name, moduleFile.FullName));
					}

					if (lastCount != Modules.Count)
						FrameworkKernel.ErrorReport(string.Format(ErrorReportFormat, NotFoundAssiganbleMethod, moduleFile.FullName));
				}
				// Ignore Exceptions. 
				catch (BadImageFormatException) { continue; }
			}

			return Modules;
		}

		/// <summary>
		/// Add Module Class.
		/// </summary>
		/// <param name="classes">Module Class.</param>
		private IModuleBase[] CreateModules(Type[] classes)
		{
			List<IModuleBase> moduleClasses = new List<IModuleBase>();

			foreach(var _class in classes)
			{
				if(CheckClassType(_class))
				{
					try
					{
						moduleClasses.Add((IModuleBase)Activator.CreateInstance(_class));
					}
					catch (MissingMethodException)
					{
						FrameworkKernel.ErrorReport(string.Format(ErrorReportFormat, MissingConstructor, _class.FullName));
					}
				}
			}

			return moduleClasses.ToArray();
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
		/// <param name="fileAbsolutePath">Module File Absolute Path</param>
		/// <returns>Classes Extract From the Module.</returns>
		private Type[] GetModuleClass(string fileAbsolutePath)
		{
			try
			{
				return Assembly.LoadFile(fileAbsolutePath).GetExportedTypes();
			}
			catch (BadImageFormatException exc)
			{
				FrameworkKernel.ErrorReport(string.Format(ErrorReportFormat, BadFormatDll, exc.FileName));
				throw;
			}
			
		}

		/// <summary>
		/// Validity Check Of The Class.
		/// </summary>
		/// <param name="Class">Class To Be Checked</param>
		/// <returns>Validity Of The Class.</returns>
		private bool CheckClassType(Type Class)
		{
			if (Class.IsAssignableFrom(typeof(IModuleBase)) && Class.IsPublic && !Class.IsAbstract)
				return true;
			else
				return false;
		}
	}

	public struct ModuleData
	{
		public IModuleBase Module;
		public string FileName;
		public string AbsolutPath;

		public ModuleData(IModuleBase module, string fileName, string absolutPath)
		{
			this.Module = module;
			this.FileName = fileName;
			this.AbsolutPath = absolutPath;

			return;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Windows.Forms;

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

		private const string ErrorReportFormat = "Module Load Error | {0} | {1} ";

		private const string BadFormatDll = "Bad Format Dll.";
		private const string MissingConstructor = "Missing Constructor.";
		private const string NotFoundAssiganbleMethod = "Not Found Assiganble Method.";

		/// <summary>
		/// Return Module Folder Path.
		/// </summary>
		public string ModulePath
		{
			get
			{
				return Environment.CurrentDirectory + @"\" + ModuleFolderName;
			}
		}

		/// <summary>
		/// The Last Loaded Module List.
		/// </summary>
		public ModuleData[] Modules { get; private set; }

		/// <summary>
		/// Load Module.
		/// </summary>
		/// <returns>Module Instances.</returns>
		public ModuleData[] Load()
		{
			List<ModuleData> moduleData = new List<ModuleData>();

			foreach(var moduleFile in GetModuleFiles())
			{
				try
				{
					int lastCount = moduleData.Count;

					foreach(var classInstances in CreateModuleInstances(GetModuleClass(moduleFile.FullName)))
					{
						moduleData.Add(new ModuleData(classInstances, moduleFile.Name, moduleFile.FullName));
					}

					if (lastCount != moduleData.Count)
						FrameworkKernel.ErrorReport(string.Format(ErrorReportFormat, NotFoundAssiganbleMethod, moduleFile.FullName));
				}
				// Ignore Exceptions. 
				catch (BadImageFormatException) { continue; }
			}

			Modules = moduleData.ToArray();
			return Modules;
		}

		/// <summary>
		/// Add Module Class.
		/// </summary>
		/// <param name="classes">Module Class.</param>
		private IModuleBase[] CreateModuleInstances(Type[] classes)
		{
			List<IModuleBase> moduleClasses = new List<IModuleBase>();

			foreach(var moduleClass in classes)
			{
				if(IsValidateClassType(moduleClass))
				{
					try
					{
						moduleClasses.Add((IModuleBase)Activator.CreateInstance(moduleClass));
					}
					catch (MissingMethodException)
					{
						FrameworkKernel.ErrorReport(string.Format(ErrorReportFormat, MissingConstructor, moduleClass.FullName));
						continue;
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
			DirectoryInfo Dir = new DirectoryInfo(ModulePath);
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
		/// <param name="moduleClass">Class To Be Checked</param>
		/// <returns>Validity Of The Class.</returns>
		private bool IsValidateClassType(Type moduleClass)
		{
			if (moduleClass.IsAssignableFrom(typeof(IModuleBase)) && moduleClass.IsPublic && !moduleClass.IsAbstract)
				return true;
			else
				return false;
		}

		private bool IsValidateProperty(IModuleBase validateModule, FileInfo fileInfo)
		{
			try
			{
				string name = validateModule.GetName;
				string version = validateModule.GetVersion;
				ModuleType type = validateModule.GetType;
				Form resultForm = validateModule.ResultForm;
			}
			catch(Exception e)
			{
				FrameworkKernel.ErrorReport(string.Format(ErrorReportFormat, fileInfo.FullName, e.ToString() + e.Message));
				return false;
			}

			return true;
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

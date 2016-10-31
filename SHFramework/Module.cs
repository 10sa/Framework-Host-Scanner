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
		private const string ModuleFolderName = "Module";

		private const string ModuleLoadFormat = "*.dll";

		private const string ErrorReportFormat = "Module Load Error | {0} | {1} ";

		private const string WorngFormatDll = "Wrong Format Dll.";
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
						if(IsValidateProperty(classInstances, moduleFile))
							moduleData.Add(new ModuleData(classInstances, moduleFile.Name, moduleFile.FullName));
					}

					if (lastCount != moduleData.Count)
						SHFrameworkKernel.Report(string.Format(ErrorReportFormat, NotFoundAssiganbleMethod, moduleFile.FullName), ReportType.Error);
				}
				// Ignore Exceptions. 
				catch (BadImageFormatException) { continue; }
			}

			Modules = moduleData.ToArray();
			return Modules;
		}

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
						SHFrameworkKernel.Report(string.Format(ErrorReportFormat, MissingConstructor, moduleClass.FullName), ReportType.Error);
						continue;
					}
				}
			}

			return moduleClasses.ToArray();
		}

		private FileInfo[] GetModuleFiles()
		{
			DirectoryInfo Dir = new DirectoryInfo(ModulePath);
			return Dir.GetFiles(ModuleLoadFormat);
		}

		private Type[] GetModuleClass(string fileAbsolutePath)
		{
			try
			{
				return Assembly.LoadFile(fileAbsolutePath).GetExportedTypes();
			}
			catch (BadImageFormatException exc)
			{
				SHFrameworkKernel.Report(string.Format(ErrorReportFormat, WorngFormatDll, exc.FileName), ReportType.Error);
				throw;
			}
			
		}

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
				SHFrameworkKernel.Report(string.Format(ErrorReportFormat, fileInfo.FullName, e.ToString() + e.Message), ReportType.Error);
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

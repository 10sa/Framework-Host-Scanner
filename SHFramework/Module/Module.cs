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
	class ModuleLoadControll
	{
		private const string ModuleFolderName = "Module";

		private const string ModuleLoadFormat = "*.dll";

		private const string ErrorReportFormat = "Module Load Error | {0} | {1} | ";

		private const string WorngFormatDll = "Wrong Format Dll.";
		private const string MissingConstructor = "Missing Constructor.";
		private const string NotFoundAssiganbleMethod = "Not Found Assiganble Method.";
		private const string NotValidateProperty = "Module Property Is Not Validate.";

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
						else
							SHFrameworkKernel.Report(string.Format(ErrorReportFormat, NotValidateProperty, moduleFile.Name), ReportType.Error);
					}

					if (lastCount != moduleData.Count)
						SHFrameworkKernel.Report(string.Format(ErrorReportFormat, NotFoundAssiganbleMethod, moduleFile.FullName), ReportType.Error);
				}
				// Ignore Exceptions. 
				catch (ModuleLoadException e)
				{
					SHFrameworkKernel.Report(e.Message, e.ReportMessageType);
				}
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
						throw new ModuleLoadException(string.Format(ErrorReportFormat, NotFoundAssiganbleMethod), ReportType.Error);
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
			catch (BadImageFormatException e)
			{
				throw new ModuleLoadException(string.Format(ErrorReportFormat, WorngFormatDll, e.FileName), ReportType.Error);
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
			catch(Exception)
			{
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

	class ModuleLoadException : ModuleExceptionFrame
	{
		public ModuleData Module { get; private set; }
		public ModuleLoadException(string message, ReportType reportType) : base(message, reportType) { }
	}
}

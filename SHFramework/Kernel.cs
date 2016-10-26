using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SHFramework.Module;
using SHFramework.Module.Interfaces;

namespace SHFramework
{
	class FrameworkKernel
	{
		private const string ErrorReportFormat = "[SHFramework | Error] {0}";
		private const string InfoReportFormat = "[SHFramework | Info] {0}";
		private const string StatusReportFormat = "[SHFramework | Status] {0}";
		private const string WarningReportFormat = "[SHFramework | Warning] {0}";

		private const string ModuleResultIsNull = "Module : {0} | Module Result Is Null.";
		private const string ModuleOutputIsNone = "Module : {0} | Module Output Is None.";
		private const string ModuleTypeNotMatched = "Module : {0} | Module Type Not Matched.";

		private ModuleLoadControll moduleController = new ModuleLoadControll();

		/// <summary>
		/// Report Info. (Error, Info, Status, Warning, Others...)
		/// </summary>
		/// <param name="message">Write Message.</param>
		/// <param name="reportType">Report Type.</param>
		public static void Report(string message, ReportType reportType)
		{
			if (reportType == ReportType.Error)
				Console.Error.WriteLine(string.Format(ErrorReportFormat, message));
			else if (reportType == ReportType.Info)
				Console.WriteLine(string.Format(InfoReportFormat, message));
			else if (reportType == ReportType.Status)
				Console.WriteLine(string.Format(StatusReportFormat, message));
			else if (reportType == ReportType.Warning)
				Console.WriteLine(string.Format(WarningReportFormat, message));
		}

		/// <summary>
		/// No Parameter Module Call. (None Parameter Options Module Call.)
		/// </summary>
		/// <returns></returns>
		public CallResultData[] DoWorkModules()
		{
			List<CallResultData> resultData = new List<CallResultData>();

			moduleController.Load();
			foreach (var modules in moduleController.Modules)
			{
				if(modules.Module.GetOptions == ModuleParameterOptions.None)
				{
					ModuleCallResult result;
					if((result = modules.Module.DoWork(null)) == ModuleCallResult.HaveData)
					{
						if (modules.Module.ResultForm == null)
							Report(string.Format(ModuleResultIsNull, modules.Module.GetName), ReportType.Warning);

						resultData.Add(new CallResultData(result, modules.Module.ResultForm, modules));
					}
					else if (result == ModuleCallResult.None)
					{
						Report(string.Format(ModuleOutputIsNone, modules.Module.GetName), ReportType.Info);
					}
				}
				else
				{
					Report(string.Format(ModuleTypeNotMatched, modules.Module.GetName), ReportType.Warning);
					continue;
				}
			}

			return resultData.ToArray();
		}
	}

	public enum ReportType
	{
		Info,
		Error,
		Warning,
		Status
	}

	public struct CallResultData
	{
		public ModuleCallResult Result;
		public Form ResultForm;
		public ModuleData Module;

		public CallResultData(ModuleCallResult callResult, Form resultForm, ModuleData module)
		{
			Result = callResult;
			ResultForm = resultForm;
			Module = module;

			return;
		}
	}
}

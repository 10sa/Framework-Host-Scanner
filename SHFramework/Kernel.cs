using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using System.Web;
using System.Net;

using SHFramework.Module;
using SHFramework.Module.Interfaces;

namespace SHFramework
{
	/// <summary>
	/// SHFramework Kernel/Main Class.
	/// </summary>
	public class SHFrameworkKernel
	{
		// Report Formats //
		private const string ErrorReportFormat = "[SHFramework | Error] <{0}>";
		private const string InfoReportFormat = "[SHFramework | Info] <{0}>";
		private const string StatusReportFormat = "[SHFramework | Status] <{0}>";
		private const string WarningReportFormat = "[SHFramework | Warning] <{0}>";
		private const string CustomReportFormat = "[SHFramework | {0}] <{1}>";
		// END //


		// Exception Messages //
		private const string ReportMessageIsNull = "Report Message Is Null.";
		// END //


		// Module Status Report Formats //
		private const string ModuleResultIsNull = "Module : {0} | Module Result Is Null.";
		private const string ModuleOutputIsNone = "Module : {0} | Module Output Is None.";
		private const string ModuleTypeNotMatched = "Module : {0} | Module Type Not Matched.";
        private const string ModuleCheckingErrorException = "Module : {0} | Module Checking Error. | Exception : {1}";
		// END //


		// Framework Messages //
		private const string FrameworkInitlizingStart = "Framework Initlizing...";
		private const string EndFrameworkInitlizing = "Framework Initlizing Done!";
		// END //


		// Private //
		private ModuleLoadControll moduleController = new ModuleLoadControll();
		// END //


		/// <summary>
		/// Report Info. (Error, Info, Status, Warning, Others...)
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="reportType">Report Type.</param>
		public static void Report(string message, ReportType reportType)
		{
			if (message != null)
			{
				if (reportType == ReportType.Error)
					Console.Error.WriteLine(string.Format(ErrorReportFormat, message));
				else if (reportType == ReportType.Info)
					Console.WriteLine(string.Format(InfoReportFormat, message));
				else if (reportType == ReportType.Status)
					Console.WriteLine(string.Format(StatusReportFormat, message));
				else if (reportType == ReportType.Warning)
					Console.Error.WriteLine(string.Format(WarningReportFormat, message));
			}
			else
				Report(string.Format(ErrorReportFormat, ReportMessageIsNull), ReportType.Error);
		}

		/// <summary>
		/// Custom Report Method.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="customReportType">Custon Report Type.</param>
		/// <param name="IsWriteErrorStream">Report Type.</param>
		public static void Report(string message, string customReportType, bool IsWriteErrorStream)
		{
			if (message != null && customReportType != null)
			{
				if (IsWriteErrorStream)
					Console.Error.WriteLine(string.Format(CustomReportFormat, customReportType, message));
				else
					Console.WriteLine(string.Format(CustomReportFormat, customReportType, message));
			}
			else
				throw new InvalidMethodArgsException(string.Format(ErrorReportFormat, ReportMessageIsNull), ReportType.Error);
			
		}

		/// <summary>
		/// Start Framework Initlizng.
		/// </summary>
		public SHFrameworkKernel()
		{
			Report(FrameworkInitlizingStart, ReportType.Info);

			// Framework Initlizing //
			moduleController.Load();
			// END //

			Report(EndFrameworkInitlizing, ReportType.Info);

            return;
		}

		/// <summary>
		/// Framework Modules Reloading.
		/// </summary>
		public void Reload()
		{
			moduleController.Load();
		}

		// Debug Method.
		/* public ModuleData[] Reload()
		{
			return moduleController.Load();
		} */

		/// <summary>
		/// No Parameter Module Call. (None Parameter Options Modules)
		/// </summary>
		/// <returns>Module Call Result.</returns>
		public CallResultData[] DoWorkModules()
		{
			List<CallResultData> resultData = new List<CallResultData>();
			
			foreach (var module in moduleController.Modules)
			{
				try
				{
					resultData.Add(GetModuleResult(module, ModuleParameterOptions.None, null));
				}
				catch(SHFrameworkExceptionFrame e) { Report(e.Message, e.ReportMessageType); }
			}

			return resultData.ToArray();
		}
		
		/// <summary>
		/// Uri Parameter Module Call (Uri Parameter Options Modules)
		/// </summary>
		/// <param name="address">Target Address.</param>
		/// <returns>Module Call Result.</returns>
		public CallResultData[] DoWorkModules(Uri address)
		{
            List<CallResultData> resultData = new List<CallResultData>();
            object[] moduleParameter = { address };

            foreach(var module in moduleController.Modules)
            {
				try
				{
					resultData.Add(GetModuleResult(module, ModuleParameterOptions.Uri, moduleParameter));
				}
				catch (SHFrameworkExceptionFrame e) { Report(e.Message, e.ReportMessageType); }
			}
			
			return resultData.ToArray();
		}

		/// <summary>
		/// IPAddress Parameter Module Call. (IP Parameter Options Modules)
		/// </summary>
		/// <param name="ipAddress">Target IP Address.</param>
		/// <returns>Module Call Result.</returns>
		public CallResultData[] DoWokrModules(IPAddress ipAddress)
        {
            List<CallResultData> resultData = new List<CallResultData>();
            object[] moduleParameter = { ipAddress };

            foreach(var module in moduleController.Modules)
            {
				try
				{
					resultData.Add(GetModuleResult(module, ModuleParameterOptions.IPAddress, moduleParameter));
				}
				catch (SHFrameworkExceptionFrame e) { Report(e.Message, e.ReportMessageType); }
			}

            return resultData.ToArray();
        }

		/// <summary>
		/// Custom Parameter Module Call. (Custom Paramter Options Modules),
		/// The Framework Does Not Guarantee The Stability Of This Method.
		/// </summary>
		/// <param name="moduleParameter">Custom Parameter(s).</param>
		/// <returns>Module Call Result.</returns>
		public CallResultData[] DoWorkModules(object[] moduleParameter)
		{
            List<CallResultData> resultData = new List<CallResultData>();
			
			foreach(var module in moduleController.Modules)
			{
				try
				{
					resultData.Add(GetModuleResult(module, ModuleParameterOptions.Custom, moduleParameter));
				}
				catch (SHFrameworkExceptionFrame e) { Report(e.Message, e.ReportMessageType); }
			}
			
			return resultData.ToArray();
		}

        private bool IsValidateResultForm(ModuleData module)
        {
			if (module.Module.ResultForm == null)
				return false;

			return true;
        }

		private CallResultData GetModuleResult(ModuleData module, ModuleParameterOptions options, object[] moduleParamter)
		{
			if (module.Module.GetOptions == options)
			{
				ModuleCallResult result;
				if ((result = module.Module.DoWork(moduleParamter)) == ModuleCallResult.HaveData)
				{
					if (IsValidateResultForm(module))
						return new CallResultData(result, module.Module.ResultForm, module);
					else
						throw new ModuleCallException(string.Format(ModuleResultIsNull, module.Module.GetName), ReportType.Error);
				}
				else
					throw new ModuleCallException(string.Format(ModuleOutputIsNone, module.Module.GetName), ReportType.Error);
			}
			else
				throw new ModuleCallException(string.Format(ModuleTypeNotMatched, module.Module.GetName), ReportType.Error);
		}
	}

	/// <summary>
	/// Report Message Type.
	/// </summary>
	public enum ReportType
	{
		/// <summary>
		/// Normally Info Message.
		/// </summary>
		Info,

		/// <summary>
		/// Normally Status Message.
		/// </summary>
		Status,

		/// <summary>
		/// Unusual Warning Message.
		/// </summary>
		Warning,

		/// <summary>
		/// Critical Error Message.
		/// </summary>
		Error
	}

	/// <summary>
	/// Module Call Result Data Struct.
	/// </summary>
	public struct CallResultData
	{
		/// <summary>
		/// Module Call Result.
		/// </summary>
		public ModuleCallResult Result;

		/// <summary>
		/// Module Result Form.
		/// </summary>
		public Form ResultForm;

		/// <summary>
		/// Original Module.
		/// </summary>
		public ModuleData Module;

		/// <summary>
		/// Initlizing Struct.
		/// </summary>
		/// <param name="callResult">Module Call Result.</param>
		/// <param name="resultForm">Module Result Form.</param>
		/// <param name="module">Original Module.</param>
		public CallResultData(ModuleCallResult callResult, Form resultForm, ModuleData module)
		{
			Result = callResult;
			ResultForm = resultForm;
			Module = module;

			return;
		}
	}

	/// <summary>
	/// Module Call Exception, See a Exception Message.
	/// </summary>
	public class ModuleCallException : ModuleExceptionFrame
	{
		/// <summary>
		/// Initlizing Exception Class.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="reportType"></param>
		public ModuleCallException(string message, ReportType reportType) : base(message, reportType) { }
	}
}
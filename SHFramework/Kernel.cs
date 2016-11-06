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
	class SHFrameworkKernel
	{
		// Report Formats //
		private const string ErrorReportFormat = "[SHFramework | Error] <{0}>";
		private const string InfoReportFormat = "[SHFramework | Info] <{0}>";
		private const string StatusReportFormat = "[SHFramework | Status] <{0}>";
		private const string WarningReportFormat = "[SHFramework | Warning] <{0}>";
		private const string CustonReportFormat = "[SHFramework | {0}] <{1}>";
		// END //

		// Module Status Report Formats //
		private const string ModuleResultIsNull = "Module : {0} | Module Result Is Null.";
		private const string ModuleOutputIsNone = "Module : {0} | Module Output Is None.";
		private const string ModuleTypeNotMatched = "Module : {0} | Module Type Not Matched.";
        private const string ModuleCheckingErrorException = "Module : {0} | Module Checking Error! | Exception {1}";
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
		/// Custom Report Method.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="customReportType">Custon Report Type.</param>
		/// <param name="IsWriteErrorStream">Report Type.</param>
		public static void Report(string message, string customReportType, bool IsWriteErrorStream)
		{
			if(IsWriteErrorStream)
				Console.Error.WriteLine(string.Format(CustonReportFormat, customReportType, message));
			else
				Console.WriteLine(string.Format(CustonReportFormat, customReportType, message));
		}


		// Kernel Init Point.
		public SHFrameworkKernel()
		{
			Report("Framework Initlizing...", ReportType.Info);

			// Framework Initlizing //
			moduleController.Load();
			// END //

			Report("Framework Initlizing Done!", ReportType.Info);

            return;
		}

		/// <summary>
		/// No Parameter Module Call. (None Parameter Options Module Call.)
		/// </summary>
		/// <returns></returns>
		public CallResultData[] DoWorkModules()
		{
			List<CallResultData> resultData = new List<CallResultData>();
			
			foreach (var module in moduleController.Modules)
			{
				if(module.Module.GetOptions == ModuleParameterOptions.None)
				{
					ModuleCallResult result;
					if((result = module.Module.DoWork(null)) == ModuleCallResult.HaveData)
					{
                        if(!IsValidateResultForm(module))
                            continue;

						resultData.Add(new CallResultData(result, module.Module.ResultForm, module));
					}
					else if (result == ModuleCallResult.None)
					{
						Report(string.Format(ModuleOutputIsNone, module.Module.GetName), ReportType.Info);
						continue;
					}
				}
				else
				{
					Report(string.Format(ModuleTypeNotMatched, module.Module.GetName), ReportType.Warning);
					continue;
				}
			}

			return resultData.ToArray();
		}
		
		
		// address call.
		public CallResultData[] DoWorkModules(Uri address)
		{
            List<CallResultData> resultData = new List<CallResultData>();
            object[] moduleParameter = { address };

            foreach(var module in moduleController.Modules)
            {
                if(module.Module.GetOptions == ModuleParameterOptions.Uri)
                {
					ModuleCallResult result = module.Module.DoWork(moduleParameter);

					if(result == ModuleCallResult.HaveData)
					{
						if(!IsValidateResultForm(module))
							continue;

						resultData.Add(new CallResultData(result, module.Module.ResultForm, module));
					}
					else
					{
						Report(string.Format(ModuleOutputIsNone, module.Module.GetName), ReportType.Info);
						continue;
					}
                }
                else
                {
                    Report(string.Format(ModuleTypeNotMatched, module.Module.GetName), ReportType.Warning);
                    continue;
                }
            }
			
			return resultData.ToArray();
		}

        // IPAddress Call.
        public CallResultData[] DoWokrModules(IPAddress ipAddress)
        {
            List<CallResultData> resultData = new List<CallResultData>();
            object[] moduleParameter = { ipAddress };

            foreach(var module in moduleController.Modules)
            {
                if(module.Module.GetOptions == ModuleParameterOptions.IPAddress)
                {
                    if(!IsValidateResultForm(module))
                        continue;

					resultData.Add(new CallResultData(module.Module.DoWork(moduleParameter), module.Module.ResultForm, module));
				}
                else
                {
                    Report(string.Format(ModuleTypeNotMatched, module.Module.GetName), ReportType.Warning);
                    continue;
                }
            }

            return resultData.ToArray();
        }
		
		// custom parameter call.
		public CallResultData[] DoWorkModules(object[] customParameter)
		{
            List<CallResultData> resultData = new List<CallResultData>();
			
			foreach(var module in moduleController.Modules)
			{
				if(module.Module.GetOptions == ModuleParameterOptions.CustomParamter)
				{
					ModuleCallResult result = module.Module.DoWork(customParameter);
					if(result == ModuleCallResult.HaveData)
					{
						if(!IsValidateResultForm(module))
							continue;

						resultData.Add(new CallResultData(result, module.Module.ResultForm, module));
					}
					else
					{
						Report(string.Format(ModuleTypeNotMatched, module.Module.GetName), ReportType.Warning);
						continue;
					}
				}
				else
				{
					Report(string.Format(ModuleTypeNotMatched, module.Module.GetName), ReportType.Warning);
					continue;
				}
			}
			
			return resultData.ToArray();
		}

        private bool IsValidateResultForm(ModuleData module)
        {
            try
            {
                if(module.Module.ResultForm == null)
                {
                    Report(string.Format(ModuleResultIsNull, module.Module.GetName), ReportType.Warning);
                    return false;
                }
            }
            catch(Exception e)
            {
                Report(string.Format(ModuleCheckingErrorException, module.Module.GetName, e.ToString()), ReportType.Error);
                return false;
            }
            

            return true;
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

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
		private ModuleLoadControll moduleController = new ModuleLoadControll();

		/// <summary>
		/// Error Report Method. (Write Stderr Stream)
		/// </summary>
		/// <param name="message">Write Message.</param>
		public static void ErrorReport(string message)
		{
			Console.Error.WriteLine("[SHFramework] " + message);
		}

		public CallResultData[] DoWorkModules()
		{
			List<CallResultData> resultData = new List<CallResultData>();

			moduleController.Load();
			foreach (var modules in moduleController.Modules)
			{
				if (modules.Module.GetOptions == ModuleParameterOptions.TargetAddress)
					ErrorReport(string.Format("Module : {0} | Module Type Not Matched.", modules.Module.GetName));

				if(modules.Module.GetOptions == ModuleParameterOptions.None)
				{
					ModuleCallResult result;
					if((result = modules.Module.DoWork(null)) == ModuleCallResult.HaveData)
					{
						if (modules.Module.ResultForm == null)
							ErrorReport(string.Format("Module : {0} | Module Result Form Is Null.", modules.Module.GetName));

						resultData.Add(new CallResultData(result, modules.Module.ResultForm));
					}
					else if (result == ModuleCallResult.None)
					{
						ErrorReport(string.Format("Module : {0} | Module No Output.", modules.Module.GetName));
					}
				}
			}

			return resultData.ToArray();
		}
	}

	public struct CallResultData
	{
		public ModuleCallResult Result;
		public Form ResultForm;

		public CallResultData(ModuleCallResult callResult, Form resultForm)
		{
			Result = callResult;
			ResultForm = resultForm;

			return;
		}
	}
}

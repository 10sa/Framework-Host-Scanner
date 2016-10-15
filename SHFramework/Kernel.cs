using System;
using System.Collections.Generic;
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

		public void DoWorkModules()
		{
			moduleController.Load();
			foreach (var modules in moduleController.Modules)
			{
				if (modules.Module.GetOptions == ModuleParameterOptions.TargetAddress)
					ErrorReport(string.Format("Module : {0} | Module Type Not Matched. (Not Call)", modules.Module.GetName));
			}
		}
	}
}

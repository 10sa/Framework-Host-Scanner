using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
	class fw_kernel
	{
		fw_module module = new fw_module();

		public fw_kernel()
		{

		}

		/// <summary>
		/// Error Report Method. (Write Stderr Stream.)
		/// </summary>
		/// <param name="Message">Write Message.</param>
		public static void WriteErrorReport(string Message)
		{
			Console.Error.WriteLine("[SHFramework] " + Message);
		}
	}
}

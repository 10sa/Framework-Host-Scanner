using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHFramework
{
	class Kernel
	{
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

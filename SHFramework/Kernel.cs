using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHFramework
{
	class FrameworkKernel
	{
		/// <summary>
		/// Error Report Method. (Write Stderr Stream)
		/// </summary>
		/// <param name="message">Write Message.</param>
		public static void ErrorReport(string message)
		{
			Console.Error.WriteLine("[SHFramework] " + message);
		}
	}
}

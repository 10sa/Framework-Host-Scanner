using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SHFramework.Module;

namespace SHFramework
{
	public class SHFrameworkExceptionFrame : Exception
	{
		public ReportType ReportMessageType { get; private set; }
		private SHFrameworkExceptionFrame() : base() { }

		protected SHFrameworkExceptionFrame(string message, ReportType reportType) : base(message)
		{
			ReportMessageType = reportType;
			return;
		}
	}

	public class ModuleExceptionFrame : SHFrameworkExceptionFrame
	{
		public ModuleExceptionFrame(string message, ReportType reportType) : base(message, reportType) { }
	}

	public class ModuleCallException : ModuleExceptionFrame
	{
		public ModuleCallException(string message, ReportType reportType) : base(message, reportType) { }
	}
}

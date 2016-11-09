using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHFramework
{
	/// <summary>
	/// SHFramework Exception Frame.
	/// </summary>
	public abstract class SHFrameworkExceptionFrame : Exception
	{
		public ReportType ReportMessageType { get; private set; }
		private SHFrameworkExceptionFrame() : base() { }

		protected SHFrameworkExceptionFrame(string message, ReportType reportType) : base(message)
		{
			ReportMessageType = reportType;
			return;
		}
	}

	/// <summary>
	/// Module Exception Frame.
	/// </summary>
	public class ModuleExceptionFrame : SHFrameworkExceptionFrame
	{
		public ModuleExceptionFrame(string message, ReportType reportType) : base(message, reportType) { }
	}

	/// <summary>
	/// Occurs When an invalid Argument's Passed.
	/// </summary>
	public class InvalidMethodArgsException : SHFrameworkExceptionFrame
	{

		public InvalidMethodArgsException(string message, ReportType reportType) : base(message, reportType) { }
	}
}

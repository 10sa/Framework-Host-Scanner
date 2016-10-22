using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SHFramework.Module.Interfaces
{
	/// <summary>
	/// Module Interface.
	/// </summary>
	public interface IModuleBase
	{
		ModuleType GetType { get; }

		string GetName { get; }

		string GetVersion { get; }
		
		ModuleParameterOptions GetOptions { get; }

		Form ResultForm { get; }
		
		/// <summary>
		/// Module Start Point.
		/// </summary>
		/// <param name="optionData"></param>
		/// <returns>Module Call Result.</returns>
		ModuleCallResult DoWork(object[] optionData);
	}
	
	/// <summary>
	/// Module Call Result.
	/// </summary>
	public enum ModuleCallResult
	{
		HaveData,
		None
	}
	
	public enum ModuleType
	{
		Global,
		Local,
		All
	}

	public enum ModuleParameterOptions
	{
		TargetAddress,
		None
	}
}

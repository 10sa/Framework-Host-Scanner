using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SHFramework.Module.Interfaces
{
	/// <summary>
	/// SHFramework Module Interface.
	/// </summary>
	public interface IModuleBase
	{
		/// <summary>
		/// Get Module Type.
		/// </summary>
		ModuleType GetType { get; }

		/// <summary>
		/// Get Module Name.
		/// </summary>
		string GetName { get; }

		/// <summary>
		/// Get Module Version.
		/// </summary>
		string GetVersion { get; }
		
		/// <summary>
		/// Get Module Paramter Options.
		/// </summary>
		ModuleParameterOptions GetOptions { get; }

		/// <summary>
		/// Get Module Result Form.
		/// </summary>
		Form ResultForm { get; }

		/// <summary>
		/// Module Start Point.
		/// </summary>
		/// <param name="optionData">Module Args. (The Data Depends On The Module's Options.)</param>
		/// <returns>Module Call Result.</returns>
		ModuleCallResult DoWork(object[] optionData);
	}
	
	/// <summary>
	/// Module Call Result Enum.
	/// </summary>
	public enum ModuleCallResult
	{
		/// <summary>
		/// There's a Result Of The Module.
		/// </summary>
		HaveData,

		/// <summary>
		/// No Result Of Module.
		/// </summary>
		None
	}
	
	/// <summary>
	/// Module Type Enum.
	/// </summary>
	public enum ModuleType
	{
		/// <summary>
		/// The Usage Range Of The Module's Global.
		/// </summary>
		/// 
		Global,

		/// <summary>
		/// The Module's Active Locally.
		/// </summary>
		Local,

		/// <summary>
		/// The Module has All Properties.
		/// </summary>
		All
	}

	/// <summary>
	/// Module Parameter Options Enum.
	/// </summary>
	public enum ModuleParameterOptions
	{
		/// <summary>
		/// The Argument Of The Module's IPAddress.
		/// </summary>
		IPAddress,

		/// <summary>
		/// The Argument Of The Module's Uri.
		/// </summary>
		Uri,

		/// <summary>
		/// The Argument Of The Module's Custom.
		/// </summary>
		Custom,

		/// <summary>
		/// /// <summary>
		/// Module has No Arguments.
		/// </summary>
		None
	}
}

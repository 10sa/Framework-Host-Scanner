using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SHFramework.Interfaces
{
	/// <summary>
	/// Module Interface.
	/// </summary>
	public interface IModuleBase
	{
		ModuleType GetModuleType { get; }
		
		ModuleOptions GetModuleOptions{ get; }
		
		ModuleCallResult ModuleMain(object ModuleOptionData);
	}
	
	public enum ModuleCallResult
	{
		HaveData,
		NoData
	}
	
	public enum ModuleType
	{
		Global,
		Loacl,
		All
	}

	public enum ModuleOptions
	{
		TargetAddress,
		None
	}
}

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
		
	}
	
	public enum ModuleType
	{
		NonLocal,
		Loacl
	}

	public enum ModuleRequestOptions
	{
		TargetAddress,
		None
	}
}

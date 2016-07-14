using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Fonts;
using MetroFramework.Forms;
using Framework;
using System.IO;

namespace _2016_KOI
{
	public partial class MainForm : MetroForm
	{
        ScannerFramework sf = new ScannerFramework();

		public MainForm ()
		{
			InitializeComponent();
		}

		private void MainForm_Load (object sender, EventArgs e)
		{
			ModuleStatusGrid.ColumnCount = 3;
			ModuleStatusGrid.ColumnHeadersVisible = true;
			ModuleStatusGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			ModuleStatusGrid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

			ModuleStatusGrid.Columns[0].Name = "모듈 이름";
			ModuleStatusGrid.Columns[1].Name = "모듈 버전";
			ModuleStatusGrid.Columns[2].Name = "모듈 상태";

			ScannerFramework Scanner = new ScannerFramework();
			
			
			foreach(var ModuleData in Scanner.ModuleControll.Data)
			{
				MessageBox.Show(ModuleData.Name);
				string[] Data = new string[] { ModuleData.Name, ModuleData.Module.ModuleVer, ModuleData.Status.ToString() };
				ModuleStatusGrid.Rows.Add(Data);
			}
		}

		private void StartCheck_button_Click (object sender, EventArgs e)
		{
            
        }
	}
}

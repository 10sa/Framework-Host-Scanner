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

namespace Form
{
	/// <summary>
	/// 모듈의 정보를 표시하고 관리하는 메인 폼입니다.
	/// </summary>
	public partial class MainForm : MetroForm
	{
		ScannerFramework Scanner;

		// 여러 Worker 에서 사용되는 Callback 메소드.
		private void AsyncAddtionCallback(object sender, RunWorkerCompletedEventArgs e)
		{
			foreach(var Data in Scanner.ModuleControll.Data)
			{
				string[] Row = new string[] { Data.Name, Data.Module.ModuleVer, Data.Status.ToString() };
				ModuleStatusGrid.Rows.Add(Row);
			}
		}

		public MainForm ()
		{
			InitializeComponent();
		}

		private void MainForm_Load (object sender, EventArgs e)
		{
			ModuleStatusGrid.ColumnCount = 4;
			ModuleStatusGrid.ColumnHeadersVisible = true;

			ModuleStatusGrid.Columns[0].Name = "모듈 이름";
			ModuleStatusGrid.Columns[1].Name = "모듈 버전";
			ModuleStatusGrid.Columns[2].Name = "모듈 상태";
			ModuleStatusGrid.Columns[3].Name = "모듈 정보";

			StartCheck_button.Enabled = false;
			ReloadButton.Enabled = false;
			BackgroundWorker FormLoadWorker = new BackgroundWorker();
			FormLoadWorker.DoWork += (a, b) =>
			{
				Scanner = new ScannerFramework();
			};

			FormLoadWorker.RunWorkerCompleted += AsyncAddtionCallback;
			FormLoadWorker.RunWorkerCompleted += (a, b) =>
			{
				StartCheck_button.Enabled = true;
				ReloadButton.Enabled = true;
			};

			FormLoadWorker.RunWorkerAsync();
		}

		private void StartCheck_button_Click (object sender, EventArgs e)
		{
			if(ServerAddress_textbox.Text == string.Empty)
			{
				MessageBox.Show("주소는 공란일수 없습니다.");
				return;
			}
				
			StartCheck_button.Enabled = false;
			ReloadButton.Enabled = false;

			BackgroundWorker AsyncWorker = new BackgroundWorker();
			AsyncWorker.DoWork += (a, b) =>
			{
				
			};
			AsyncWorker.RunWorkerCompleted += (a, b) =>
			{

			};

        }

		private void ReloadButton_Click(object sender, EventArgs e)
		{
			ReloadButton.Enabled = false;
			ModuleStatusGrid.Rows.Clear();

			BackgroundWorker AsyncWorker = new BackgroundWorker();
			AsyncWorker.DoWork += (a, b) =>
			{
				Scanner.ModuleControll.Reload();
			};

			AsyncWorker.RunWorkerCompleted += AsyncAddtionCallback;
			AsyncWorker.RunWorkerCompleted += (a, b) =>
			{
				ReloadButton.Enabled = true;
			};

			AsyncWorker.RunWorkerAsync();
		}

		private void ModuleStatusGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			//MessageBox.Show(e.ColumnIndex.ToString());
			if(e.ColumnIndex == 3)
			{
				InfoForm Info = new InfoForm();
				Info.InfoTextBox.Text = GetModuleData(e.RowIndex);
				Info.Text = Scanner.ModuleControll.Data[e.RowIndex].Name;
				Info.Show();
			}
		}

		private string GetModuleData(int Row)
		{
			return Scanner.ModuleControll.Data[Row].Module.IVulnerableInfo + Environment.NewLine;
		}
	}
}

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
		ScannerFramework Scanner;

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
			StartCheck_button.Enabled = false;
			ReloadButton.Enabled = false;

			BackgroundWorker AsyncWorker = new BackgroundWorker();
			AsyncWorker.DoWork += (a, b) =>
			{
				Scanner.VulnerablePointCheck(ServerAddress_textbox.Text);
			};
			AsyncWorker.RunWorkerCompleted += (a, b) =>
			{
				StartCheck_button.Enabled = false;
				MessageBox.Show("Progress Done.");
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


		// 여러 Worker 에서 사용되는 Callback 메소드.
		private void AsyncAddtionCallback(object sender, RunWorkerCompletedEventArgs e)
		{
			foreach(var Data in Scanner.ModuleControll.Data)
			{
				string[] Row = new string[] { Data.Name, Data.Module.ModuleVer, Data.Status.ToString() };
				ModuleStatusGrid.Rows.Add(Row);
			}
		}
	}
}

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
using Framework.Struct;
using Framework.Enum;
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
                string[] Row;
                if(Data.Status != Framework.Enum.ModuleStatus.Error)
				    Row = new string[] { Data.Name, Data.Module.ModuleVer, Data.Status.ToString(), string.Empty, "호출" };
                else
                    Row = new string[] { Data.Name, Data.Module.ModuleVer, Data.Status.ToString(), string.Empty, "에러" };

                ModuleStatusGrid.Rows.Add(Row);
			}
		}

		public MainForm ()
		{
			InitializeComponent();
		}

		private void MainForm_Load (object sender, EventArgs e)
		{
            StartCheck_button.Enabled = false;
            ReloadButton.Enabled = false;

            ModuleStatusGrid.ColumnCount = 5;
			ModuleStatusGrid.ColumnHeadersVisible = true;

			ModuleStatusGrid.Columns[0].Name = "모듈 이름";
			ModuleStatusGrid.Columns[1].Name = "모듈 버전";
			ModuleStatusGrid.Columns[2].Name = "모듈 상태";
			ModuleStatusGrid.Columns[3].Name = "모듈 정보";
            ModuleStatusGrid.Columns[4].Name = "호출 여부";

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
            if (ServerAddress_textbox.Text == string.Empty)
			{
				MessageBox.Show("주소를 입력해 주세요.");
				return;
			}

            List<ModuleCallResult> Result;

            StartCheck_button.Enabled = false;
			ReloadButton.Enabled = false;

			BackgroundWorker AsyncWorker = new BackgroundWorker();
			AsyncWorker.DoWork += (a, b) =>
			{
                Scanner.VulnerablePointCheck(ServerAddress_textbox.Text);
                
			};
			AsyncWorker.RunWorkerCompleted += (a, b) =>
			{
                Result = Scanner.Info;
                for (int i=0; i<Result.Count; i++)
                {
                    if((Result[i].Info != string.Empty) && (Result[i].Info != null))
                        ModuleStatusGrid[3, i].Value = "(클릭)";
                    else if(Scanner.Info[i].Result == CallResult.Exception)
                        ModuleStatusGrid[3, i].Value = "<에러>";
                    else if(Scanner.Info[i].Result == CallResult.Safe)
                        ModuleStatusGrid[3, i].Value = "[안전]";
                    else
                        ModuleStatusGrid[3, i].Value = "|정보 없음|";
                }
                    

                StartCheck_button.Enabled = true;
                ReloadButton.Enabled = true;
            };

            AsyncWorker.RunWorkerAsync();
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
            if(e.ColumnIndex == 3)
            {
                if(!ModuleStatusGrid[3, e.RowIndex].Value.Equals(string.Empty) && ModuleStatusGrid[3, e.RowIndex].Value != null)
                {
                    if(ModuleStatusGrid[3, e.RowIndex].Value.Equals("(클릭)"))
                    {
                        InfoForm Info = new InfoForm();
                        Info.InfoTextBox.Text = GetModuleData(e.RowIndex);
                        Info.Text = Scanner.ModuleControll.Data[e.RowIndex].Name;
                        Info.Show();
                    }
                }
			}

            if(e.ColumnIndex == 4)
            {
                if (ModuleStatusGrid[4, e.RowIndex].Value.Equals("호출"))
                {
                    ModuleStatusGrid[4, e.RowIndex].Value = "비호출";
                    Scanner.ModuleControll.SetModuleStatus(e.RowIndex, Framework.Enum.ModuleStatus.DontCall);
                }
                else if (ModuleStatusGrid[4, e.RowIndex].Value.Equals("비호출"))
                {
                    ModuleStatusGrid[4, e.RowIndex].Value = ("호출");
                    Scanner.ModuleControll.SetModuleStatus(e.RowIndex, Framework.Enum.ModuleStatus.Call);
                }
            }
		}

		private string GetModuleData(int Row)
		{
            if ((Scanner.Info[Row].Info == string.Empty) || (Scanner.Info[Row].Info == null))
                return string.Empty;
            else
                return Scanner.Info[Row].Info + Environment.NewLine;
        }


	}
}

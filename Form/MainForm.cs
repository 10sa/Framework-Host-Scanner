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
		}

		private void StartCheck_button_Click (object sender, EventArgs e)
		{
            
        }
	}
}

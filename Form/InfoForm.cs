using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace Form
{
	public partial class InfoForm : MetroForm
	{
		public InfoForm()
		{
			InitializeComponent();
			Visible = false;
		}

		private void InfoForm_Load(object sender, EventArgs e)
		{
			Visible = false;
		}
	}
}

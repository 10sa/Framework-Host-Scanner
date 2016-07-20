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
	/// <summary>
	/// 모듈의 호출 후 정보를 담기 위한 폼입니다.
	/// </summary>
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

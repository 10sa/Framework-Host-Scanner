namespace Form
{
	partial class MainForm
	{
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose (bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form 디자이너에서 생성한 코드

		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다. 
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
		/// </summary>
		private void InitializeComponent ()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ServerAddress_textbox = new MetroFramework.Controls.MetroTextBox();
            this.StartCheck_button = new MetroFramework.Controls.MetroButton();
            this.ServerAddressTextbox_Desc_Label = new MetroFramework.Controls.MetroLabel();
            this.ReloadButton = new MetroFramework.Controls.MetroButton();
            this.ModuleStatusGrid = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.ModuleStatusGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // ServerAddress_textbox
            // 
            resources.ApplyResources(this.ServerAddress_textbox, "ServerAddress_textbox");
            this.ServerAddress_textbox.FontSize = MetroFramework.MetroTextBoxSize.Medium;
            this.ServerAddress_textbox.Name = "ServerAddress_textbox";
            // 
            // StartCheck_button
            // 
            resources.ApplyResources(this.StartCheck_button, "StartCheck_button");
            this.StartCheck_button.Name = "StartCheck_button";
            this.StartCheck_button.Click += new System.EventHandler(this.StartCheck_button_Click);
            // 
            // ServerAddressTextbox_Desc_Label
            // 
            resources.ApplyResources(this.ServerAddressTextbox_Desc_Label, "ServerAddressTextbox_Desc_Label");
            this.ServerAddressTextbox_Desc_Label.Name = "ServerAddressTextbox_Desc_Label";
            // 
            // ReloadButton
            // 
            resources.ApplyResources(this.ReloadButton, "ReloadButton");
            this.ReloadButton.Name = "ReloadButton";
            this.ReloadButton.Click += new System.EventHandler(this.ReloadButton_Click);
            // 
            // ModuleStatusGrid
            // 
            this.ModuleStatusGrid.AllowUserToAddRows = false;
            this.ModuleStatusGrid.AllowUserToDeleteRows = false;
            this.ModuleStatusGrid.AllowUserToResizeColumns = false;
            this.ModuleStatusGrid.AllowUserToResizeRows = false;
            resources.ApplyResources(this.ModuleStatusGrid, "ModuleStatusGrid");
            this.ModuleStatusGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.ModuleStatusGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.ModuleStatusGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ModuleStatusGrid.Name = "ModuleStatusGrid";
            this.ModuleStatusGrid.ReadOnly = true;
            this.ModuleStatusGrid.RowTemplate.Height = 23;
            this.ModuleStatusGrid.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.ModuleStatusGrid_CellMouseDoubleClick);
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.Controls.Add(this.ModuleStatusGrid);
            this.Controls.Add(this.ReloadButton);
            this.Controls.Add(this.ServerAddressTextbox_Desc_Label);
            this.Controls.Add(this.StartCheck_button);
            this.Controls.Add(this.ServerAddress_textbox);
            this.Name = "MainForm";
            this.Style = MetroFramework.MetroColorStyle.Orange;
            this.TextAlign = System.Windows.Forms.VisualStyles.HorizontalAlign.Center;
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ModuleStatusGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private MetroFramework.Controls.MetroTextBox ServerAddress_textbox;
		private MetroFramework.Controls.MetroButton StartCheck_button;
		private MetroFramework.Controls.MetroLabel ServerAddressTextbox_Desc_Label;
		private MetroFramework.Controls.MetroButton ReloadButton;
		private System.Windows.Forms.DataGridView ModuleStatusGrid;
    }
}


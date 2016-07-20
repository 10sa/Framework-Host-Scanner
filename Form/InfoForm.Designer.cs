namespace Form
{
	partial class InfoForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.InfoTextBox = new MetroFramework.Controls.MetroTextBox();
			this.SuspendLayout();
			// 
			// InfoTextBox
			// 
			this.InfoTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.InfoTextBox.Location = new System.Drawing.Point(20, 60);
			this.InfoTextBox.MaxLength = 150000;
			this.InfoTextBox.Multiline = true;
			this.InfoTextBox.Name = "InfoTextBox";
			this.InfoTextBox.Size = new System.Drawing.Size(663, 441);
			this.InfoTextBox.TabIndex = 0;
			// 
			// InfoForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(703, 521);
			this.Controls.Add(this.InfoTextBox);
			this.Name = "InfoForm";
			this.Text = "InfoForm";
			this.Load += new System.EventHandler(this.InfoForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		public MetroFramework.Controls.MetroTextBox InfoTextBox;
	}
}
namespace myMiniGame
{
	partial class Form1
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
			if (disposing && (components != null))
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
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.bDown = new System.Windows.Forms.Button();
			this.bUp = new System.Windows.Forms.Button();
			this.textMaxTicks = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(900, 900);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.picRepaint);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(906, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 53);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Rule Set Description";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "label1";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.bDown);
			this.groupBox2.Controls.Add(this.bUp);
			this.groupBox2.Controls.Add(this.textMaxTicks);
			this.groupBox2.Location = new System.Drawing.Point(906, 72);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 54);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Tick Limit";
			// 
			// bDown
			// 
			this.bDown.Location = new System.Drawing.Point(113, 17);
			this.bDown.Name = "bDown";
			this.bDown.Size = new System.Drawing.Size(19, 23);
			this.bDown.TabIndex = 2;
			this.bDown.Text = "-";
			this.bDown.UseVisualStyleBackColor = true;
			this.bDown.Click += new System.EventHandler(this.bDown_Click);
			// 
			// bUp
			// 
			this.bUp.Location = new System.Drawing.Point(138, 17);
			this.bUp.Name = "bUp";
			this.bUp.Size = new System.Drawing.Size(19, 23);
			this.bUp.TabIndex = 1;
			this.bUp.Text = "+";
			this.bUp.UseVisualStyleBackColor = true;
			this.bUp.Click += new System.EventHandler(this.bUp_Click);
			// 
			// textMaxTicks
			// 
			this.textMaxTicks.Location = new System.Drawing.Point(7, 20);
			this.textMaxTicks.Name = "textMaxTicks";
			this.textMaxTicks.Size = new System.Drawing.Size(100, 20);
			this.textMaxTicks.TabIndex = 0;
			this.textMaxTicks.Text = "20";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(987, 133);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 4;
			this.button1.Text = "Do Nothing";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1124, 930);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.pictureBox1);
			this.KeyPreview = true;
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.form_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.form_KeyUp);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button bDown;
		private System.Windows.Forms.Button bUp;
		private System.Windows.Forms.TextBox textMaxTicks;
		private System.Windows.Forms.Button button1;
	}
}

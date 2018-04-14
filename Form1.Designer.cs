namespace AtentisPrimer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusData = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusTrans = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbConnect = new System.Windows.Forms.ToolStripButton();
            this.tsbDisconnect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbStartRobot = new System.Windows.Forms.ToolStripButton();
            this.tsbStopRobot = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cbAutoNewSession = new System.Windows.Forms.CheckBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.button_ClearFile = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusData,
            this.StatusTrans});
            this.statusStrip1.Location = new System.Drawing.Point(0, 460);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(857, 22);
            this.statusStrip1.TabIndex = 3;
            // 
            // StatusData
            // 
            this.StatusData.Name = "StatusData";
            this.StatusData.Size = new System.Drawing.Size(0, 17);
            // 
            // StatusTrans
            // 
            this.StatusTrans.Name = "StatusTrans";
            this.StatusTrans.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbConnect,
            this.tsbDisconnect,
            this.toolStripSeparator1,
            this.tsbStartRobot,
            this.tsbStopRobot,
            this.toolStripSeparator2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(857, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // tsbConnect
            // 
            this.tsbConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConnect.Image = ((System.Drawing.Image)(resources.GetObject("tsbConnect.Image")));
            this.tsbConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConnect.Name = "tsbConnect";
            this.tsbConnect.Size = new System.Drawing.Size(23, 22);
            this.tsbConnect.Text = "Connect";
            this.tsbConnect.Click += new System.EventHandler(this.tsbConnect_Click);
            // 
            // tsbDisconnect
            // 
            this.tsbDisconnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDisconnect.Image = ((System.Drawing.Image)(resources.GetObject("tsbDisconnect.Image")));
            this.tsbDisconnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDisconnect.Name = "tsbDisconnect";
            this.tsbDisconnect.Size = new System.Drawing.Size(23, 22);
            this.tsbDisconnect.Text = "Disconnect";
            this.tsbDisconnect.Click += new System.EventHandler(this.tsbDisconnect_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbStartRobot
            // 
            this.tsbStartRobot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStartRobot.Image = ((System.Drawing.Image)(resources.GetObject("tsbStartRobot.Image")));
            this.tsbStartRobot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStartRobot.Name = "tsbStartRobot";
            this.tsbStartRobot.Size = new System.Drawing.Size(23, 22);
            this.tsbStartRobot.Text = "toolStripButton1";
            this.tsbStartRobot.ToolTipText = "Start Robot";
            this.tsbStartRobot.Click += new System.EventHandler(this.tsbStartRobot_Click);
            // 
            // tsbStopRobot
            // 
            this.tsbStopRobot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStopRobot.Enabled = false;
            this.tsbStopRobot.Image = ((System.Drawing.Image)(resources.GetObject("tsbStopRobot.Image")));
            this.tsbStopRobot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStopRobot.Name = "tsbStopRobot";
            this.tsbStopRobot.Size = new System.Drawing.Size(23, 22);
            this.tsbStopRobot.Text = "toolStripButton2";
            this.tsbStopRobot.ToolTipText = "Stop Robot";
            this.tsbStopRobot.Click += new System.EventHandler(this.tsbStopRobot_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // timer1
            // 
            this.timer1.Interval = 180000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // cbAutoNewSession
            // 
            this.cbAutoNewSession.AutoSize = true;
            this.cbAutoNewSession.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbAutoNewSession.Location = new System.Drawing.Point(138, 5);
            this.cbAutoNewSession.Name = "cbAutoNewSession";
            this.cbAutoNewSession.Size = new System.Drawing.Size(107, 17);
            this.cbAutoNewSession.TabIndex = 11;
            this.cbAutoNewSession.Text = "AutoNewSession";
            this.cbAutoNewSession.UseVisualStyleBackColor = true;
            this.cbAutoNewSession.CheckedChanged += new System.EventHandler(this.cbAutoNewSession_CheckedChanged);
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(0, 31);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(857, 426);
            this.listView1.TabIndex = 12;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // button_ClearFile
            // 
            this.button_ClearFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.button_ClearFile.Location = new System.Drawing.Point(313, 5);
            this.button_ClearFile.Name = "button_ClearFile";
            this.button_ClearFile.Size = new System.Drawing.Size(82, 20);
            this.button_ClearFile.TabIndex = 13;
            this.button_ClearFile.Text = "KillFile";
            this.button_ClearFile.UseVisualStyleBackColor = false;
            this.button_ClearFile.Click += new System.EventHandler(this.button_ClearFile_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(857, 482);
            this.Controls.Add(this.button_ClearFile);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.cbAutoNewSession);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "Form1";
            this.Text = "AtentisPrimer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel StatusTrans;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton tsbConnect;
        private System.Windows.Forms.ToolStripButton tsbDisconnect;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton tsbStartRobot;
        private System.Windows.Forms.ToolStripButton tsbStopRobot;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripStatusLabel StatusData;
        private System.Windows.Forms.CheckBox cbAutoNewSession;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button button_ClearFile;
    }
}


namespace DFU
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.button_Ping = new System.Windows.Forms.Button();
            this.button_Upgrade = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button_searchDevice = new System.Windows.Forms.Button();
            this.comboBox_deviceIP = new System.Windows.Forms.ComboBox();
            this.comboBox_LocalIP = new System.Windows.Forms.ComboBox();
            this.button_File = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.button_Refresh = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_File = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.progressBar_Upgrade = new System.Windows.Forms.ProgressBar();
            this.textBox_Log = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.button_Ping, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.button_Upgrade, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 295);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(10, 0, 10, 10);
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(505, 48);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // button_Ping
            // 
            this.button_Ping.AutoSize = true;
            this.button_Ping.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Ping.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_Ping.Location = new System.Drawing.Point(13, 3);
            this.button_Ping.Name = "button_Ping";
            this.button_Ping.Size = new System.Drawing.Size(236, 32);
            this.button_Ping.TabIndex = 2;
            this.button_Ping.Text = "Ping";
            this.button_Ping.UseVisualStyleBackColor = true;
            this.button_Ping.Click += new System.EventHandler(this.button_Ping_Click);
            // 
            // button_Upgrade
            // 
            this.button_Upgrade.BackColor = System.Drawing.SystemColors.Control;
            this.button_Upgrade.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Upgrade.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_Upgrade.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(197)))), ((int)(((byte)(197)))));
            this.button_Upgrade.Location = new System.Drawing.Point(255, 3);
            this.button_Upgrade.Name = "button_Upgrade";
            this.button_Upgrade.Size = new System.Drawing.Size(237, 32);
            this.button_Upgrade.TabIndex = 1;
            this.button_Upgrade.Text = "升 级";
            this.button_Upgrade.UseVisualStyleBackColor = false;
            this.button_Upgrade.Click += new System.EventHandler(this.button_Upgrade_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.67442F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.32558F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.Controls.Add(this.button_searchDevice, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.comboBox_deviceIP, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.comboBox_LocalIP, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.button_File, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.button_Refresh, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBox_File, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(5, 5, 5, 2);
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(505, 89);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // button_searchDevice
            // 
            this.button_searchDevice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_searchDevice.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_searchDevice.Location = new System.Drawing.Point(382, 35);
            this.button_searchDevice.Name = "button_searchDevice";
            this.button_searchDevice.Size = new System.Drawing.Size(115, 21);
            this.button_searchDevice.TabIndex = 8;
            this.button_searchDevice.Text = "搜 索";
            this.button_searchDevice.UseVisualStyleBackColor = true;
            this.button_searchDevice.Click += new System.EventHandler(this.button_searchDevice_Click);
            // 
            // comboBox_deviceIP
            // 
            this.comboBox_deviceIP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox_deviceIP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_deviceIP.FormattingEnabled = true;
            this.comboBox_deviceIP.Location = new System.Drawing.Point(93, 35);
            this.comboBox_deviceIP.Name = "comboBox_deviceIP";
            this.comboBox_deviceIP.Size = new System.Drawing.Size(283, 20);
            this.comboBox_deviceIP.TabIndex = 7;
            // 
            // comboBox_LocalIP
            // 
            this.comboBox_LocalIP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox_LocalIP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_LocalIP.FormattingEnabled = true;
            this.comboBox_LocalIP.Location = new System.Drawing.Point(93, 8);
            this.comboBox_LocalIP.Name = "comboBox_LocalIP";
            this.comboBox_LocalIP.Size = new System.Drawing.Size(283, 20);
            this.comboBox_LocalIP.TabIndex = 4;
            // 
            // button_File
            // 
            this.button_File.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_File.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_File.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_File.Location = new System.Drawing.Point(382, 62);
            this.button_File.Name = "button_File";
            this.button_File.Size = new System.Drawing.Size(115, 22);
            this.button_File.TabIndex = 0;
            this.button_File.Text = "打 开";
            this.button_File.UseVisualStyleBackColor = true;
            this.button_File.Click += new System.EventHandler(this.button_File_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Right;
            this.label2.Location = new System.Drawing.Point(27, 5);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(5);
            this.label2.Size = new System.Drawing.Size(63, 27);
            this.label2.TabIndex = 3;
            this.label2.Text = "本地IP：";
            // 
            // button_Refresh
            // 
            this.button_Refresh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Refresh.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_Refresh.Location = new System.Drawing.Point(382, 8);
            this.button_Refresh.Name = "button_Refresh";
            this.button_Refresh.Size = new System.Drawing.Size(115, 21);
            this.button_Refresh.TabIndex = 5;
            this.button_Refresh.Text = "刷 新";
            this.button_Refresh.UseVisualStyleBackColor = true;
            this.button_Refresh.Click += new System.EventHandler(this.button_Refresh_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Right;
            this.label1.Location = new System.Drawing.Point(15, 59);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(5);
            this.label1.Size = new System.Drawing.Size(75, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "升级固件：";
            // 
            // textBox_File
            // 
            this.textBox_File.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_File.Location = new System.Drawing.Point(93, 62);
            this.textBox_File.Name = "textBox_File";
            this.textBox_File.ReadOnly = true;
            this.textBox_File.Size = new System.Drawing.Size(283, 21);
            this.textBox_File.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Right;
            this.label3.Location = new System.Drawing.Point(27, 32);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(5);
            this.label3.Size = new System.Drawing.Size(63, 27);
            this.label3.TabIndex = 6;
            this.label3.Text = "设备IP：";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.progressBar_Upgrade, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.textBox_Log, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 89);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 9F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(505, 206);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // progressBar_Upgrade
            // 
            this.progressBar_Upgrade.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar_Upgrade.Location = new System.Drawing.Point(13, 200);
            this.progressBar_Upgrade.Name = "progressBar_Upgrade";
            this.progressBar_Upgrade.Size = new System.Drawing.Size(479, 3);
            this.progressBar_Upgrade.Step = 1;
            this.progressBar_Upgrade.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar_Upgrade.TabIndex = 1;
            // 
            // textBox_Log
            // 
            this.textBox_Log.BackColor = System.Drawing.Color.Black;
            this.textBox_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Log.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_Log.ForeColor = System.Drawing.Color.Lime;
            this.textBox_Log.Location = new System.Drawing.Point(13, 3);
            this.textBox_Log.Multiline = true;
            this.textBox_Log.Name = "textBox_Log";
            this.textBox_Log.ReadOnly = true;
            this.textBox_Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Log.Size = new System.Drawing.Size(479, 191);
            this.textBox_Log.TabIndex = 0;
            this.textBox_Log.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBox_Log_MouseDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(505, 343);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DFU™";
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button button_Ping;
        private System.Windows.Forms.Button button_Upgrade;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button button_File;
        private System.Windows.Forms.ComboBox comboBox_LocalIP;
        private System.Windows.Forms.Button button_Refresh;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TextBox textBox_Log;
        private System.Windows.Forms.ProgressBar progressBar_Upgrade;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_File;
        private System.Windows.Forms.Button button_searchDevice;
        private System.Windows.Forms.ComboBox comboBox_deviceIP;
        private System.Windows.Forms.Label label3;
    }
}


namespace MediaClient
{
    partial class StartupDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartupDlg));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxServerName = new System.Windows.Forms.TextBox();
            this.textBoxServerPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.buttonSetupOK = new System.Windows.Forms.Button();
            this.buttonSetupCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(188, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(165, 32);
            this.label2.TabIndex = 1;
            this.label2.Text = "Server Port:";
            // 
            // textBoxServerName
            // 
            this.textBoxServerName.Location = new System.Drawing.Point(228, 82);
            this.textBoxServerName.Name = "textBoxServerName";
            this.textBoxServerName.Size = new System.Drawing.Size(342, 38);
            this.textBoxServerName.TabIndex = 2;
            this.textBoxServerName.Text = "127.0.0.1";
            // 
            // textBoxServerPort
            // 
            this.textBoxServerPort.Location = new System.Drawing.Point(228, 161);
            this.textBoxServerPort.Name = "textBoxServerPort";
            this.textBoxServerPort.Size = new System.Drawing.Size(342, 38);
            this.textBoxServerPort.TabIndex = 3;
            this.textBoxServerPort.Text = "554";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(43, 267);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(173, 32);
            this.label3.TabIndex = 4;
            this.label3.Text = "File To Play:";
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Location = new System.Drawing.Point(228, 267);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(342, 38);
            this.textBoxFileName.TabIndex = 5;
            this.textBoxFileName.Text = "movie.Mjpeg";
            // 
            // buttonSetupOK
            // 
            this.buttonSetupOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSetupOK.Location = new System.Drawing.Point(102, 432);
            this.buttonSetupOK.Name = "buttonSetupOK";
            this.buttonSetupOK.Size = new System.Drawing.Size(149, 54);
            this.buttonSetupOK.TabIndex = 6;
            this.buttonSetupOK.Text = "OK";
            this.buttonSetupOK.UseVisualStyleBackColor = true;
            // 
            // buttonSetupCancel
            // 
            this.buttonSetupCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonSetupCancel.Location = new System.Drawing.Point(353, 432);
            this.buttonSetupCancel.Name = "buttonSetupCancel";
            this.buttonSetupCancel.Size = new System.Drawing.Size(149, 54);
            this.buttonSetupCancel.TabIndex = 7;
            this.buttonSetupCancel.Text = "Cancel";
            this.buttonSetupCancel.UseVisualStyleBackColor = true;
            // 
            // StartupDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 619);
            this.Controls.Add(this.buttonSetupCancel);
            this.Controls.Add(this.buttonSetupOK);
            this.Controls.Add(this.textBoxFileName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxServerPort);
            this.Controls.Add(this.textBoxServerName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StartupDlg";
            this.Text = "Play Remote File";
            this.Load += new System.EventHandler(this.StartupDlg_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxServerName;
        private System.Windows.Forms.TextBox textBoxServerPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Button buttonSetupOK;
        private System.Windows.Forms.Button buttonSetupCancel;
    }
}
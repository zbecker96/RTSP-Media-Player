namespace MediaServer
{
    partial class FormMediaServer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMediaServer));
            this.backgroundControlChannel = new System.ComponentModel.BackgroundWorker();
            this.textDisplay = new System.Windows.Forms.RichTextBox();
            this.backgroundSendFile = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // backgroundControlChannel
            // 
            this.backgroundControlChannel.WorkerReportsProgress = true;
            this.backgroundControlChannel.WorkerSupportsCancellation = true;
            this.backgroundControlChannel.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundControlChannel_DoWork);
            this.backgroundControlChannel.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundControlChannel_RunWorkerCompleted);
            // 
            // textDisplay
            // 
            this.textDisplay.Location = new System.Drawing.Point(5, 28);
            this.textDisplay.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.textDisplay.Name = "textDisplay";
            this.textDisplay.Size = new System.Drawing.Size(524, 306);
            this.textDisplay.TabIndex = 0;
            this.textDisplay.Text = "";
            this.textDisplay.TextChanged += new System.EventHandler(this.textDisplay_TextChanged);
            // 
            // backgroundSendFile
            // 
            this.backgroundSendFile.WorkerReportsProgress = true;
            this.backgroundSendFile.WorkerSupportsCancellation = true;
            this.backgroundSendFile.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundSendFile_DoWork);
            // 
            // FormMediaServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(531, 332);
            this.Controls.Add(this.textDisplay);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.Name = "FormMediaServer";
            this.Text = "Media Server";
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundControlChannel;
        private System.Windows.Forms.RichTextBox textDisplay;
        private System.ComponentModel.BackgroundWorker backgroundSendFile;
    }
}


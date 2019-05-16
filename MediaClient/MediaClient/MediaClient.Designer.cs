namespace MediaClient
{
    partial class MediaClient
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MediaClient));
            this.buttonSetup = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonTearDown = new System.Windows.Forms.Button();
            this.textDisplay = new System.Windows.Forms.RichTextBox();
            this.videoTimer = new System.Windows.Forms.Timer(this.components);
            this.movieDisplay = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.movieDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonSetup
            // 
            this.buttonSetup.Location = new System.Drawing.Point(5, -2);
            this.buttonSetup.Name = "buttonSetup";
            this.buttonSetup.Size = new System.Drawing.Size(285, 61);
            this.buttonSetup.TabIndex = 0;
            this.buttonSetup.Text = "Setup";
            this.buttonSetup.UseVisualStyleBackColor = true;
            this.buttonSetup.Click += new System.EventHandler(this.buttonSetup_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Location = new System.Drawing.Point(288, -2);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(270, 61);
            this.buttonPlay.TabIndex = 1;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // buttonPause
            // 
            this.buttonPause.Location = new System.Drawing.Point(555, -2);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(281, 61);
            this.buttonPause.TabIndex = 2;
            this.buttonPause.Text = "Pause";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // buttonTearDown
            // 
            this.buttonTearDown.Location = new System.Drawing.Point(834, -2);
            this.buttonTearDown.Name = "buttonTearDown";
            this.buttonTearDown.Size = new System.Drawing.Size(218, 61);
            this.buttonTearDown.TabIndex = 3;
            this.buttonTearDown.Text = "Tear Down";
            this.buttonTearDown.UseVisualStyleBackColor = true;
            this.buttonTearDown.Click += new System.EventHandler(this.buttonTearDown_Click);
            // 
            // textDisplay
            // 
            this.textDisplay.Location = new System.Drawing.Point(10, 634);
            this.textDisplay.Name = "textDisplay";
            this.textDisplay.Size = new System.Drawing.Size(1042, 187);
            this.textDisplay.TabIndex = 4;
            this.textDisplay.Text = "";
            // 
            // videoTimer
            // 
            this.videoTimer.Interval = 10;
            this.videoTimer.Tick += new System.EventHandler(this.videoTimer_Tick);
            // 
            // movieDisplay
            // 
            this.movieDisplay.Location = new System.Drawing.Point(10, 65);
            this.movieDisplay.Name = "movieDisplay";
            this.movieDisplay.Size = new System.Drawing.Size(1042, 563);
            this.movieDisplay.TabIndex = 5;
            this.movieDisplay.TabStop = false;
            // 
            // MediaClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1059, 827);
            this.Controls.Add(this.movieDisplay);
            this.Controls.Add(this.buttonTearDown);
            this.Controls.Add(this.buttonPause);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.buttonSetup);
            this.Controls.Add(this.textDisplay);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MediaClient";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "MediaClient";
            ((System.ComponentModel.ISupportInitialize)(this.movieDisplay)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSetup;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Button buttonTearDown;
        private System.Windows.Forms.RichTextBox textDisplay;
        private System.Windows.Forms.Timer videoTimer;
        private System.Windows.Forms.PictureBox movieDisplay;
    }
}


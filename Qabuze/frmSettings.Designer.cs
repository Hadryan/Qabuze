namespace Qabuze
{
    partial class frmSettings
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
            this.txtAppId = new System.Windows.Forms.TextBox();
            this.txtAppSecret = new System.Windows.Forms.TextBox();
            this.txtUserAuthToken = new System.Windows.Forms.TextBox();
            this.lblAppId = new System.Windows.Forms.Label();
            this.lblAppSecret = new System.Windows.Forms.Label();
            this.lblUserAuthToken = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtFolderOutput = new System.Windows.Forms.TextBox();
            this.lblFolderPath = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtFolderStructure = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFilenames = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.fbdOutput = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // txtAppId
            // 
            this.txtAppId.Location = new System.Drawing.Point(100, 12);
            this.txtAppId.Name = "txtAppId";
            this.txtAppId.Size = new System.Drawing.Size(331, 20);
            this.txtAppId.TabIndex = 0;
            // 
            // txtAppSecret
            // 
            this.txtAppSecret.Location = new System.Drawing.Point(100, 38);
            this.txtAppSecret.Name = "txtAppSecret";
            this.txtAppSecret.Size = new System.Drawing.Size(331, 20);
            this.txtAppSecret.TabIndex = 1;
            // 
            // txtUserAuthToken
            // 
            this.txtUserAuthToken.Location = new System.Drawing.Point(100, 66);
            this.txtUserAuthToken.Name = "txtUserAuthToken";
            this.txtUserAuthToken.Size = new System.Drawing.Size(331, 20);
            this.txtUserAuthToken.TabIndex = 2;
            // 
            // lblAppId
            // 
            this.lblAppId.AutoSize = true;
            this.lblAppId.Location = new System.Drawing.Point(12, 15);
            this.lblAppId.Name = "lblAppId";
            this.lblAppId.Size = new System.Drawing.Size(35, 13);
            this.lblAppId.TabIndex = 3;
            this.lblAppId.Text = "AppId";
            // 
            // lblAppSecret
            // 
            this.lblAppSecret.AutoSize = true;
            this.lblAppSecret.Location = new System.Drawing.Point(12, 41);
            this.lblAppSecret.Name = "lblAppSecret";
            this.lblAppSecret.Size = new System.Drawing.Size(57, 13);
            this.lblAppSecret.TabIndex = 4;
            this.lblAppSecret.Text = "AppSecret";
            // 
            // lblUserAuthToken
            // 
            this.lblUserAuthToken.AutoSize = true;
            this.lblUserAuthToken.Location = new System.Drawing.Point(12, 69);
            this.lblUserAuthToken.Name = "lblUserAuthToken";
            this.lblUserAuthToken.Size = new System.Drawing.Size(82, 13);
            this.lblUserAuthToken.TabIndex = 5;
            this.lblUserAuthToken.Text = "UserAuthToken";
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSave.Location = new System.Drawing.Point(0, 182);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(443, 51);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtFolderOutput
            // 
            this.txtFolderOutput.Location = new System.Drawing.Point(100, 92);
            this.txtFolderOutput.Name = "txtFolderOutput";
            this.txtFolderOutput.Size = new System.Drawing.Size(295, 20);
            this.txtFolderOutput.TabIndex = 8;
            // 
            // lblFolderPath
            // 
            this.lblFolderPath.AutoSize = true;
            this.lblFolderPath.Location = new System.Drawing.Point(12, 95);
            this.lblFolderPath.Name = "lblFolderPath";
            this.lblFolderPath.Size = new System.Drawing.Size(71, 13);
            this.lblFolderPath.TabIndex = 9;
            this.lblFolderPath.Text = "Output Folder";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(394, 92);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(37, 20);
            this.button1.TabIndex = 10;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtFolderStructure
            // 
            this.txtFolderStructure.Location = new System.Drawing.Point(100, 119);
            this.txtFolderStructure.Name = "txtFolderStructure";
            this.txtFolderStructure.Size = new System.Drawing.Size(331, 20);
            this.txtFolderStructure.TabIndex = 11;
            this.txtFolderStructure.Text = "%albumartist%\\%album%/";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 122);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "(Sub-)Folder";
            // 
            // txtFilenames
            // 
            this.txtFilenames.Location = new System.Drawing.Point(100, 145);
            this.txtFilenames.Name = "txtFilenames";
            this.txtFilenames.Size = new System.Drawing.Size(331, 20);
            this.txtFilenames.TabIndex = 13;
            this.txtFilenames.Text = "%disknumber% - %tracknumber% - %title% - %album% - %artist%.%ext%";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 148);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Filenames";
            // 
            // fbdOutput
            // 
            this.fbdOutput.Description = "Please select a folder to download your music into";
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 233);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFilenames);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFolderStructure);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblFolderPath);
            this.Controls.Add(this.txtFolderOutput);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblUserAuthToken);
            this.Controls.Add(this.lblAppSecret);
            this.Controls.Add(this.lblAppId);
            this.Controls.Add(this.txtUserAuthToken);
            this.Controls.Add(this.txtAppSecret);
            this.Controls.Add(this.txtAppId);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmSettings";
            this.Text = "Qabuze Settings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAppId;
        private System.Windows.Forms.TextBox txtAppSecret;
        private System.Windows.Forms.TextBox txtUserAuthToken;
        private System.Windows.Forms.Label lblAppId;
        private System.Windows.Forms.Label lblAppSecret;
        private System.Windows.Forms.Label lblUserAuthToken;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtFolderOutput;
        private System.Windows.Forms.Label lblFolderPath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtFolderStructure;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFilenames;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FolderBrowserDialog fbdOutput;
    }
}
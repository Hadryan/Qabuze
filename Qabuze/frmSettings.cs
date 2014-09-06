using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Qabuze
{
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            this.txtAppId.Text = Properties.Settings.Default.AppId;
            this.txtAppSecret.Text = Properties.Settings.Default.AppSecret;
            this.txtUserAuthToken.Text = Properties.Settings.Default.UserAuthToken;
            this.txtFolderOutput.Text = Properties.Settings.Default.outputFolder;
            this.txtFilenames.Text = Properties.Settings.Default.filenames;
            this.txtFolderStructure.Text = Properties.Settings.Default.folderStructure;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AppId = this.txtAppId.Text;
            Properties.Settings.Default.AppSecret = this.txtAppSecret.Text;
            Properties.Settings.Default.UserAuthToken = this.txtUserAuthToken.Text;
            Properties.Settings.Default.outputFolder = this.txtFolderOutput.Text;
            Properties.Settings.Default.filenames = this.txtFilenames.Text;
            Properties.Settings.Default.folderStructure = this.txtFolderStructure.Text;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fbdOutput.ShowDialog();
            if (fbdOutput.SelectedPath != "")
            {
                txtFolderOutput.Text = fbdOutput.SelectedPath;
            }
        }
    }
}

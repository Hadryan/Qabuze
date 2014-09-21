/*
 * Qabuze: A Music-data-gatherer
 * Copyright (C) 2014 Hendrik 'Xendo' Meyer <code@xendosoft.de>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

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

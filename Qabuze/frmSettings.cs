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
            this.txtAppId.Text = QabuzeAPI.Config.getInstance().appId;
            this.txtAppSecret.Text = QabuzeAPI.Config.getInstance().appSecret;
            this.txtUserAuthToken.Text = QabuzeAPI.Config.getInstance().accounts.AsReadOnly()[0].getToken();
            this.txtFolderOutput.Text = QabuzeAPI.Config.getInstance().outputFolder;
            this.txtFilenames.Text = QabuzeAPI.Config.getInstance().fileScheme;
            this.txtFolderStructure.Text = QabuzeAPI.Config.getInstance().folderScheme;
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

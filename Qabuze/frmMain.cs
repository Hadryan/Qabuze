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
using System.Net;
using System.Threading;
using System.Windows.Shell;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Qabuze
{
    public partial class frmMain : Form
    {
        private QabuzeAlbum album = new QabuzeAlbum();
        private QabuzeArtist artist = new QabuzeArtist();
        public static frmMain instance;
        public frmMain()
        {
            InitializeComponent();
            instance = this;
            QabuzeAPI.init();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmSettings settings = new frmSettings();
            settings.ShowDialog();
        }
        
        private void frmMain_Load(object sender, EventArgs e)
        {
            //reset:
            #if(!WITHDOWNLOAD)
            button2.Hide();
            btnDLStat.Hide();
            #endif
            lblArtist.Text = "Nothing loaded";
            lblAlbum.Text = "";
            lblGenre.Text = "";
            lblLabel.Text = "";
            lblDuration.Text = "";
            lblRelease.Text = "";
            lblTracks.Text = "";
            lblEstSize.Text = "";
            lblVer.Text = "v" + typeof(frmMain).Assembly.GetName().Version.ToString();
            txtSearch.KeyPress += new KeyPressEventHandler(txtSearch_handler);
            AcceptButton = btnSearch;
            txtSearch.Focus();
            //end reset   
        }

        private void txtSearch_handler(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                btnSearch.PerformClick();
                e.Handled = true;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            album = QabuzeAlbum.queryAlbum(txtAlbumId.Text);
            if (album != null)
            {
                TimeSpan t = TimeSpan.FromSeconds((int)album.duration);
                string duration = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                t.Hours,
                                t.Minutes,
                                t.Seconds);

                pbArtwork.ImageLocation = album.coverURL;
                lblArtist.Text = album.artist;
                lblAlbum.Text = album.title;
                lblGenre.Text = album.genre;
                lblLabel.Text = album.label;
                lblDuration.Text = duration;
                lblEstSize.Text = "< " + (((t.Hours * 60) + t.Minutes) * 11) + " MiB *"; //Estimate uses 16 bit 44.1 KHz PCM. FLAC size WILL be lower!
                if (album.release_timestamp == null)
                {
                    lblRelease.Text = "no date";
                }else{
                    lblRelease.Text = Utils.UnixTimeStampToDateTime((int)album.release_timestamp).ToShortDateString();
                }
                lblTracks.Text = album.track_count.ToString();
                lblAvailability.Text = (album.availableForStreaming ? "Yes (until " : "No (available ");
                string until = album.availableUntil.ToShortDateString(), from = album.availableFrom.ToShortDateString();
                DateTime dateToDisplay = new DateTime(9999, 12, 30, 0, 0, 0); //Use this to accomodate for Localization-issues regarding date-format
                until = until.Replace(dateToDisplay.ToShortDateString(), "forever");
                from = from.Replace(dateToDisplay.ToShortDateString(), "at no time");
                lblAvailability.Text += (album.availableForStreaming ? until : from) + ")";
                lblAvailability.ForeColor = (album.availableForStreaming ? Color.DarkGreen : Color.DarkRed);
                #if(WITHDOWNLOAD)
                    button2.Enabled = album.availableForStreaming;
                #endif

                artist = QabuzeArtist.queryArtist(album.artistID.ToString());
                if (artist != null)
                {
                    txtArtist_Bio.Text = artist.biography;
                    pbArtist.ImageLocation = artist.imageURL;
                    Console.WriteLine(artist.biography);
                }
               
                //Console.WriteLine(album.songs[1].Value.getDownloadLink(true));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            #if(WITHDOWNLOAD)
                QabuzeDLM.DownloadAsync(album);
            #else
                MessageBox.Show("Download functionality was not included at compiletime!", "Download functionality not included");
            #endif
        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            frmSearch.instance.initSearch(txtSearch.Text, 0);
            frmSearch.instance.Show();
            frmSearch.instance.Focus();

        }

        public void setAlbumID(string album_id) {

            txtAlbumId.Text = album_id;
            btnGrabAlbumInfo.PerformClick();

        }

        private void btnDLStat_Click(object sender, EventArgs e)
        {
            #if(WITHDOWNLOAD)
                frmDLStat.instance.Show();
                frmDLStat.instance.Focus();
            #else
                MessageBox.Show("Download functionality was not included at compiletime!", "Download functionality not included");
            #endif
        }

        public void setProgressbarThreadSafe(int max, int value)
        {

            ThreadPool.QueueUserWorkItem(o => this.setProgressbar(max, value));

        }

        public void setProgressbarThreadSafe(int value)
        {

            ThreadPool.QueueUserWorkItem(o => this.setProgressbar(value));

        }

        private void setProgressbar(int max, int value)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.progressBar1.Maximum = max;
            this.setProgressbar(value);

        }
        private void setProgressbar(int value)
        {

            Control.CheckForIllegalCrossThreadCalls = false;
            this.progressBar1.Value = value;
            this.Text = "Qabuze - " + value.ToString() + "%";

        }

        private void button3_Click(object sender, EventArgs e)
        {
            (new frmLicense()).ShowDialog();
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

       

    }
}
